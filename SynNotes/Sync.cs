using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Data.SQLite;

namespace SynNotes {
  static class Sync {
    public static string Email { get; set; }
    public static string Password { get; set; }
    public static int Freq { get; set; }        // minutes, 0=Manual
    public static double LastSync { get; set; } // unixtime utc
    const string host = "https://simple-note.appspot.com";
    static CookieContainer cookies = new CookieContainer();
    static string Token = "";                   // auth token 
    private static string UserAgent = "SynNotes/" + Application.ProductVersion;

    /// <summary>
    /// validate email/pass, update token
    /// </summary>
    public static bool checkLogin() {
      var auth = "email=" + Email + "&password=" + Password;
      auth = Base64Encode(auth);
      try {
        foreach (Cookie c in cookies.GetCookies(new Uri(host))) c.Expired=true;
        Token = Request("/api/login", "POST", auth, "text/plain");
        return cookies.Count > 0;
      }
      catch {
        return false;
      }      
    }

    // REST call
    private static string Request(string Uri, string Method = "GET", string Data = "", string ContentType = "application/json") {
      //if (Uri.StartsWith("/api2/data") || Uri.StartsWith("/api2/tags/")) 
      if(Method == "POST") Uri += "?auth=" + Token + "&email=" + Email; // Cuz note API doesn't check cookies ;(
      var request = (HttpWebRequest)WebRequest.Create(host + Uri);
      request.Method = Method;
      request.ContentType = ContentType;
      request.UserAgent = UserAgent;
      request.Timeout = 10000; //10sec

      // send POST
      if (!string.IsNullOrEmpty(Data) && Method == "POST") {
        var bytes = Encoding.GetEncoding("UTF-8").GetBytes(Data);
        request.ContentLength = bytes.Length;
        using (var writeStream = request.GetRequestStream()) {
          writeStream.Write(bytes, 0, bytes.Length);
        }
      }
      request.CookieContainer = cookies;
      Debug.WriteLine("Headers: " + request.Headers);
      Debug.WriteLine("Cookies: " + request.CookieContainer.ToString());
      Debug.WriteLine("Data: " + Data);

      string responseValue;
      using (var response = (HttpWebResponse)request.GetResponse()) {        
        // accept only 200 OK
        if (response.StatusCode != HttpStatusCode.OK) {
          var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
          throw new ApplicationException(message);
        }
        // grab the response
        var responseStream = response.GetResponseStream();
        using (var reader = new StreamReader(responseStream)) {
          responseValue = reader.ReadToEnd();
        }
        if (response.Cookies.Count > 0) cookies.Add(response.Cookies); //save cookies
      }
      return responseValue;
    }

    /// <summary>
    /// reauth if request fails and try request again
    /// </summary>
    private static string RequestRetry(string Uri, string Method = "GET", string Data = "") {
      if (cookies.Count == 0 && !checkLogin()) throw new ApplicationException("Wrong login/password");
      try {
        return Request(Uri, Method, Data);
      }
      catch (WebException e) { // retry if 401-unauth cookie/token expired
        var ex = (HttpWebResponse)e.Response;
        if (ex.StatusCode == HttpStatusCode.Unauthorized) {
          if (checkLogin()) return Request(Uri, Method, Data);
          else throw new ApplicationException("Wrong login/password");
        }
        else if (ex.StatusCode == HttpStatusCode.NotFound) { // not found for update = create
          if (Uri.StartsWith("/api2/tags/")) return Request("/api2/tags", Method, Data);
          if (Uri.StartsWith("/api2/data/")) return Request("/api2/data", Method, Data);
          else throw;
        }
        else throw;
      }
    }

    /// <summary>
    /// get list of notes metadata changed since given time
    /// </summary>
    public static List<NoteMeta> getIndex(double since) {
      var result = new List<NoteMeta>();
      var js = new JavaScriptSerializer();
      var meta = new NotesMeta();

      do {
        var s = RequestRetry("/api2/index?length=100&since=" + since.ToString("R").Replace(',', '.') + "&mark=" + meta.mark);
        meta = js.Deserialize<NotesMeta>(s);
        result.AddRange(meta.data);
      } 
      while (!String.IsNullOrEmpty(meta.mark));

      return result;
    }

    /// <summary>
    /// return note data for given key
    /// </summary>
    internal static NoteData getNote(string key) {
      var js = new JavaScriptSerializer();
      var s = RequestRetry("/api2/data/" + key);
      return js.Deserialize<NoteData>(s);
    }

    /// <summary>
    /// create/update note and return new note data
    /// </summary>
    internal static NoteData pushNote(NoteItem note, SQLiteConnection sql) {
      var js = new JavaScriptSerializer();
      var node = new NoteDataUser();
      node.deleted = (note.Deleted) ? (byte)1 : (byte)0;
      node.modifydate = note.ModifyDate.ToString("R").Replace(',','.');
      node.tags = new string[note.Tags.Count];
      for (int i = 0; i < note.Tags.Count; i++) node.tags[i] = note.Tags[i].Name;

      //read additional info from db
      var systemtags="";
      using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
        cmd.CommandText = "SELECT content, createdate, systemtags, version FROM notes WHERE id=" + note.Id;
        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
          while (rdr.Read()) {
            node.content = rdr.GetString(0);
            node.createdate = rdr.GetDouble(1).ToString("R").Replace(',','.');;
            if (!rdr.IsDBNull(2)) systemtags = rdr.GetString(2);
            if (!rdr.IsDBNull(3)) node.version = rdr.GetInt32(3);
          }
        }
      }
      // fill systemtags
      if (note.Pinned) systemtags += " pinned";
      if (note.Unread) systemtags += " unread";
      if (!string.IsNullOrEmpty(note.Lexer)) systemtags += " sn-lexer=" + note.Lexer;
      node.systemtags = systemtags.Trim().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
      var data = js.Serialize(node);

      var url = (string.IsNullOrEmpty(note.Key)) ? "/api2/data" : "/api2/data/"+note.Key; // create/update
      var s = RequestRetry(url, "POST", data);
      return js.Deserialize<NoteData>(s);
    }

    /// <summary>
    /// delete note permanently
    /// </summary>
    internal static void delNote(NoteItem note) {
      RequestRetry("/api2/data/" + note.Key, "DELETE");
    }

    /// <summary>
    /// get tags index
    /// </summary>
    internal static List<TagMeta> getTags() {
      var result = new List<TagMeta>();
      var js = new JavaScriptSerializer();
      var meta = new TagsMeta();

      do {
        var s = RequestRetry("/api2/tags?length=1000&mark=" + meta.mark);
        meta = js.Deserialize<TagsMeta>(s);
        result.AddRange(meta.tags);
      }
      while (!String.IsNullOrEmpty(meta.mark));

      return result;
    }

    /// <summary>
    /// push tag order info
    /// </summary>
    internal static TagMeta pushTag(TagItem i) {
      var js = new JavaScriptSerializer();
      var node = new TagMeta();
      node.index = i.Index;
      node.name = i.Name;
      node.version = i.Version;
      var data = js.Serialize(node);
      var url = (i.Version == 0) ? "/api2/tags" : "/api2/tags/"+i.Name;
      var s = RequestRetry(url, "POST", data);
      return js.Deserialize<TagMeta>(s);
    }

    /// <summary>
    /// delete tag permanently
    /// </summary>
    internal static void delTag(TagItem tag) {
      RequestRetry("/api2/tags/" + tag.Name, "DELETE");
    }

    public static string Base64Encode(string plainText) {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return System.Convert.ToBase64String(plainTextBytes);
    }
  }

  //used for deserialize json index
  class NotesMeta {
    public string mark { get; set; }
    public NoteMeta[] data { get; set; }
  }
  class NoteMetaUser {
    public byte deleted { get; set; }
    public string modifydate { get; set; }
    public string createdate { get; set; }
    public string[] systemtags { get; set; }
    public string[] tags { get; set; }
    public int version { get; set; }
  }
  class NoteMeta : NoteMetaUser {
    public int syncnum { get; set; }
    public string key { get; set; }
  }
  // get note by key
  class NoteData : NoteMeta {
    public string content { get; set; }
  }
  // push new note to server
  class NoteDataUser : NoteMetaUser {
    public string content { get; set; }
  }

  class TagsMeta {
    public string mark { get; set; }
    public TagMeta[] tags { get; set; }
  }
  class TagMeta {
    public string name { get; set; }
    public int index { get; set; }
    public int version { get; set; }
  }
}
