using System.Net;

namespace AnnouncementCreatorBot;

public class GetRequest
{
    private HttpWebRequest _request;
    private string _address;

    public string Response { get; set; }
    public GetRequest(string address)
    {
        _address = address;
    }

    public void Run()
    {
        _request = (HttpWebRequest)WebRequest.Create(_address);
        _request.Method = "Get";

        try
        {
            var response = (HttpWebResponse) _request.GetResponse();
            var stream = response.GetResponseStream();
            if (stream != null)
                Response = new StreamReader(stream).ReadToEnd();
        }
        catch (Exception)
        {
            Console.WriteLine(typeof(Exception));
        }
    }
}