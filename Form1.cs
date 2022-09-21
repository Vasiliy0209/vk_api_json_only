using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.Xml.Serialization;
using CookComputing.XmlRpc;
using MicrosoftTranslator;
using System.Runtime.Serialization;


namespace Messenger
{ 

    public partial class Form1 : Form
    {
        public string vk_access_token = "";
       
        public struct LJPostEventArg
        {
            public string username;
            public string password;
            [XmlRpcMember("event")]
            public string ljevent;
            public string lineendings;
            public string subject;
            public int year;
            public int mon;
            public int day;
            public int hour;
            public int min;
        }

        public struct LJRetVal
        {
            public int itemid;
            public int anum;
            public string url;
        }

       
        [XmlRpcUrl("http://www.livejournal.com/interface/xmlrpc")]
        public interface ILJService
        {
            [XmlRpcMethod("LJ.XMLRPC.postevent")]
            LJRetVal postevent(LJPostEventArg posteventarg);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Публикация в ЖЖ
            if (cb_LJ.Checked) { 
            ILJService LJ = XmlRpcProxyGen.Create<ILJService>();
            LJPostEventArg arg = new LJPostEventArg();
            arg.day = DateTime.Now.Day;
            arg.hour = DateTime.Now.Hour;
            arg.min = DateTime.Now.Minute;
            arg.mon = DateTime.Now.Month;
            arg.year = DateTime.Now.Year;
            arg.username = "inf_study";
            arg.password = "do520xLpim4";
            arg.lineendings = "pc";
            arg.subject = richTextBox1.Text;//"Hello from DotNet2!";
            arg.ljevent = richTextBox1.Text;
            

                try
                {
                    LJRetVal link=LJ.postevent(arg);
                    MessageBox.Show("Сообщение успешно опубликовано ["+link.url+"]");
                }
                catch(XmlRpcIllFormedXmlException ex)
                {
                    MessageBox.Show("Сообщение не опубликованно. Ошибка ["+ex.Message+"]");
                }
            }

            //Публикация в ВК
            if(cb_VK.Checked)
            {
                if(vk_access_token.Trim()=="")
                {
                    BrowserDialog bd = new BrowserDialog();
                    bd.ShowDialog();
                    vk_access_token = bd.vk_access_token;

                    ChannelFactory<VK_API> vk_proxy = new ChannelFactory<VK_API>("VK_ENDPOINT");
                    var vk_proxy_obj = vk_proxy.CreateChannel();
                    VK_Response resp = vk_proxy_obj.WallPost("219075416", richTextBox1.Text, vk_access_token);
                }
            }
            
        }

        private void tcb_Language_Click(object sender, EventArgs e)
        {
            AdmAuthentication admAuth = new AdmAuthentication("b4da5e0c-9e5e-4ccd-a568-dc6a3feab1d7", "/A2RXiPRJAEEobs9MevhqspwLNeTvTCxpRUQf7fPpbQ=");
            AdmAccessToken admToken = admAuth.GetAccessToken();

            MicrosoftTranslator.LanguageServiceClient cl = new MicrosoftTranslator.LanguageServiceClient();

            //Добавление к SOAP-запросу HTTP-заголовка вида Authorization: Bearer <токен>
            HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
            httpRequestProperty.Headers.Add("Authorization", "Bearer " + admToken.access_token);
            using (OperationContextScope scope = new OperationContextScope(cl.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                //Вызов метода               
                string[] languages = cl.GetLanguagesForTranslate("");
                
                tcb_Language.Items.Clear();

                tcb_Language.Items.Add("<выберите язык>");
                
                foreach (string language in languages)                
                    tcb_Language.Items.Add(language);    
            }

            tcb_Language.SelectedIndex = 0;            
        }

        private void tbtn_Translate_Click(object sender, EventArgs e)
        {
            //Получение токена
            AdmAuthentication admAuth = new AdmAuthentication("b4da5e0c-9e5e-4ccd-a568-dc6a3feab1d7", "/A2RXiPRJAEEobs9MevhqspwLNeTvTCxpRUQf7fPpbQ=");
            AdmAccessToken admToken = admAuth.GetAccessToken();

            //Создание прокси-класса
            MicrosoftTranslator.LanguageServiceClient cl = new MicrosoftTranslator.LanguageServiceClient();

            //Добавление HTTP-заголовка Authorization: Bearer <токен>
            HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
            httpRequestProperty.Headers.Add("Authorization", "Bearer " + admToken.access_token);
            using (OperationContextScope scope = new OperationContextScope(cl.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                //Запрос метода Translate  (перевод)
                richTextBox1.SelectedText=cl.Translate("", richTextBox1.SelectedText, "", tcb_Language.Text, "text/plain", "", "");
            }
        }

        private void tb_Speller_Click(object sender, EventArgs e)
        {
            //Создание прокси-класса
            YandexSpeller.SpellService cl = new YandexSpeller.SpellService();
            //Запрос метода checkText, который выполнит проверку орфографии
            YandexSpeller.SpellError[] errors = cl.checkText(richTextBox1.SelectedText,"ru",0,"");
            //Формирование сообщения пользователю
            string message="",variants="";
            foreach (YandexSpeller.SpellError err in errors)
            {                
                foreach (string variant in err.s)
                    variants += variant+"\n";
                message += "Слово [" + err.word + "]\nВозможные варианты:\n" + variants;
            }
            //Вывод сообщения пользователю
            MessageBox.Show(message);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }

    //VK REST
    [ServiceContract]
    [DataContractFormat]
    public interface VK_API
    {
        [WebGet(UriTemplate = "method/wall.post?owner_id={owner_id}&message={message}&access_token={access_token}&v=5.81", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        VK_Response WallPost(string owner_id, string message, string access_token);
    }

    [DataContract]
    public class VK_Response
    {
        [DataMember]
        public string post_id;
    }
}
