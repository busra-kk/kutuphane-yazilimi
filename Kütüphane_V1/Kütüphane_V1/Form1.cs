using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.Mail;
using System.Net;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Office.Interop.Excel;



namespace Kütüphane_V1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            SqlAdres_textBox.Text = Settings.Default.Sql_Data_Source;
            DatabaseName_textBox.Text = Settings.Default.Sql_İnitial_Catalog;
            KitapTabloİsmi_textBox.Text = Settings.Default.Kitaplar_Tablo;
            FilmTabloİsmi_textBox.Text = Settings.Default.Filmler_Tablo;
            KullanıcılarTabloİsmi_textBox.Text = Settings.Default.Kullanıcılar_Tablo;
            KiralananlarTabloismi_textBox.Text = Settings.Default.Kiralananlar_Tablo;

            Guest guest = new Guest();
            guest.Show();
        }
        private void Login_button_Click(object sender, EventArgs e)
        {
            string KULLANICIADI = Settings.Default.KULLANICIADI;
            string ŞİFRE = Settings.Default.ŞİFRE;

            if (KullanıcıAd_textBox.Text == KULLANICIADI && Şifre_textBox.Text == ŞİFRE)
            {
                Login_panel.Visible = false;
            }
            else if (KullanıcıAd_textBox.Text == "" && Şifre_textBox.Text == "")
            {
                MessageBox.Show("Bilgilerinizi Eksik Girdiniz.Lütfen Kontrol Ediniz.");
            }
            else
                MessageBox.Show("Bilgilerinizi kontrol ediniz.", "Hatalı giriş!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Kaydet_button_Click(object sender, EventArgs e)
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("insert into " + Settings.Default.Kullanıcılar_Tablo + " values (@Ad,@Soyad,@Telefon,@TCKimlikNo,@EPosta,@Adres,@DoğumTarihi,@ÜyelikTarihi,@KullanıcıAd,@Şifre,@Kredi)"))
                {
                    cmd.Connection = sql_cnn;
                    sql_cnn.Open();
                    cmd.Parameters.AddWithValue("@Ad", Ad_textBox.Text);
                    cmd.Parameters.AddWithValue("@Soyad", Soyad_textBox.Text);
                    cmd.Parameters.AddWithValue("@Telefon", Telefon_maskedTextBox.Text);
                    cmd.Parameters.AddWithValue("@TCKimlikNo", TCKimlik_maskedTextBox.Text);
                    cmd.Parameters.AddWithValue("@Eposta", EPosta_textBox.Text);
                    cmd.Parameters.AddWithValue("@Adres", Adres_textBox.Text);
                    cmd.Parameters.AddWithValue("@DoğumTarihi", DoğumT_maskedTextBox.Text);
                    cmd.Parameters.AddWithValue("@ÜyelikTarihi", ÜyelikT_maskedTextBox.Text);
                    cmd.Parameters.AddWithValue("@KullanıcıAd", Kullanıcıadı_textBox.Text);
                    cmd.Parameters.AddWithValue("Şifre", Sifre_textBox.Text);
                    cmd.Parameters.AddWithValue("Kredi", Kkredi_textBox.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("BİR YENİ KULLANICI EKLENDİ");
                    Kullanıcılistele();
                }
            }
        }
        public void Kullanıcılistele()
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Kullanıcılar_Tablo, sql_cnn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    System.Data.DataTable myDataTable = new System.Data.DataTable();
                    sda.Fill(myDataTable);
                    if (this.Kitaplar_dataGridView.InvokeRequired)
                        Invoke(new MethodInvoker(() =>
                        {
                            Kullanıcılar_dataGridView.DataSource = myDataTable;
                        }));
                    else
                        Kullanıcılar_dataGridView.DataSource = myDataTable;
                }
            }
        }


        private void Kullanıcılar_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            KullanıcıID_textBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[0].Value.ToString();
            Ad_textBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[1].Value.ToString();
            Soyad_textBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[2].Value.ToString();
            Telefon_maskedTextBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[3].Value.ToString();
            TCKimlik_maskedTextBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[4].Value.ToString();
            EPosta_textBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[5].Value.ToString();
            Adres_textBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[6].Value.ToString();
            DoğumT_maskedTextBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[7].Value.ToString();
            ÜyelikT_maskedTextBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[8].Value.ToString();
            Kullanıcıadı_textBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[9].Value.ToString();
            Sifre_textBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[10].Value.ToString();
            Kkredi_textBox.Text = Kullanıcılar_dataGridView.CurrentRow.Cells[11].Value.ToString();
        }

        private void Listele_button_Click(object sender, EventArgs e)
        {
            Kullanıcılistele();
            TEMİZLE();
        }

        private void Güncelle_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (Kullanıcılar_dataGridView.CurrentRow.Cells[0].Value.ToString() == KullanıcıID_textBox.Text)
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlCommand cmd = new SqlCommand("update " + Settings.Default.Kullanıcılar_Tablo + " set Ad=@Ad,Soyad=@Soyad,Telefon=@Telefon,TCKimlikNo=@TCKimlikNo,EPosta=@EPosta,Adres=@Adres,DoğumTarihi=@DoğumTarihi,ÜyelikTarihi=@ÜyelikTarihi,KullanıcıAd=@KullanıcıAd,Şifre=@Şifre,Kredi=@Kredi where KullanıcıID=@KullanıcıID"))
                        {
                            cmd.Connection = sql_cnn;
                            sql_cnn.Open();
                            cmd.Parameters.AddWithValue("@KullanıcıID", KullanıcıID_textBox.Text);
                            cmd.Parameters.AddWithValue("@Ad", Ad_textBox.Text);
                            cmd.Parameters.AddWithValue("@Soyad", Soyad_textBox.Text);
                            cmd.Parameters.AddWithValue("@Telefon", Telefon_maskedTextBox.Text);
                            cmd.Parameters.AddWithValue("@TCKimlikNo", TCKimlik_maskedTextBox.Text);
                            cmd.Parameters.AddWithValue("@Eposta", EPosta_textBox.Text);
                            cmd.Parameters.AddWithValue("@Adres", Adres_textBox.Text);
                            cmd.Parameters.AddWithValue("@DoğumTarihi", DoğumT_maskedTextBox.Text);
                            cmd.Parameters.AddWithValue("@ÜyelikTarihi", ÜyelikT_maskedTextBox.Text);
                            cmd.Parameters.AddWithValue("@KullanıcıAd", Kullanıcıadı_textBox.Text);
                            cmd.Parameters.AddWithValue("Şifre", Sifre_textBox.Text);
                            cmd.Parameters.AddWithValue("Kredi", Kkredi_textBox.Text);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("KULLANICI BİLGİLERİ GÜNCELLENDİ");
                            Kullanıcılistele();
                            TEMİZLE();
                        }
                    }
                }
                else
                    MessageBox.Show("KULLANICI ID DEĞİŞTİRİLEMEZ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Sil_button_Click(object sender, EventArgs e)
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("Delete from " + Settings.Default.Kullanıcılar_Tablo + " where KullanıcıID=@KullanıcıID"))
                {
                    cmd.Connection = sql_cnn;
                    sql_cnn.Open();
                    cmd.Parameters.AddWithValue("@KullanıcıID", Kullanıcılar_dataGridView.CurrentRow.Cells["KullanıcıID"].Value.ToString());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("BİR KULLANICI SİLİNDİ");
                    Kullanıcılistele();
                    TEMİZLE();
                }
            }
        }


        private void search_textBox_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("select * from " + Settings.Default.Kullanıcılar_Tablo + " where TCKimlikNo like '%" + search_textBox.Text + "%'", sql_cnn))
                {
                    SqlCommandBuilder build = new SqlCommandBuilder(sda);
                    DataSet DS = new DataSet();
                    sda.Fill(DS, "Kullanıcılar");
                    Kullanıcılar_dataGridView.DataSource = DS.Tables["Kullanıcılar"];
                }
            }
        }


        public void TEMİZLE()
        {
            foreach (Control item in KullanıcıBilgileri_groupBox.Controls)
            {
                if (item is System.Windows.Forms.TextBox)
                {
                    item.Text = "";
                }
                else if (item is MaskedTextBox)
                    item.Text = "";
            }
        }

        private void Kitapkaydet_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (Kitapişlemleri_groupBox != null)
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("insert into " + Settings.Default.Kitaplar_Tablo + " values ((select MAX(KitapID)+1 from dbo.Kitaplar),@KitapAdı,@YazarAdı,@YayınEvi,@Kategori,@StokSayısı,@İçerik,@DolapNo,@RafNo,@Sıra,@Kredi,@KapakResmi)"))
                        {
                            sqlCommand.Connection = sql_cnn;
                            sql_cnn.Open();
                            sqlCommand.Parameters.AddWithValue("@KitapID", KitapID_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@KitapAdı", KitapAd_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@YazarAdı", YazarAd_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@YayınEvi", YayınEvi_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@Kategori", Kategori_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@StokSayısı", StokSayısı_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@İçerik", İçerik_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@DolapNo", DolapNo_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@RafNo", RafNo_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@Sıra", SıraNo_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@Kredi", Kredi_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@KapakResmi", KapakResmi_textBox.Text);
                            sqlCommand.ExecuteNonQuery();
                            MessageBox.Show("YENİ KİTAP EKLENDİ.");
                            Kitaplistele();
                        }
                    }
                }
                else
                    MessageBox.Show("LÜTFEN EKSİK BİLGİLERİ DOLDURUNUZ!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("KİTAP KAYDEDİLİRKEN BİR HATA OLUŞTU: " + ex.Message);
            }
        }

        private void ResimSeç_button_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Resim seç";
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "PNG Files (*.png) | *.png | JPG Files (*.jpg) | *.jpg |JPEG Files (*.jpeg)|*.jpeg|  All Files(*.*) | *.*";
            openFileDialog1.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog1.FileName;
            KapakResmi_textBox.Text = openFileDialog1.FileName;
        }

        private void KitapListele_button_Click(object sender, EventArgs e)
        {
            Kitaplistele();
        }
        public void Kitaplistele()
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Kitaplar_Tablo, sql_cnn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    System.Data.DataTable myDataTable = new System.Data.DataTable();
                    sda.Fill(myDataTable);
                    if (this.Kitaplar_dataGridView.InvokeRequired)
                        Invoke(new MethodInvoker(() =>
                        {
                            Kitaplar_dataGridView.DataSource = myDataTable;
                        }));
                    else
                        Kitaplar_dataGridView.DataSource = myDataTable;
                }
            }
        }

        private void Kitaplar_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            KitapID_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[0].Value.ToString();
            KitapAd_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[1].Value.ToString();
            YazarAd_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[2].Value.ToString();
            YayınEvi_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[3].Value.ToString();
            Kategori_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[4].Value.ToString();
            StokSayısı_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[5].Value.ToString();
            İçerik_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[6].Value.ToString();
            DolapNo_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[7].Value.ToString();
            RafNo_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[8].Value.ToString();
            SıraNo_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[9].Value.ToString();
            Kredi_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[10].Value.ToString();
            KapakResmi_textBox.Text = Kitaplar_dataGridView.CurrentRow.Cells[11].Value.ToString();

            pictureBox1.ImageLocation = Kitaplar_dataGridView.CurrentRow.Cells[11].Value.ToString();

        }

        private void KitapGüncelle_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (KitapID_textBox.Text == Kitaplar_dataGridView.CurrentRow.Cells[0].Value.ToString())
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlCommand cmd = new SqlCommand("update " + Settings.Default.Kitaplar_Tablo + " set KitapAdı=@KitapAdı,YazarAdı=@YazarAdı,YayınEvi=@YayınEvi,Kategori=@Kategori,StokSayısı=@StokSayısı,İçerik=@İçerik,DolapNo=@DolapNo,RafNo=@RafNo,Sıra=@Sıra,Kredi=@Kredi,KapakResmi=@KapakResmi where KitapID=@KitapID"))
                        {
                            cmd.Connection = sql_cnn;
                            sql_cnn.Open();
                            cmd.Parameters.AddWithValue("@KitapID", KitapID_textBox.Text);
                            cmd.Parameters.AddWithValue("@KitapAdı", KitapAd_textBox.Text);
                            cmd.Parameters.AddWithValue("@YazarAdı", YazarAd_textBox.Text);
                            cmd.Parameters.AddWithValue("@YayınEvi", YayınEvi_textBox.Text);
                            cmd.Parameters.AddWithValue("@Kategori", Kategori_textBox.Text);
                            cmd.Parameters.AddWithValue("@StokSayısı", StokSayısı_textBox.Text);
                            cmd.Parameters.AddWithValue("@İçerik", İçerik_textBox.Text);
                            cmd.Parameters.AddWithValue("@DolapNo", DolapNo_textBox.Text);
                            cmd.Parameters.AddWithValue("@RafNo", RafNo_textBox.Text);
                            cmd.Parameters.AddWithValue("@Sıra", SıraNo_textBox.Text);
                            cmd.Parameters.AddWithValue("@Kredi", Kredi_textBox.Text);
                            cmd.Parameters.AddWithValue("@KapakResmi", KapakResmi_textBox.Text);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("KİTAP BİLGİLERİ GÜNCELLENDİ.");
                            Kitaplistele();
                            Temizlik();
                        }
                    }
                }
                else
                    MessageBox.Show("KİTAP GÜNCELLENİRKEN KİTAP ID DEĞİŞTİRİLEMEZ!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void KitapSil_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (KitapID_textBox.Text == Kitaplar_dataGridView.CurrentRow.Cells[0].Value.ToString())
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlCommand cmd = new SqlCommand("Delete from " + Settings.Default.Kitaplar_Tablo + " where KitapID=@KitapID"))
                        {
                            cmd.Connection = sql_cnn;
                            sql_cnn.Open();
                            cmd.Parameters.AddWithValue("@KitapID", Kitaplar_dataGridView.CurrentRow.Cells["KitapID"].Value.ToString());
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("BİR KİTAP SİLİNDİ");
                            Kitaplistele();
                            Temizlik();
                        }
                    }
                }
                else
                    MessageBox.Show("KİTAP ID UYUŞMUYOR!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void KitapAdıAra_textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (KitapAdıAra_textBox.Text != "")

                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter("select * from " + Settings.Default.Kitaplar_Tablo + " where KitapAdı like '%" + KitapAdıAra_textBox.Text + "%'", sql_cnn))
                        {
                            SqlCommandBuilder build = new SqlCommandBuilder(sda);
                            DataSet DS = new DataSet();
                            sda.Fill(DS, "Kitaplar");
                            Kitaplar_dataGridView.DataSource = DS.Tables["Kitaplar"];
                        }
                    }
                else { Temizlik(); }
            }
            catch { }
        }
        public void Temizlik()
        {
            KitapID_label.Text = string.Empty + "KitapID:";
            KitapAdı_label.Text = string.Empty + "Kitap Adı:";
            YazarAdı_label.Text = string.Empty + "YazarAdı:";
            YayınEvi_label.Text = string.Empty + "Yayın Evi:";
            Kategori_label.Text = string.Empty + "Kategori:";
            İçerik_label.Text = string.Empty + "İçerik:";
            StokSayısı_label.Text = string.Empty + "Stok Sayısı:";
            DolapNo_label.Text = string.Empty + "Dolap No:";
            RafNo_label.Text = string.Empty + "Raf No:";
            SıraNo_label.Text = string.Empty + "Sıra No:";
            Kredi_label.Text = string.Empty + "Kredi";
            pictureBox1.Image = null;
        }

        private void CSV_dosya_sec_buton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV Dosyaları (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8,
                Delimiter = ";"
            };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedCsvFile = openFileDialog1.FileName;

                try
                {
                    System.Data.DataTable myDataTable = new System.Data.DataTable();
                    using (StreamReader sr = new StreamReader(selectedCsvFile))
                    {
                        using (CsvReader csvReader = new CsvReader(sr, configuration))
                        {
                            using (CsvDataReader dataReader = new CsvDataReader(csvReader))
                            {
                                myDataTable.Load(dataReader);
                            }
                        }
                    }
                    Kitaplar_dataGridView.DataSource = myDataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CSV DOSYASINI YÜKLERKEN BİR HATA OLUŞTU: " + ex.Message);
                }
            }
        }

        private void aktar_button_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                {
                    SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Kitaplar_Tablo, sql_cnn);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    System.Data.DataTable myDataTable = new System.Data.DataTable();
                    sda.Fill(myDataTable);
                    string Write_Command = "Insert into " + Settings.Default.Kitaplar_Tablo + "(KitapID,KitapAdı,YazarAdı,YayınEvi,Kategori,StokSayısı,İçerik,DolapNo,RafNo,Sıra,Kredi,KapakResmi) values ((select MAX(KitapID)+1 from dbo.Kitaplar),@KitapAdı,@YazarAdı,@YayınEvi,@Kategori,@StokSayısı,@İçerik,@DolapNo,@RafNo,@Sıra,@Kredi,@KapakResmi)";

                    using (SqlCommand Data_write = new SqlCommand(Write_Command))
                    {
                        Data_write.Connection = sql_cnn;
                        sql_cnn.Open();

                        if (Kitaplar_dataGridView.Rows.Count > 0)
                        {
                            for (int i = 0; i < Kitaplar_dataGridView.Rows.Count; i++)
                            {
                                Data_write.Parameters.Clear();
                                for (int j = 0; j < Kitaplar_dataGridView.Columns.Count; j++)
                                {
                                    if (Kitaplar_dataGridView.Rows[i].Cells[j].Value.ToString() != "" && Kitaplar_dataGridView.Rows[i].Cells[j].Value.ToString() != null)
                                    {
                                        if (myDataTable.Columns[j].DataType == typeof(string))
                                            Data_write.Parameters.AddWithValue("@" + Kitaplar_dataGridView.Columns[j].Name, Convert.ToString(Kitaplar_dataGridView.Rows[i].Cells[j].Value));
                                        else if (myDataTable.Columns[j].DataType == typeof(int))
                                            Data_write.Parameters.AddWithValue("@" + Kitaplar_dataGridView.Columns[j].Name, Convert.ToInt32(Kitaplar_dataGridView.Rows[i].Cells[j].Value));
                                    }
                                    else
                                        Data_write.Parameters.AddWithValue("@" + Kitaplar_dataGridView.Columns[j].Name, DBNull.Value);
                                }
                                Data_write.ExecuteNonQuery();
                            }
                            MessageBox.Show("TÜM KİTAPLAR SQL'E AKTARILDI!");
                        }
                        else
                            MessageBox.Show("ÖNCE CSV DOSYASINI SEÇMELİSİNİZ!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void KitapExcel_button_Click(object sender, EventArgs e)
        {
            int rowCount = Kitaplar_dataGridView.RowCount;
            if (rowCount > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx";
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.CreatePrompt = false;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                        excel.Visible = false;
                        Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
                        Microsoft.Office.Interop.Excel.Worksheet sheet1 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

                        for (int i = 0; i < Kitaplar_dataGridView.Columns.Count; i++)
                        {
                            sheet1.Cells[1, i + 1] = Kitaplar_dataGridView.Columns[i].HeaderText;
                        }

                        for (int i = 0; i < Kitaplar_dataGridView.Rows.Count; i++)
                        {
                            for (int j = 0; j < Kitaplar_dataGridView.Columns.Count; j++)
                            {
                                sheet1.Cells[i + 2, j + 1] = Kitaplar_dataGridView.Rows[i].Cells[j].Value.ToString();
                            }
                        }
                        workbook.SaveAs(saveFileDialog.FileName);
                        excel.Quit();
                    }
                    catch (Exception ex)
                    {
                        {
                            MessageBox.Show("HATA OLUŞTU: " + ex.Message);
                        }
                    }
                }
            }
            else
                MessageBox.Show("LÜTFEN TABLONUN DOLU OLDUĞUNDAN EMİN OLUNUZ!");
        }

        private void Kitaplar_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Kitaplar_dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            KitapID_label.Text = "Kitap ID:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
            KitapAdı_label.Text = "Kitap Adı:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
            YazarAdı_label.Text = "Yazar Adı:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            YayınEvi_label.Text = "Yayın Evi:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[3].Value.ToString();
            Kategori_label.Text = "Kategori:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[4].Value.ToString();
            StokSayısı_label.Text = "Stok Sayısı:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[5].Value.ToString();
            İçerik_label.Text = "İçerik:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[6].Value.ToString();
            DolapNo_label.Text = "Dolap No:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[7].Value.ToString();
            RafNo_label.Text = "Raf No:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[8].Value.ToString();
            SıraNo_label.Text = "Sıra No:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[9].Value.ToString();
            Kredi_label.Text = "Kredi:" + Kitaplar_dataGridView.Rows[e.RowIndex].Cells[10].Value.ToString();
            pictureBox1.ImageLocation = Kitaplar_dataGridView.Rows[e.RowIndex].Cells[11].Value.ToString();
        }

        private void filmkaydet_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (Filmİşlemleri_groupBox != null)
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("insert into " + Settings.Default.Filmler_Tablo + " values ((select MAX(FilmID)+1 from dbo.Filmler),@FilmAdı,@Yönetmen,@Kategori,@İmdbPuanı,@Kredi,@İçerik,@KapakResmi)"))
                        {
                            sqlCommand.Connection = sql_cnn;
                            sql_cnn.Open();
                            sqlCommand.Parameters.AddWithValue("@FilmID", FilmID_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@FilmAdı", FilmAdı_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@Yönetmen", Yönetmen_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@Kategori", FilmKategori_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@İmdbPuanı", İmdb_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@Kredi", FilmKredi_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@İçerik", Filmİçerik_textBox.Text);
                            sqlCommand.Parameters.AddWithValue("@KapakResmi", FilmResim_textBox.Text);
                            sqlCommand.ExecuteNonQuery();
                            CLEAN();
                            MessageBox.Show("YENİ FİLM EKLENDİ.");
                            FilmListele();
                        }
                    }
                }
                else
                    MessageBox.Show("LÜTFEN EKSİK BİLGİLERİ DOLDURUNUZ!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("FİLM KAYDEDİLİRKEN BİR HATA OLUŞTU" + ex.Message);
            }
        }

        private void FilmResimSeç_button_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Resim seç";
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "PNG Files (*.png) | *.png | JPG Files (*.jpg) | *.jpg |JPEG Files (*.jpeg)|*.jpeg|  All Files(*.*) | *.*";
            openFileDialog1.ShowDialog();
            pictureBox2.ImageLocation = openFileDialog1.FileName;
            FilmResim_textBox.Text = openFileDialog1.FileName;
        }

        private void filmlistele_button_Click(object sender, EventArgs e)
        {
            FilmListele();
        }
        public void FilmListele()
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Filmler_Tablo, sql_cnn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    System.Data.DataTable MyDataTable = new System.Data.DataTable();
                    sda.Fill(MyDataTable);
                    if (this.Kitaplar_dataGridView.InvokeRequired)
                        Invoke(new MethodInvoker(() =>
                        {
                            Filmler_dataGridView.DataSource = MyDataTable;

                        }));
                    else
                        Filmler_dataGridView.DataSource = MyDataTable;
                }
            }
        }

        private void Filmler_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FilmID_textBox.Text = Filmler_dataGridView.CurrentRow.Cells[0].Value.ToString();
            FilmAdı_textBox.Text = Filmler_dataGridView.CurrentRow.Cells[1].Value.ToString();
            Yönetmen_textBox.Text = Filmler_dataGridView.CurrentRow.Cells[2].Value.ToString();
            FilmKategori_textBox.Text = Filmler_dataGridView.CurrentRow.Cells[3].Value.ToString();
            double İMDB = Convert.ToDouble(Filmler_dataGridView.CurrentRow.Cells[4].Value.ToString());
            string İMDBpuanı = İMDB.ToString();
            İMDBpuanı = İMDBpuanı.Replace(",", ".");
            İmdb_textBox.Text = İMDBpuanı;
            FilmKredi_textBox.Text = Filmler_dataGridView.CurrentRow.Cells[5].Value.ToString();
            Filmİçerik_textBox.Text = Filmler_dataGridView.CurrentRow.Cells[6].Value.ToString();
            FilmResim_textBox.Text = Filmler_dataGridView.CurrentRow.Cells[7].Value.ToString();

            pictureBox2.ImageLocation = Filmler_dataGridView.CurrentRow.Cells[7].Value.ToString();
        }

        private void filmgüncelle_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (FilmID_textBox.Text == Filmler_dataGridView.CurrentRow.Cells[0].Value.ToString())
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlCommand cmd = new SqlCommand("update " + Settings.Default.Filmler_Tablo + " set FilmAdı=@FilmAdı,Yönetmen=@Yönetmen,Kategori=@Kategori,İmdbPuanı=@İmdbPuanı,Kredi=@Kredi,İçerik=@İçerik,KapakResmi=@KapakResmi where FilmID=@FilmID"))
                        {
                            cmd.Connection = sql_cnn;
                            sql_cnn.Open();
                            cmd.Parameters.AddWithValue("@FilmID", FilmID_textBox.Text);
                            cmd.Parameters.AddWithValue("@FilmAdı", FilmAdı_textBox.Text);
                            cmd.Parameters.AddWithValue("@Yönetmen", Yönetmen_textBox.Text);
                            cmd.Parameters.AddWithValue("@Kategori", FilmKategori_textBox.Text);
                            cmd.Parameters.AddWithValue("@İmdbPuanı", İmdb_textBox.Text);
                            cmd.Parameters.AddWithValue("@Kredi", FilmKredi_textBox.Text);
                            cmd.Parameters.AddWithValue("@İçerik", Filmİçerik_textBox.Text);
                            cmd.Parameters.AddWithValue("@KapakResmi", FilmResim_textBox.Text);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("FİLM BİLGİLERİ GÜNCELLENDİ!");
                            FilmListele();
                            CLEAN();
                        }
                    }
                }
                else
                    MessageBox.Show("Film Güncellenirken Film ID Değiştirilemez!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Filmsil_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (FilmID_textBox.Text == Filmler_dataGridView.CurrentRow.Cells[0].Value.ToString())
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlCommand command = new SqlCommand("Delete from " + Settings.Default.Filmler_Tablo + " where FilmID=@FilmID"))
                        {
                            command.Connection = sql_cnn;
                            sql_cnn.Open();
                            command.Parameters.AddWithValue("@FilmID", Filmler_dataGridView.CurrentRow.Cells["FilmID"].Value.ToString());
                            command.ExecuteNonQuery();
                            MessageBox.Show("Bir Film Silindi");
                            FilmListele();
                            clean();
                            CLEAN();
                        }
                    }
                }
                else
                    MessageBox.Show("FİLM ID UYUŞMUYOR!");
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.ToString());
            }
        }

        private void FilmAdıAra_textBox_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlDataAdapter SDA = new SqlDataAdapter("select * from " + Settings.Default.Filmler_Tablo + " where FilmAdı like '%" + FilmAdıAra_textBox.Text + "%'", sql_cnn))
                {
                    SqlCommandBuilder build = new SqlCommandBuilder(SDA);
                    DataSet DSET = new DataSet();
                    SDA.Fill(DSET, "Filmler");
                    Filmler_dataGridView.DataSource = DSET.Tables["Filmler"];
                }
            }
        }

        private void Filmler_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Filmler_dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            FilmID_label.Text = "FilmID:" + Filmler_dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
            FilmAdı_label.Text = "FilmAdı:" + Filmler_dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
            Yönetmen_label.Text = "Yönetmen:" + Filmler_dataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            Filmkategori_label.Text = "Kategori:" + Filmler_dataGridView.Rows[e.RowIndex].Cells[3].Value.ToString();
            İmdb_label.Text = "İmdb Puanı:" + Filmler_dataGridView.Rows[e.RowIndex].Cells[4].Value.ToString();
            Filmkredi_label.Text = "Kredi:" + Filmler_dataGridView.Rows[e.RowIndex].Cells[5].Value.ToString();
            Filmiçerik_label.Text = "İçerik:" + Filmler_dataGridView.Rows[e.RowIndex].Cells[6].Value.ToString();
            pictureBox2.ImageLocation = Filmler_dataGridView.Rows[e.RowIndex].Cells[7].Value.ToString();
        }
        public void clean()
        {
            FilmID_label.Text = string.Empty + "FilmID:";
            FilmAdı_label.Text = string.Empty + "Film Adı:";
            Yönetmen_label.Text = string.Empty + "Yönetmen:";
            Filmkategori_label.Text = string.Empty + "Kategori:";
            İmdb_label.Text = string.Empty + "İmdb Puanı:";
            Filmiçerik_label.Text = string.Empty + "İçerik:";
            Filmkredi_label.Text = string.Empty + "Kredi";
            pictureBox2.Image = null;
        }
        public void CLEAN()
        {
            foreach (Control item in Filmİşlemleri_groupBox.Controls)
            {
                if (item is System.Windows.Forms.TextBox)
                    item.Text = "";
            }
        }

        private void FilmCSV_button_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV Dosyaları (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8,
                Delimiter = ";"
            };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedCsvFile = openFileDialog1.FileName;
                try
                {
                    System.Data.DataTable MyDataTable = new System.Data.DataTable();
                    using (StreamReader sr = new StreamReader(selectedCsvFile))
                    {
                        using (CsvReader csvReader = new CsvReader(sr, configuration))
                        {
                            using (CsvDataReader dataReader = new CsvDataReader(csvReader))
                            {
                                MyDataTable.Load(dataReader);
                            }
                        }
                    }
                    Filmler_dataGridView.DataSource = MyDataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CSV DOSYASINI YÜKLERKEN BİR HATA OLUŞTU: " + ex.Message);
                }
            }
        }

        private void filmaktar_button_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                {
                    SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Filmler_Tablo, sql_cnn);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    System.Data.DataTable myDataTable = new System.Data.DataTable();
                    sda.Fill(myDataTable);
                    string Write_Cmd = "Insert into " + Settings.Default.Filmler_Tablo + " (FilmID,FilmAdı,Yönetmen,Kategori,İmdbPuanı,Kredi,İçerik,KapakResmi) VALUES ((select MAX(FilmID)+1 from dbo.Filmler),@FilmAdı,@Yönetmen,@Kategori,@İmdbPuanı,@Kredi,@İçerik,@KapakResmi)";

                    using (SqlCommand Data_Write = new SqlCommand(Write_Cmd))
                    {
                        Data_Write.Connection = sql_cnn;
                        sql_cnn.Open();
                        if (Filmler_dataGridView.Rows.Count > 0)
                        {
                            for (int i = 0; i < Filmler_dataGridView.Rows.Count; i++)
                            {
                                Data_Write.Parameters.Clear();
                                for (int j = 0; j < Filmler_dataGridView.Columns.Count; j++)
                                {
                                    if (Filmler_dataGridView.Rows[i].Cells[j].Value.ToString() != "" && Filmler_dataGridView.Rows[i].Cells[j].Value.ToString() != null)
                                    {
                                        if (myDataTable.Columns[j].DataType == typeof(string))
                                            Data_Write.Parameters.AddWithValue("@" + Filmler_dataGridView.Columns[j].Name, Convert.ToString(Filmler_dataGridView.Rows[i].Cells[j].Value));
                                        else if (myDataTable.Columns[j].DataType == typeof(int))
                                            Data_Write.Parameters.AddWithValue("@" + Filmler_dataGridView.Columns[j].Name, Convert.ToInt32(Filmler_dataGridView.Rows[i].Cells[j].Value));
                                        else if (myDataTable.Columns[j].DataType == typeof(double))
                                            Data_Write.Parameters.AddWithValue("@" + Filmler_dataGridView.Columns[j].Name, Convert.ToDecimal(Filmler_dataGridView.Rows[i].Cells[j].Value));
                                    }
                                    else
                                        Data_Write.Parameters.AddWithValue("@" + Filmler_dataGridView.Columns[j].Name, DBNull.Value);
                                }
                                Data_Write.ExecuteNonQuery();
                            }
                            MessageBox.Show("TÜM FİLMLER SQL'E AKTARILDI");
                        }
                        else
                            MessageBox.Show("CSV DOSYASI SEÇMELİSİNİZ");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Filmexcel_button_Click(object sender, EventArgs e)
        {
            int rowCount = Filmler_dataGridView.RowCount;
            if (rowCount > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx";
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.CreatePrompt = false;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                        excel.Visible = false;
                        Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
                        Microsoft.Office.Interop.Excel.Worksheet sheet1 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

                        for (int i = 0; i < Filmler_dataGridView.Columns.Count; i++)
                        {
                            sheet1.Cells[1, i + 1] = Filmler_dataGridView.Columns[i].HeaderText;
                        }

                        for (int i = 0; i < Filmler_dataGridView.Rows.Count; i++)
                        {
                            for (int j = 0; j < Filmler_dataGridView.Columns.Count; j++)
                            {
                                sheet1.Cells[i + 2, j + 1] = Filmler_dataGridView.Rows[i].Cells[j].Value.ToString();
                            }
                        }
                        workbook.SaveAs(saveFileDialog.FileName);
                        excel.Quit();
                    }
                    catch (Exception ex)
                    {
                        {
                            MessageBox.Show("HATA OLUŞTU: " + ex.Message);
                        }
                    }
                }
            }
            else
                MessageBox.Show("LÜTFEN TABLONUN DOLU OLDUĞUNDAN EMİN OLUN!");
        }

        private void SqlAdres_textBox_TextChanged(object sender, EventArgs e)
        {
            if (SqlAdres_textBox.Text != "")
            {
                Settings.Default.Sql_Data_Source = SqlAdres_textBox.Text;
                Settings.Default.Save();
            }
        }

        private void DatabaseName_textBox_TextChanged(object sender, EventArgs e)
        {
            if (DatabaseName_textBox.Text != "")
            {
                Settings.Default.Sql_İnitial_Catalog = DatabaseName_textBox.Text;
                Settings.Default.Save();
            }
        }

        private void KitapTabloİsmi_textBox_TextChanged(object sender, EventArgs e)
        {
            if (KitapTabloİsmi_textBox.Text != "")
            {
                Settings.Default.Kitaplar_Tablo = KitapTabloİsmi_textBox.Text;
                Settings.Default.Save();
            }
        }

        private void FilmTabloİsmi_textBox_TextChanged(object sender, EventArgs e)
        {
            if (FilmTabloİsmi_textBox.Text != "")
            {
                Settings.Default.Filmler_Tablo = FilmTabloİsmi_textBox.Text;
                Settings.Default.Save();
            }
        }

        private void KullanıcılarTabloİsmi_textBox_TextChanged(object sender, EventArgs e)
        {
            if (KullanıcılarTabloİsmi_textBox.Text != "")
            {
                Settings.Default.Kullanıcılar_Tablo = KullanıcılarTabloİsmi_textBox.Text;
                Settings.Default.Save();
            }
        }
        private void KiralananlarTabloismi_textBox_TextChanged(object sender, EventArgs e)
        {
            if (KiralananlarTabloismi_textBox.Text != "")
            {
                Settings.Default.Kiralananlar_Tablo = KiralananlarTabloismi_textBox.Text;
                Settings.Default.Save();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Thread kiralananlar_thread = new Thread(KiralananlarıListele);
            Thread kitap_thread = new Thread(Kitaplistele);
            Thread film_thread = new Thread(FilmListele);
            Thread kullanıcılar_thread = new Thread(Kullanıcılistele);

            kiralananlar_thread.Start();
            kitap_thread.Start();
            film_thread.Start();
            kullanıcılar_thread.Start();
        }
        private void KiralananlarıListele()
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Kiralananlar_Tablo + " WHERE Tür='Kitap'", sql_cnn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    System.Data.DataTable MyDataTable = new System.Data.DataTable();
                    sda.Fill(MyDataTable);

                    Filtrele_comboBox.SelectedIndex = 0;

                    if (this.Kitaplar_dataGridView.InvokeRequired)
                        Invoke(new MethodInvoker(() =>
                        {
                            KiralananKitaplar_dataGridView.DataSource = MyDataTable;
                        }));
                    else
                        KiralananKitaplar_dataGridView.DataSource = MyDataTable;
                }
            }
        }

        private void Filtrele_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime tarih = DateTime.Now;
                switch (Filtrele_comboBox.SelectedIndex)
                {
                    case 0:
                        KiralananlarıListele();
                        break;
                    case 1:
                        using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                        {
                            using (SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Kiralananlar_Tablo + " where İadeTarihi < @tarih", sql_cnn))
                            {
                                cmd.Parameters.AddWithValue("@tarih", tarih);

                                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                                System.Data.DataTable Table = new System.Data.DataTable();
                                sda.Fill(Table);
                                KiralananKitaplar_dataGridView.DataSource = Table;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void KiralananKitaplar_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            KiralananKitaplar_dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ürünıdlabel.Text = "Ürün ID :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
            ürünadılabel.Text = "Ürün  Adı :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
            KREDİ_label.Text = "Kredi :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            Tür_label.Text = "Tür :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[3].Value.ToString();
            teslimtarihi_label.Text = "Teslim Tarihi :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[4].Value.ToString();
            iadetarih_label.Text = "İade Tarihi :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[5].Value.ToString();
            kullanıcıad_label.Text = "Ad :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[6].Value.ToString();
            soyadlabel.Text = "Soyad :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[7].Value.ToString();
            tckimliknolabel.Text = "TC Kimlik No :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[8].Value.ToString();
            epostalabel.Text = "E Posta :" + KiralananKitaplar_dataGridView.Rows[e.RowIndex].Cells[9].Value.ToString();
        }
    }
}


