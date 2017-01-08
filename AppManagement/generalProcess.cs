using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AppManagement
{
    class generalProcess
    {
        Boolean ret = false;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Çalışan uygulamaların listesi.
        /// </summary>
        /// <returns></returns>
        public Process[] getProcess()
        {
            Process[] localProcesses = Process.GetProcesses();

            return localProcesses;
        }

        /// <summary>
        /// Xml dosyasını yükler.
        /// </summary>
        public void loadXml()
        {
            this.xmlWriter();
        }

        /// <summary>
        /// Çalışan uygulamaları xml dosyasına yazar.
        /// </summary>
        private void xmlWriter()
        {
            if (!File.Exists("AppManagement.xml"))
            {
                FileStream fileStream = File.Create("AppManagement.xml");
                fileStream.Close();
            }
            
            XmlTextWriter xmlolustur = new XmlTextWriter("AppManagement.xml", UTF8Encoding.UTF8);//Dosyanın oluşturulacağı dizin,Kodlaması
            xmlolustur.WriteStartDocument();//Xml içine Element Oluşturma işlemine başlanıyor.
            xmlolustur.WriteStartElement("AppManagement");//AppManagement Etiketi ekledik.
            xmlolustur.WriteEndDocument();//Element Oluşturma işlemi bitti.
            xmlolustur.Close();//Dosya oluşturuldu ve bağlantı kapatıldı.

            foreach (Process p in this.getProcess())
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle)
                    || p.ProcessName == "Skype"
                    || p.ProcessName == "Dropbox")
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load("AppManagement.xml");

                    XmlElement UserElement = doc.CreateElement("App");//Element Ekledik.
                    UserElement.SetAttribute("Name", p.ProcessName);//elemente bir attribute atadık

                    //Dropbox skype gibi programlar bazen arka planda çalıştığı için mainWindowTitle boş atanıyor.
                    //Bundan dolayı label olarak processName veriyoruz.
                    XmlElement labelElement = doc.CreateElement("Label");

                    switch (p.ProcessName)
                    {
                        case "Dropbox":
                            labelElement.InnerText = p.ProcessName;
                            UserElement.AppendChild(labelElement);
                            ret = true;
                            break;

                        case "Skype":
                            labelElement.InnerText = p.ProcessName;
                            UserElement.AppendChild(labelElement);
                            ret = true;
                            break;
                    }

                    //Ön planda çalışıyorsa, görünen adını atıyoruz.
                    if (!ret)
                    {
                        labelElement.InnerText = p.MainWindowTitle;
                        UserElement.AppendChild(labelElement);
                    }
                    
                    XmlElement StartTimeElement = doc.CreateElement("StartTime");
                    StartTimeElement.InnerText = p.StartTime.ToString();
                    UserElement.AppendChild(StartTimeElement);

                    doc.DocumentElement.AppendChild(UserElement);//xml dosyamıza element ve kayıtları ekledik

                    XmlTextWriter xmleEkle = new XmlTextWriter("AppManagement.xml", null);//xml dosyamıza fiziksel olarak kayıtları yazıyoruz
                    xmleEkle.Formatting = Formatting.Indented;
                    doc.WriteContentTo(xmleEkle);//kayıtlar eklendi
                    xmleEkle.Close();//dosya kapatıldı

                    ret = false;
                }
            }
        }

        /// <summary>
        /// Gönderilen xml verisini DataGridview'a yazar.
        /// </summary>
        /// <param name="_xmlFilePath"></param>
        /// <param name="_dataGridView"></param>
        public void listDatagridview(string _xmlFilePath, DataGridView _dataGridView)
        {
            XmlDocument i = new XmlDocument();
            DataSet ds = new DataSet();
            //xml dosyamızı okumak için ir reader oluşturuyoruz.
            XmlReader xmlReader;
            //readerin içine pathini verdiğimiz dosyayı dolduruyoruz.
            //burada önemli olan bir nokta var.ya path imizin başına @ yazacağız ya da çift // kullanacağız.
            xmlReader = XmlReader.Create(_xmlFilePath, new XmlReaderSettings());
            //içeriği Dataset e aktarıyoruz.
            ds.ReadXml(xmlReader);
            //gridviewin kaynağı olarak dataseti gösteriyoruz.
            _dataGridView.DataSource = ds.Tables[0];

            //Kolon başlıkları.
            _dataGridView.Columns[0].HeaderText = "Görünen Ad";
            _dataGridView.Columns[1].HeaderText = "Başlatılma zamanı";
            _dataGridView.Columns[2].HeaderText = "Program Adı";

            //Kolon sıralaması
            _dataGridView.Columns[0].DisplayIndex = 0;
            _dataGridView.Columns[1].DisplayIndex = 2;
            _dataGridView.Columns[2].DisplayIndex = 1;

            //Kolonların otomatik genişlik ayarı.
            _dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _dataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            xmlReader.Close();
        }

        /// <summary>
        /// Çalışan uygulamayı sonlandırır.
        /// </summary>
        /// <param name="_processName">Uygulama adı.</param>
        public void killProcess(string _processName)
        {
            foreach (var item in this.getProcess())
            {
                if (item.ProcessName == _processName)
                {
                    item.Kill();
                }
            }
        }
    }
}
