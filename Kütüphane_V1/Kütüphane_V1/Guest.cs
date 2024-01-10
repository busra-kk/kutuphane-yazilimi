using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Kütüphane_V1
{
    public partial class Guest : Form
    {
        public Guest()
        {
            InitializeComponent();

        }
        private void LOGİN_button_Click(object sender, EventArgs e)
        {
            int KULLANICIID = 0;
            try
            {
                if (GKullanıcıAd_textBox.Text != "" || GŞifre_textBox.Text != "")
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Kullanıcılar_Tablo + " where KullanıcıAd='" + GKullanıcıAd_textBox.Text + "' and Şifre='" + GŞifre_textBox.Text + "'", sql_cnn))
                        {
                            cmd.Connection = sql_cnn;
                            sql_cnn.Open();
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read() == true)
                            {
                                if (GŞifre_textBox.Text == reader["Şifre"].ToString() && GKullanıcıAd_textBox.Text == reader["KullanıcıAd"].ToString())
                                {
                                    GuestLogin_panel.Visible = false;
                                    KULLANICIID = int.Parse(reader["KullanıcıID"].ToString());
                                    kullanıcıadtextBox.Text = reader["KullanıcıAd"].ToString();
                                    sifre_textBox.Text = reader["Şifre"].ToString();
                                    kullanıcııdlabel.Text = "Kullanıcı ID:" + reader["KullanıcıID"].ToString();
                                    ad_textBox.Text = reader["Ad"].ToString();
                                    soyad_textBox.Text = reader["Soyad"].ToString();
                                    tc_maskedTextBox.Text = reader["TCKimlikNo"].ToString();
                                    doğumt_maskedTextBox.Text = reader["DoğumTarihi"].ToString();
                                    telefon_maskedTextBox.Text = reader["Telefon"].ToString();
                                    geposta_textBox.Text = reader["EPosta"].ToString();
                                    üyelik_maskedTextBox.Text = reader["ÜyelikTarihi"].ToString();
                                    adres_textBox.Text = reader["Adres"].ToString();
                                    Kullanıcıkredi_label.Text = "Kredi:" + reader["Kredi"].ToString();
                                    Kullanıcıkredisi_label.Text = "Kredi :" + reader["Kredi"].ToString();
                                    Thread kiraladıklarım_thread = new Thread(kiraladıklarım);
                                    kiraladıklarım_thread.Start();
                                }
                                else
                                    MessageBox.Show("Bilgilerinizi kontrol ediniz.", "Hatalı giriş!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                                MessageBox.Show("Böyle bir kullanıcı bulunamadı");
                        }
                    }
                }
                else
                    MessageBox.Show("Lütfen kullanıcı ad ve şifrenizi giriniz ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                GKullanıcıAd_textBox.Clear();
                GŞifre_textBox.Clear();
                GKullanıcıAd_textBox.Focus();
            }

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            GŞifre_textBox.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }
        private void Kayıt_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (EPosta_textBox.Text == "" || YŞifre_textBox.Text == "" || YŞifreTekrar_textBox.Text == "")
                    MessageBox.Show("Lütfen Boş Alanları Doldurunuz");
                else
                {
                    string eposta = EPosta_textBox.Text;
                    if (isvalidemail(eposta))
                    {
                        if (YŞifre_textBox.Text == YŞifreTekrar_textBox.Text)
                        {
                            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                            {
                                using (SqlCommand sqlCommand = new SqlCommand("insert into " + Settings.Default.Kullanıcılar_Tablo + " (KullanıcıAd,Şifre) values ('" + EPosta_textBox.Text + "','" + YŞifre_textBox.Text + "')", sql_cnn))
                                {
                                    sqlCommand.Connection = sql_cnn;
                                    sql_cnn.Open();
                                    sqlCommand.ExecuteNonQuery();
                                    MessageBox.Show("Kaydınız başarı ile oluşturulmuştur.EPostanız ile giriş yapabilirsiniz");
                                }
                            }
                            EPosta_textBox.Text = "";
                            YŞifre_textBox.Text = "";
                            YŞifreTekrar_textBox.Text = "";
                        }
                        else
                            MessageBox.Show("Şifrelerin aynı olduğundan emin olunuz");
                    }
                    else
                        MessageBox.Show("Lütfen geçerli bir e posta adresi giriniz");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("!!!!" + ex.Message);
            }
        }

        private bool isvalidemail(string eposta)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(eposta);
                return mailAddress.Address == eposta;
            }
            catch (FormatException)
            {
                return false;
            }
        }


        private void Kitapadıara_textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Kitapadıara_textBox.Text != "")
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter("select * from " + Settings.Default.Kitaplar_Tablo + " where KitapAdı like '%" + Kitapadıara_textBox.Text + "%'", sql_cnn))
                        {
                            SqlCommandBuilder build = new SqlCommandBuilder(sda);
                            DataSet DS = new DataSet();
                            sda.Fill(DS, "Kitaplar");
                            KitapListesi_dataGridView.DataSource = DS.Tables["Kitaplar"];
                        }
                    }
                }
                else { clean(); }
            }
            catch { }
        }

        private void kategoriara_textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (kategoriara_textBox.Text != "")
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter("select * from " + Settings.Default.Kitaplar_Tablo + " where Kategori like '%" + kategoriara_textBox.Text + "%'", sql_cnn))
                        {
                            SqlCommandBuilder build = new SqlCommandBuilder(sda);
                            DataSet DS = new DataSet();
                            sda.Fill(DS, "Kitaplar");
                            KitapListesi_dataGridView.DataSource = DS.Tables["Kitaplar"];
                        }
                    }
                }
                else { clean(); }
            }
            catch { }
        }

        private void yazaradıara_textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (yazaradıara_textBox.Text != "")
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter("select * from " + Settings.Default.Kitaplar_Tablo + " where YazarAdı like '%" + yazaradıara_textBox.Text + "%'", sql_cnn))
                        {
                            SqlCommandBuilder build = new SqlCommandBuilder(sda);
                            DataSet DS = new DataSet();
                            sda.Fill(DS, "Kitaplar");
                            KitapListesi_dataGridView.DataSource = DS.Tables["Kitaplar"];
                        }
                    }
                }
                else { clean(); }
            }
            catch { }

        }
        public void clean()
        {
            kitapıd_label.Text = string.Empty + "KitapID:";
            Kitapadı_label.Text = string.Empty + "Kitap Adı:";
            Yazaradı_label.Text = string.Empty + "YazarAdı:";
            yayınevi_label.Text = string.Empty + "Yayın Evi:";
            kategori_label.Text = string.Empty + "Kategori:";
            içerik_label.Text = string.Empty + "İçerik:";
            stoksayısı_label.Text = string.Empty + "Stok Sayısı:";
            dolapno_label.Text = string.Empty + "Dolap No:";
            rafno_label.Text = string.Empty + "Raf No:";
            sırano_label.Text = string.Empty + "Sıra No:";
            kredi_label.Text = string.Empty + "Kredi";
            pictureBox1.Image = null;
        }

        private void Guest_Load(object sender, EventArgs e)
        {
            Thread Kitap_thread = new Thread(KitapListesi);
            Thread Film_thread = new Thread(FilmListesi);
            Thread sepet_thread = new Thread(sepetgüncelle);
            Kitap_thread.Start();
            Film_thread.Start();
            sepet_thread.Start();
        }
        private void KitapListesi()
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Kitaplar_Tablo, sql_cnn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    System.Data.DataTable myDataTable = new System.Data.DataTable();
                    sda.Fill(myDataTable);
                    if (this.KitapListesi_dataGridView.InvokeRequired)
                        Invoke(new MethodInvoker(() =>
                        {
                            KitapListesi_dataGridView.DataSource = myDataTable;
                        }));
                    else
                        KitapListesi_dataGridView.DataSource = myDataTable;
                }
            }
        }
        private void FilmListesi()
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from " + Settings.Default.Filmler_Tablo, sql_cnn))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    System.Data.DataTable myDataTable = new System.Data.DataTable();
                    sda.Fill(myDataTable);
                    if (this.FilmListesi_dataGridView.InvokeRequired)
                        Invoke(new MethodInvoker(() =>
                        {
                            FilmListesi_dataGridView.DataSource = myDataTable;
                        }));
                    else
                        FilmListesi_dataGridView.DataSource = myDataTable;
                }
            }
        }

        private void KitapListesi_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            KitapListesi_dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            kitapıd_label.Text = "KitapID:" + KitapListesi_dataGridView.CurrentRow.Cells[0].Value.ToString();
            Kitapadı_label.Text = "Kitap Adı:" + KitapListesi_dataGridView.CurrentRow.Cells[1].Value.ToString();
            Yazaradı_label.Text = "Yazar Adı:" + KitapListesi_dataGridView.CurrentRow.Cells[2].Value.ToString();
            yayınevi_label.Text = "Yayın Evi:" + KitapListesi_dataGridView.CurrentRow.Cells[3].Value.ToString();
            kategori_label.Text = "Kategori:" + KitapListesi_dataGridView.CurrentRow.Cells[4].Value.ToString();
            stoksayısı_label.Text = "Stok Sayısı:" + KitapListesi_dataGridView.CurrentRow.Cells[5].Value.ToString();
            içerik_label.Text = "İçerik:" + KitapListesi_dataGridView.CurrentRow.Cells[6].Value.ToString();
            dolapno_label.Text = "Dolap No:" + KitapListesi_dataGridView.CurrentRow.Cells[7].Value.ToString();
            rafno_label.Text = "Raf No:" + KitapListesi_dataGridView.CurrentRow.Cells[8].Value.ToString();
            sırano_label.Text = "Sıra No:" + KitapListesi_dataGridView.CurrentRow.Cells[9].Value.ToString();
            kredi_label.Text = "Kredi:" + KitapListesi_dataGridView.CurrentRow.Cells[10].Value.ToString();

            pictureBox1.ImageLocation = KitapListesi_dataGridView.CurrentRow.Cells[11].Value.ToString();
        }

        private void Seçilenleriekle_button_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = KitapListesi_dataGridView.SelectedRows;
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir veya daha fazla satır seçin.");
                return;
            }
            DataTable selectedDataTable;
            if (sepet_dataGridView.DataSource == null)
            {
                selectedDataTable = new DataTable();
                selectedDataTable.Columns.Add("ÜrünID");
                selectedDataTable.Columns.Add("ÜrünAdı");
                selectedDataTable.Columns.Add("Kredi");
                selectedDataTable.Columns.Add("Tür");
            }
            else
            {
                selectedDataTable = (DataTable)sepet_dataGridView.DataSource;
            }
            foreach (DataGridViewRow row in selectedRows)
            {
                DataRow selectedRow = selectedDataTable.NewRow();
                selectedRow["ÜrünID"] = row.Cells["KitapID"].Value.ToString();
                selectedRow["ÜrünAdı"] = row.Cells["KitapAdı"].Value.ToString();
                selectedRow["Kredi"] = row.Cells["Kredi"].Value.ToString();
                selectedRow["Tür"] = "Kitap";

                selectedDataTable.Rows.Add(selectedRow);
            }
            sepet_dataGridView.DataSource = selectedDataTable;
            MessageBox.Show("Kitap(lar) sepete eklendi");
            sepetgüncelle();
            int KALANKREDİ = krediPuanı - toplamKrediPuanı;
            kalankredi_label.Text = KALANKREDİ.ToString();
        }

        private void filmadıara_textBox_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("select * from " + Settings.Default.Filmler_Tablo + " where FilmAdı like '%" + filmadıara_textBox.Text + "%'", sql_cnn))
                {
                    SqlCommandBuilder build = new SqlCommandBuilder(sda);
                    DataSet DS = new DataSet();
                    sda.Fill(DS, "Filmler");
                    FilmListesi_dataGridView.DataSource = DS.Tables["Filmler"];
                }
            }
        }

        private void filmkategoriara_textBox_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("select * from " + Settings.Default.Filmler_Tablo + " where Kategori like '%" + filmkategoriara_textBox.Text + "%'", sql_cnn))
                {
                    SqlCommandBuilder build = new SqlCommandBuilder(sda);
                    DataSet DS = new DataSet();
                    sda.Fill(DS, "Filmler");
                    FilmListesi_dataGridView.DataSource = DS.Tables["Filmler"];
                }
            }
        }

        private void yönetmenara_textBox_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("select * from " + Settings.Default.Filmler_Tablo + " where Yönetmen like '%" + yönetmenara_textBox.Text + "%'", sql_cnn))
                {
                    SqlCommandBuilder build = new SqlCommandBuilder(sda);
                    DataSet DS = new DataSet();
                    sda.Fill(DS, "Filmler");
                    FilmListesi_dataGridView.DataSource = DS.Tables["Filmler"];
                }
            }
        }

        private void FilmListesi_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FilmListesi_dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            filmıd_label.Text = "Film ID:" + FilmListesi_dataGridView.CurrentRow.Cells[0].Value.ToString();
            filmadı_label.Text = "Film Adı:" + FilmListesi_dataGridView.CurrentRow.Cells[1].Value.ToString();
            yönetmen_label.Text = "Yönetmen:" + FilmListesi_dataGridView.CurrentRow.Cells[2].Value.ToString();
            filmkategori_label.Text = "Kategori:" + FilmListesi_dataGridView.CurrentRow.Cells[3].Value.ToString();
            imdbpuanı_label.Text = "İmdb Puanı:" + FilmListesi_dataGridView.CurrentRow.Cells[4].Value.ToString();
            filmkredi_label.Text = "Kredi:" + FilmListesi_dataGridView.CurrentRow.Cells[5].Value.ToString();
            filmiçerik_label.Text = "İçerik:" + FilmListesi_dataGridView.CurrentRow.Cells[6].Value.ToString();

            pictureBox2.ImageLocation = FilmListesi_dataGridView.CurrentRow.Cells[7].Value.ToString();
        }

        private void seçilenekle_button_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = FilmListesi_dataGridView.SelectedRows;
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir veya daha fazla satır seçin.");
                return;
            }
            DataTable selectedDataTable;
            if (sepet_dataGridView.DataSource == null)
            {
                selectedDataTable = new DataTable();
                selectedDataTable.Columns.Add("ÜrünID");
                selectedDataTable.Columns.Add("ÜrünAdı");
                selectedDataTable.Columns.Add("Kredi");
                selectedDataTable.Columns.Add("Tür");
            }
            else
            {
                selectedDataTable = (DataTable)sepet_dataGridView.DataSource;
            }
            foreach (DataGridViewRow row in selectedRows)
            {
                DataRow selectedRow = selectedDataTable.NewRow();
                selectedRow["ÜrünID"] = row.Cells["FilmID"].Value.ToString();
                selectedRow["ÜrünAdı"] = row.Cells["FilmAdı"].Value.ToString();
                selectedRow["Kredi"] = row.Cells["Kredi"].Value.ToString();
                selectedRow["Tür"] = "Film";


                selectedDataTable.Rows.Add(selectedRow);
            }

            sepet_dataGridView.DataSource = selectedDataTable;
            MessageBox.Show("Film(ler) sepete eklendi");
            sepetgüncelle();
        }
        private void sepetgüncelle()
        {
            int toplamKrediPuanı = 0;
            int urunsayısı = sepet_dataGridView.Rows.Count;

            for (int i = 0; i < sepet_dataGridView.Rows.Count; i++)
            {
                toplamKrediPuanı += Convert.ToInt32(sepet_dataGridView.Rows[i].Cells["Kredi"].Value);
            }
            sepettekitoplamkredi_label.Text = "Sepetteki Toplam Kredi :" + toplamKrediPuanı.ToString();
            sepettekiürünsayısı_label.Text = "Sepetteki Ürün Sayısı :" + urunsayısı.ToString();
        }
        private void sil_button_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in sepet_dataGridView.SelectedRows)
            {
                try
                {
                    if (!item.IsNewRow)
                        sepet_dataGridView.Rows.Remove(item);
                    sepetgüncelle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme işlemi sırasında bir hata oluştu: " + ex.Message);
                }
            }
        }
        private void tümünüsil_button_Click(object sender, EventArgs e)
        {
            try
            {
                sepet_dataGridView.DataSource = null;

                if (sepet_dataGridView.SelectedRows.Count > 0)
                {
                    sepet_dataGridView.Rows.Clear();
                }
                sepetgüncelle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme işlemi sırasında bir hata oluştu: " + ex.Message);
            }

        }

        private void KAYDET_button_Click(object sender, EventArgs e)
        {
            using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
            {
                sql_cnn.Open();
                if (kullanıcıadtextBox.Text != "" && sifre_textBox.Text != "")
                {
                    using (SqlCommand cmd = new SqlCommand("update " + Settings.Default.Kullanıcılar_Tablo + " set Ad=@Ad,Soyad=@Soyad,Telefon=@Telefon,TCKimlikNo=@TCKimlikNo,EPosta=@EPosta,Adres=@Adres,DoğumTarihi=@DoğumTarihi,ÜyelikTarihi=@ÜyelikTarihi,KullanıcıAd=@KullanıcıAd,Şifre=@Şifre where KullanıcıID=@KullanıcıID", sql_cnn))
                    {
                        int kullaniciID;
                        string kullaniciIDText = kullanıcııdlabel.Text.Replace("Kullanıcı ID:", "").Trim();
                        if (int.TryParse(kullaniciIDText, out kullaniciID))
                        {
                            cmd.Parameters.AddWithValue("@KullanıcıID", kullaniciID);
                        }
                        else
                        {
                            MessageBox.Show("Geçersiz Kullanıcı ID formatı");
                            return;
                        }
                        cmd.Parameters.AddWithValue("@Ad", ad_textBox.Text);
                        cmd.Parameters.AddWithValue("@Soyad", soyad_textBox.Text);
                        cmd.Parameters.AddWithValue("@Telefon", telefon_maskedTextBox.Text);
                        cmd.Parameters.AddWithValue("@TCKimlikNo", tc_maskedTextBox.Text);
                        cmd.Parameters.AddWithValue("@Eposta", geposta_textBox.Text);
                        cmd.Parameters.AddWithValue("@Adres", adres_textBox.Text);
                        cmd.Parameters.AddWithValue("@DoğumTarihi", doğumt_maskedTextBox.Text);
                        cmd.Parameters.AddWithValue("@ÜyelikTarihi", üyelik_maskedTextBox.Text);
                        cmd.Parameters.AddWithValue("@KullanıcıAd", kullanıcıadtextBox.Text);
                        cmd.Parameters.AddWithValue("Şifre", sifre_textBox.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("KULLANICI BİLGİLERİ KAYDEDİLDİ");
                    }
                }
                else
                    MessageBox.Show("KULLANICI AD VE ŞİFRE KISMI BOŞ GEÇİLEMEZ!");
            }
        }

        private void Kirala_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (ad_textBox.Text != "" && soyad_textBox.Text != "" && tc_maskedTextBox.Text != "" && geposta_textBox.Text != "")
                {
                    int krediPuanı;
                    if (int.TryParse(Kullanıcıkredi_label.Text.Replace("Kredi:", null), out krediPuanı))
                    {
                        int toplamKrediPuanı = 0;

                        for (int i = 0; i < sepet_dataGridView.Rows.Count - 1; i++)
                        {
                            toplamKrediPuanı += Convert.ToInt32(sepet_dataGridView.Rows[i].Cells["Kredi"].Value);
                        }

                        int KULLANICIID;
                        if (int.TryParse(kullanıcııdlabel.Text.Replace("Kullanıcı ID:", null), out KULLANICIID))
                        {
                            if (krediPuanı >= toplamKrediPuanı)
                            {
                                using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                                {
                                    using (SqlCommand cmd = new SqlCommand("insert into " + Settings.Default.Kiralananlar_Tablo + " (ÜrünID,ÜrünAdı,Kredi,Tür,TeslimTarihi,İadeTarihi,Ad,Soyad,TCKimlikNo,Eposta) values (@ÜrünID,@ÜrünAdı,@Kredi,@Tür,@TeslimTarihi,@İadeTarihi,@Ad,@Soyad,@TCKimlikNo,@Eposta)"))
                                    {
                                        cmd.Connection = sql_cnn;
                                        sql_cnn.Open();
                                        for (int i = 0; i < sepet_dataGridView.Rows.Count - 1; i++)
                                        {
                                            cmd.Parameters.Clear();
                                            cmd.Parameters.AddWithValue("@ÜrünID", sepet_dataGridView.Rows[i].Cells["ÜrünID"].Value.ToString());
                                            cmd.Parameters.AddWithValue("@ÜrünAdı", sepet_dataGridView.Rows[i].Cells["ÜrünAdı"].Value.ToString());
                                            cmd.Parameters.AddWithValue("@Kredi", sepet_dataGridView.Rows[i].Cells["Kredi"].Value.ToString());
                                            cmd.Parameters.AddWithValue("@Tür", sepet_dataGridView.Rows[i].Cells["Tür"].Value.ToString());
                                            cmd.Parameters.AddWithValue("@TeslimTarihi", DateTime.Now);
                                            cmd.Parameters.AddWithValue("@İadeTarihi", DateTime.Now.AddDays(10));
                                            cmd.Parameters.AddWithValue("Ad", ad_textBox.Text);
                                            cmd.Parameters.AddWithValue("Soyad", soyad_textBox.Text);
                                            cmd.Parameters.AddWithValue("TCKimlikNo", tc_maskedTextBox.Text);
                                            cmd.Parameters.AddWithValue("EPosta", geposta_textBox.Text);

                                            cmd.ExecuteNonQuery();
                                            try
                                            {
                                                string KitapAdı = sepet_dataGridView.Rows[i].Cells["ÜrünAdı"].Value.ToString();
                                                int ürünsayısı = sepet_dataGridView.Rows.Count - 1;
                                                using (SqlCommand cmd2 = new SqlCommand("update Kitaplar set StokSayısı=StokSayısı - '" + ürünsayısı + "' where KitapAdı='" + KitapAdı + "'", sql_cnn))
                                                {
                                                    cmd2.ExecuteNonQuery();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.Message);
                                            }
                                            try
                                            {
                                                using (SqlCommand cmd3 = new SqlCommand("update Kullanıcılar set Kredi=Kredi - '" + toplamKrediPuanı + "' where KullanıcıID='" + KULLANICIID + "'", sql_cnn))
                                                {
                                                    cmd3.ExecuteNonQuery();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.Message);
                                            }

                                        }
                                        MessageBox.Show("KİRALAMA İŞLEMİ BAŞARILI");
                                        sepet_dataGridView.DataSource = null;
                                    }
                                }
                            }
                            else
                                MessageBox.Show("KULLANICI KREDİSİ YETERSİZ!");
                        }
                        else
                            MessageBox.Show("KULLANICI ID PARSE HATASI!");
                    }
                    else
                        MessageBox.Show("KULLANICI KREDİSİ PARSE HATASI!");
                }
                else
                    MessageBox.Show("KULLANICI BİLGİLERİ GİRİLMELİ!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("KİRALAMA İŞLEMİ SIRASINDA BİR HATA OLUŞTU: " + ex.Message);
            }

        }
        private void kiraladıklarım()
        {
            try
            {
                if (geposta_textBox.Text != "")
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        using (SqlCommand cmd = new SqlCommand("Select ÜrünID,ÜrünAdı,TeslimTarihi,İadeTarihi from " + Settings.Default.Kiralananlar_Tablo + " where Eposta='" + geposta_textBox.Text + "' and Tür = 'Kitap'", sql_cnn))
                        {
                            SqlDataAdapter sda = new SqlDataAdapter(cmd);
                            System.Data.DataTable MYDATATABLE = new System.Data.DataTable();
                            sda.Fill(MYDATATABLE);

                            if (this.Kiraladıklarım_dataGridView.InvokeRequired)
                                Invoke(new MethodInvoker(() =>
                                {
                                    Kiraladıklarım_dataGridView.DataSource = MYDATATABLE;
                                }));
                            else
                                Kiraladıklarım_dataGridView.DataSource = MYDATATABLE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        (DataTable, string) kiraladıklarım_new(int kullanıcı_id)
        {
            DataTable dataTable = new DataTable();
            string Error = "";
            try
            {
                if (geposta_textBox.Text != "")
                {
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        SqlCommand cmd = new SqlCommand("select ÜrünID,ÜrünAdı,TeslimTarihi,İadeTarihi from " + Settings.Default.Kiralananlar_Tablo + " where KullanıcıID='" + kullanıcı_id.ToString() + "' and Tür = 'Kitap'", sql_cnn);
                        using (SqlDataReader sqlDataReader = cmd.ExecuteReader())
                        {
                            dataTable.Load(sqlDataReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error = "Hata oluştu: " + ex.Message;
            }
            return (dataTable, Error);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 3)
            {
                kiraladıklarım();
                string error;
                //  (Kiraladıklarım_dataGridView.DataSource,error)= kiraladıklarım_new(1);
            }
        }

        private void Teslimet_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (Kiraladıklarım_dataGridView.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = Kiraladıklarım_dataGridView.SelectedRows[0];
                    int KitapID = Convert.ToInt32(selectedRow.Cells["ÜrünID"].Value);
                    using (SqlConnection sql_cnn = new SqlConnection(@"Data Source=" + Settings.Default.Sql_Data_Source + ";Initial Catalog=" + Settings.Default.Sql_İnitial_Catalog + ";Integrated Security=True"))
                    {
                        sql_cnn.Open();
                        using (SqlCommand cmd = new SqlCommand("update " + Settings.Default.Kitaplar_Tablo + " set StokSayısı=StokSayısı + 1 where KitapID='" + KitapID + "'", sql_cnn))
                        {
                            cmd.Connection = sql_cnn;
                            cmd.Parameters.AddWithValue("@UrunID", KitapID);
                            cmd.ExecuteNonQuery();
                        }
                        using (SqlCommand CMD2 = new SqlCommand("delete from " + Settings.Default.Kiralananlar_Tablo + " where ÜrünID=@UrunID"))
                        {
                            CMD2.Connection = sql_cnn;
                            CMD2.Parameters.AddWithValue("@UrunID", KitapID);
                            CMD2.ExecuteNonQuery();
                        }

                    }
                    MessageBox.Show("KİTAP TESLİM EDİLDİ ");
                    kiraladıklarım();
                }
                else
                    MessageBox.Show("LÜTFEN TESLİM ETMEK İSTEDİĞİNİZ BİR KİTAP SEÇİN.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

    }
}
