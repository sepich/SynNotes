using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace SynNotes {
  static class Sync {
    public static string Email { get; set; }
    public static string Password { get; set; }
    public static int Freq { get; set; }        // minutes, 0=Manual
    public static int LastSync { get; set; }    // unixtime utc
    const string host = "https://simple-note.appspot.com";
    static CookieContainer cookies = new CookieContainer();


    /// <summary>
    /// validate email/pass, update token
    /// </summary>
    public static bool checkLogin() {
      var auth = "email=" + Email + "&password=" + Password;
      auth = Base64Encode(auth);
      try {
        foreach (Cookie c in cookies.GetCookies(new Uri(host))) c.Expired=true;
        Request("/api/login", "POST", auth, "text/plain");
        return cookies.Count > 0;
      }
      catch {
        return false;
      }      
    }

    // REST call
    private static string Request(string Uri, string Method = "GET", string data = "", string ContentType = "application/json") {
      var request = (HttpWebRequest)WebRequest.Create(host + Uri);
      request.Method = Method;
      request.ContentType = ContentType;
      request.UserAgent = "SynNotes/0.1";
      request.Timeout = 10000; //10sec

      // send POST
      if (!string.IsNullOrEmpty(data) && Method == "POST") {
        var bytes = Encoding.GetEncoding("UTF-8").GetBytes(data);
        request.ContentLength = bytes.Length;
        using (var writeStream = request.GetRequestStream()) {
          writeStream.Write(bytes, 0, bytes.Length);
        }
      }
      request.CookieContainer = cookies;
      Debug.WriteLine("Headers: " + request.Headers);
      Debug.WriteLine("Cookies: " + request.CookieContainer.ToString());
      Debug.WriteLine("Data: " + data);

      using (var response = (HttpWebResponse)request.GetResponse()) {
        string responseValue;
        // accept only 200 OK
        if (response.StatusCode != HttpStatusCode.OK) {
          var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
          throw new ApplicationException(message);
        }
        // grab the response
        using (var responseStream = response.GetResponseStream()) {
          using (var reader = new StreamReader(responseStream)) {
            responseValue = reader.ReadToEnd();
          }
        }
        if (response.Cookies.Count > 0) cookies.Add(response.Cookies); //save cookies
        response.Close();
        return responseValue;
      }
    }

    /// <summary>
    /// reauth if request fails and try request again
    /// </summary>
    private static string RequestRetry(string Uri) {
      if (cookies.Count == 0 && !checkLogin()) throw new ApplicationException("Wrong login/password");
      try {
        return Request(Uri);
      }
      catch {
        if (checkLogin()) return Request(Uri);
        else throw new ApplicationException("Wrong login/password");
      }
    }

    /// <summary>
    /// get list of notes metadata changed since given time
    /// </summary>
    public static List<NoteMeta> getIndex(float since) {
      var result = new List<NoteMeta>();
      var js = new JavaScriptSerializer();
      var meta = new NotesMeta();

      do {
        var s = RequestRetry("/api2/index?length=100&since=" + since + "&mark=" + meta.mark);
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


    public static string Base64Encode(string plainText) {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return System.Convert.ToBase64String(plainTextBytes);
    }


  }

  //used for deserialize json
  class NotesMeta {
    public string mark { get; set; }
    public NoteMeta[] data { get; set; }
  }
  class NoteMeta {
    public float modifydate { get; set; }
    public string[] tags { get; set; }
    public byte deleted { get; set; }
    public float createdate { get; set; }
    public string[] systemtags { get; set; }
    public int version { get; set; }
    public int syncnum { get; set; }
    public string key { get; set; }
  }
  class NoteData : NoteMeta {
    public string content { get; set; }
  }
}
