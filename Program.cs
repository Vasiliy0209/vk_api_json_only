using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace FromScratch
{
    [ServiceContract]
    [DataContractFormat]
    public interface VK_API
    {
        [WebGet(UriTemplate = "method/wall.post?owner_id={owner_id}&message={message}&access_token={access_token}&v=5.81", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        VK_Response WallPost(string owner_id,string message,string access_token);
    }

    [DataContract]
    public class VK_Response
    {
        [DataMember]
        public VK_Response_inner response;

        [DataMember]
        public VK_Error_inner error;
    }

    [DataContract]
    public class VK_Response_inner
    {
        [DataMember]
        public string post_id;
    }

    [DataContract]
    public class VK_Error_inner
    {
        [DataMember]
        public string error_code;
        [DataMember]
        public string error_msg;
    }

    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<VK_API> vk_api = new ChannelFactory<VK_API>("VK_ENDPOINT");
            var proxy = vk_api.CreateChannel();
            //try
           // {
            VK_Response err = proxy.WallPost("219075416", "Hello for new", "vk1.a.sCajadJ80IVvzpbHzgw0C7UAlaitcWQwkUGX8CXMEjnjO26FsotUkUZsQCppUz4ngwU5P6iTd3fqwXivx-z-_swhiUFCeyCgo6pBW3YTOpHTPnJUmS4E_8w4QWRjlzVcu12wWX7D__s7LyVGRlG1CdCJL7RAjyqrcM3Cz6WJ4LcERAjX-UCsn-EJ-eD_Zo0H");
            if (err.error != null)
                Console.WriteLine("Ошибка ["+err.error.error_msg+"]");
            else
                Console.WriteLine("Успех post_id=["+err.response.post_id+"]");

            Console.ReadLine();
                //}
           // catch(System.Runtime.Serialization.SerializationException ex)
           // {
             //   ex.
            //}
            ((IDisposable)vk_api).Dispose();
        }
    }
}
