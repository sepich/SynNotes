using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace SynNotes {
  static class Sync {
    public static string Email { get; set; }
    public static string Password { get; set; }
    public static int Freq { get; set; } // minutes, 0=Manual
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
    /// get list of notes metadata
    /// </summary>
    public static List<NoteItem> getIndex() {
      var result = new List<NoteItem>();
      if (cookies.Count == 0 && !checkLogin()) throw new ApplicationException("Wrong login/password");


      return result;
    }




    public static string Base64Encode(string plainText) {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return System.Convert.ToBase64String(plainTextBytes);
    }
  }
}
