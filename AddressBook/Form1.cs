using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace AddressBook
{
    public partial class Form1 : Form
    {
        int index;
        List<Person> people = new List<Person>();
        List<PersonIndex> searchPeople;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("settings.xml"))
            {
                XmlTextWriter xw = new XmlTextWriter("settings.xml", Encoding.UTF8);
                xw.WriteStartElement("People");
                xw.WriteEndElement();
                xw.Close();
            }
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("settings.xml");
            foreach (XmlNode xNode in xdoc.SelectNodes("People/Person"))
            {
                Person p = new Person();
                p.Name = xNode.SelectSingleNode("Name").InnerText;
                p.EmailID = xNode.SelectSingleNode("Email").InnerText;
                p.LINE = xNode.SelectSingleNode("Facebook").InnerText;
                p.Phone = xNode.SelectSingleNode("Phone").InnerText;
                p.Notes = xNode.SelectSingleNode("Notes").InnerText;
                p.Birthday = DateTime.FromFileTime(Convert.ToInt64(xNode.SelectSingleNode("Birthday").InnerText));
                people.Add(p);
                listBox.Items.Add(p.Name);
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                index = listBox.SelectedIndex;
                textBox1.Text = people[index].Name;
                textBox2.Text = people[index].EmailID;
                textBox3.Text = people[index].LINE;
                textBox4.Text = people[index].Phone;
                textBox5.Text = people[index].Notes;
                dateTimePicker1.Value = people[index].Birthday;
            }
            catch { }
        }

        private void add_Click(object sender, EventArgs e)
        {
            Person p = new Person();
            p.Name = textBox1.Text;
            p.EmailID = textBox2.Text;
            p.LINE = textBox3.Text;
            p.Birthday = dateTimePicker1.Value;
            p.Phone = textBox4.Text;
            p.Notes = textBox5.Text;
            people.Add(p);
            listBox.Items.Add(p.Name);
            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = "";
            dateTimePicker1.Value = DateTime.Now;
        }

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                people[index].Name = textBox1.Text;
                people[index].EmailID = textBox2.Text;
                people[index].LINE = textBox3.Text;
                people[index].Phone = textBox4.Text;
                people[index].Notes = textBox5.Text;
                people[index].Birthday = dateTimePicker1.Value; ;
                listBox.Items[index] = textBox1.Text;
            }
            catch
            { }
        }

        private void remove_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = "";
                dateTimePicker1.Value = DateTime.Now;
                people.RemoveAt(index);
                listBox.Items.RemoveAt(index);
            }
            catch { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("settings.xml");
            XmlNode xnode = xdoc.SelectSingleNode("People");
            xnode.RemoveAll();
            foreach (Person p in people)
            {
                XmlNode xTop = xdoc.CreateElement("Person");
                XmlNode xName = xdoc.CreateElement("Name");
                XmlNode xEmail = xdoc.CreateElement("Email");
                XmlNode xFacebook = xdoc.CreateElement("Facebook");
                XmlNode xNotes = xdoc.CreateElement("Notes");
                XmlNode xBirthday = xdoc.CreateElement("Birthday");
                XmlNode xPhone = xdoc.CreateElement("Phone");
                xName.InnerText = p.Name;
                xEmail.InnerText = p.EmailID;
                xFacebook.InnerText = p.LINE;
                xNotes.InnerText = p.Notes;
                xBirthday.InnerText = p.Birthday.ToFileTime().ToString();
                xPhone.InnerText = p.Phone;
                xTop.AppendChild(xName);
                xTop.AppendChild(xEmail);
                xTop.AppendChild(xFacebook);
                xTop.AppendChild(xNotes);
                xTop.AppendChild(xBirthday);
                xTop.AppendChild(xPhone);
                xdoc.DocumentElement.AppendChild(xTop);
            }
            xdoc.Save("settings.xml");
        }

        private void search_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            if (textBox6.Text != "")
            {
                var p = people
                   .Select((x, i) => new PersonIndex() { Person = x, Index = i })
                   .Where(x => x.Person == textBox6.Text)
                   .Select(x => x);
                searchPeople = new List<PersonIndex>(p);
                foreach (var sp in searchPeople)
                    listBox1.Items.Add(sp.Person.Name);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sindex;
            try
            {
                sindex = listBox1.SelectedIndex;
                index = searchPeople[sindex].Index;
                listBox.SelectedIndex = index;
                textBox1.Text = people[index].Name;
                textBox2.Text = people[index].EmailID;
                textBox3.Text = people[index].LINE;
                textBox4.Text = people[index].Phone;
                textBox5.Text = people[index].Notes;
                dateTimePicker1.Value = people[index].Birthday;
            }
            catch { }
        }
    }
    class PersonIndex
    {
        public Person Person { get; set; }
        public int Index { get; set; }
    }
    class Person
    {
        public string Name
        {
            get;
            set;
        }
        public string EmailID
        {
            get;
            set;
        }
        public string LINE
        {
            get;
            set;
        }
        public string Phone
        {
            get;
            set;
        }
        public string Notes
        {
            get;
            set;
        }
        public DateTime Birthday
        {
            get;
            set;
        }

        public static bool operator ==(Person p, string s)
        {
            if (p.Name == s || p.EmailID == s || s == p.LINE || s == p.Phone)
                return true;
            return false;
        }

        public static bool operator !=(Person p, string s)
        {
            if (p.Name == s || p.EmailID == s || s == p.LINE || s == p.Phone)
                return false;
            return true;
        }
    }
}
