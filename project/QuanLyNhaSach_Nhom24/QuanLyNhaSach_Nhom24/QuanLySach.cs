using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlTypes;

namespace QuanLyNhaSach_Nhom24
{
    public partial class QuanLySach : Form
    {
        private readonly string connectionString = "Data Source=LAPTOP-Q12JULH6\\KHANHKHIEMTON;Initial Catalog=dbQUANLYNHASACH;Integrated Security=True";
        private readonly string xmlFilePath = Path.Combine(Application.StartupPath, "SACH.xml");
        private readonly string theLoaiFilePath = Path.Combine(Application.StartupPath, "THELOAI.xml");

        private void ExportSachToXml()
        {
            try
            {
                string query = "SELECT * FROM SACH";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    XElement sachXml = new XElement("SACHES",
                        dataTable.AsEnumerable().Select(row =>
                            new XElement("SACH",
                                row.Table.Columns.Cast<DataColumn>().Select(col =>
                                    new XElement(col.ColumnName, row[col])
                                )
                            )
                        )
                    );

                    sachXml.Save(xmlFilePath);
                }

                //MessageBox.Show("Xuất dữ liệu sang SACH.xml thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất dữ liệu sang XML: " + ex.Message);
            }
        }

        public QuanLySach()
        {
            InitializeComponent();
            LoadIDTheLoai();
            LoadSachData();
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn đăng xuất không?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {

                this.Close();
                DangNhap changetoform = new DangNhap();
                changetoform.Show();
            }
        }
        private void LoadIDTheLoai()
        {
            if (File.Exists(theLoaiFilePath))
            {
                XElement theLoaiXml = XElement.Load(theLoaiFilePath);
                var ids = theLoaiXml.Elements("Category").Select(x => x.Element("IDTheLoai")?.Value);
                foreach (var id in ids)
                {
                    cbIDTheLoai.Items.Add(id);
                }
            }
            else
            {
                MessageBox.Show("File XML 'THELOAI.xml' không tồn tại");
            }
        }

        private void LoadSachData()
        {
            //ExportSachToXml();
            //if (File.Exists(xmlFilePath))
            //{
            //    XElement sachXml = XElement.Load(xmlFilePath);
            //    var dataTable = new DataTable();
            //    dataTable.Columns.AddRange(new[]
            //    {
            //        new DataColumn("IDSach"), new DataColumn("TenSach"), new DataColumn("TacGia"),
            //        new DataColumn("IDTheLoai"), new DataColumn("NhaXuatBan"), new DataColumn("NamXuatBan"),
            //        new DataColumn("GiaNhap"), new DataColumn("GiaBan"), new DataColumn("SoLuongTon")
            //    });

            //    foreach (var sach in sachXml.Elements("SACH"))
            //    {
            //        dataTable.Rows.Add(
            //            sach.Element("IDSach")?.Value,
            //            sach.Element("TenSach")?.Value,
            //            sach.Element("TacGia")?.Value,
            //            sach.Element("IDTheLoai")?.Value,
            //            sach.Element("NhaXuatBan")?.Value,
            //            sach.Element("NamXuatBan")?.Value,
            //            sach.Element("GiaNhap")?.Value,
            //            sach.Element("GiaBan")?.Value,
            //            sach.Element("SoLuongTon")?.Value
            //        );
            //    }

            //    dataGridViewSach.DataSource = dataTable;
            //}
            //else
            //{
            //    MessageBox.Show("File XML 'SACH.xml' không tồn tại");
            //}


            // Xóa dữ liệu cũ trong dataGridViewSach
            dataGridViewSach.Rows.Clear();

            // Kiểm tra và thêm cột nếu chưa có
            if (dataGridViewSach.Columns.Count == 0)
            {
                dataGridViewSach.Columns.Add("IDSach", "ID Sách");
                dataGridViewSach.Columns.Add("TenSach", "Tên Sách");
                dataGridViewSach.Columns.Add("TacGia", "Tác Giả");
                dataGridViewSach.Columns.Add("IDTheLoai", "ID Thể Loại");
                dataGridViewSach.Columns.Add("NhaXuatBan", "Nhà Xuất Bản");
                dataGridViewSach.Columns.Add("NamXuatBan", "Năm Xuất Bản");
                dataGridViewSach.Columns.Add("GiaNhap", "Giá Nhập");
                dataGridViewSach.Columns.Add("GiaBan", "Giá Bán");
                dataGridViewSach.Columns.Add("SoLuongTon", "Số Lượng Tồn");
            }

            // Tải dữ liệu từ file XML
            XElement sachXml = XElement.Load(xmlFilePath);

            // Thêm từng sách vào dataGridViewSach
            foreach (XElement sach in sachXml.Elements("SACH"))
            {
                int rowIndex = dataGridViewSach.Rows.Add();
                dataGridViewSach.Rows[rowIndex].Cells["IDSach"].Value = sach.Element("IDSach")?.Value;
                dataGridViewSach.Rows[rowIndex].Cells["TenSach"].Value = sach.Element("TenSach")?.Value;
                dataGridViewSach.Rows[rowIndex].Cells["TacGia"].Value = sach.Element("TacGia")?.Value;
                dataGridViewSach.Rows[rowIndex].Cells["IDTheLoai"].Value = sach.Element("IDTheLoai")?.Value;
                dataGridViewSach.Rows[rowIndex].Cells["NhaXuatBan"].Value = sach.Element("NhaXuatBan")?.Value;
                dataGridViewSach.Rows[rowIndex].Cells["NamXuatBan"].Value = sach.Element("NamXuatBan")?.Value;
                dataGridViewSach.Rows[rowIndex].Cells["GiaNhap"].Value = sach.Element("GiaNhap")?.Value;
                dataGridViewSach.Rows[rowIndex].Cells["GiaBan"].Value = sach.Element("GiaBan")?.Value;
                dataGridViewSach.Rows[rowIndex].Cells["SoLuongTon"].Value = sach.Element("SoLuongTon")?.Value;
            }
        }






        //=============================================================Sự kiện click======================================================================//

        private void quảnLýSáchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            QuanLySach changetoform = new QuanLySach();
            changetoform.Show();
        }

        private void QuanLySach_Load(object sender, EventArgs e)
        {
            label2.Parent = pictureBox1;
            label2.BackColor = Color.Transparent;
        }

        private void cbIDTheLoai_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement sachXml = XElement.Load(xmlFilePath);

                // Kiểm tra xem IDSach đã tồn tại chưa
                bool exists = sachXml.Elements("SACH").Any(x => x.Element("IDSach")?.Value == tbIDSach.Text);
                if (exists)
                {
                    MessageBox.Show("Mã sách đã tồn tại, vui lòng nhập mã khác.");
                    return;
                }

                XElement newBook = new XElement("SACH",
                    new XElement("IDSach", tbIDSach.Text),
                    new XElement("TenSach", tbTenSach.Text),
                    new XElement("TacGia", tbTacGia.Text),
                    new XElement("IDTheLoai", cbIDTheLoai.SelectedItem?.ToString()),
                    new XElement("NhaXuatBan", tbNhaXuatBan.Text),
                    new XElement("NamXuatBan", tbNamXuatBan.Text),
                    new XElement("GiaNhap", tbGiaNhap.Text),
                    new XElement("GiaBan", tbGiaBan.Text),
                    new XElement("SoLuongTon", tbSoLuongTon.Text)
                );

                sachXml.Add(newBook);
                sachXml.Save(xmlFilePath);

                MessageBox.Show("Thêm sách thành công!");
                LoadSachData();
            }
            else
            {
                MessageBox.Show("File XML 'SACH.xml' không tồn tại");
            }

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement sachXml = XElement.Load(xmlFilePath);

                XElement book = sachXml.Elements("SACH").FirstOrDefault(x => x.Element("IDSach")?.Value == tbIDSach.Text);
                if (book != null)
                {
                    book.Element("TenSach")?.SetValue(tbTenSach.Text);
                    book.Element("TacGia")?.SetValue(tbTacGia.Text);
                    book.Element("IDTheLoai")?.SetValue(cbIDTheLoai.SelectedItem?.ToString());
                    book.Element("NhaXuatBan")?.SetValue(tbNhaXuatBan.Text);
                    book.Element("NamXuatBan")?.SetValue(tbNamXuatBan.Text);
                    book.Element("GiaNhap")?.SetValue(tbGiaNhap.Text);
                    book.Element("GiaBan")?.SetValue(tbGiaBan.Text);
                    book.Element("SoLuongTon")?.SetValue(tbSoLuongTon.Text);

                    sachXml.Save(xmlFilePath);

                    MessageBox.Show("Cập nhật sách thành công!");
                    LoadSachData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sách với mã sách đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'SACH.xml' không tồn tại");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement sachXml = XElement.Load(xmlFilePath);

                XElement book = sachXml.Elements("SACH").FirstOrDefault(x => x.Element("IDSach")?.Value == tbIDSach.Text);
                if (book != null)
                {
                    book.Remove();
                    sachXml.Save(xmlFilePath);

                    MessageBox.Show("Xóa sách thành công!");
                    LoadSachData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sách với mã sách đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'SACH.xml' không tồn tại");
            }
        }


       



        private void dataGridViewSach_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewSach.Rows[e.RowIndex];


                tbIDSach.Text = row.Cells["IDSach"].Value.ToString();
                tbTenSach.Text = row.Cells["TenSach"].Value.ToString();
                tbTacGia.Text = row.Cells["TacGia"].Value.ToString();
                cbIDTheLoai.SelectedItem = row.Cells["IDTheLoai"].Value.ToString();
                tbNhaXuatBan.Text = row.Cells["NhaXuatBan"].Value.ToString();
                tbNamXuatBan.Text = row.Cells["NamXuatBan"].Value.ToString();
                tbGiaNhap.Text = row.Cells["GiaNhap"].Value.ToString();
                tbGiaBan.Text = row.Cells["GiaBan"].Value.ToString();
                tbSoLuongTon.Text = row.Cells["SoLuongTon"].Value.ToString();
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            tbIDSach.Clear();
            tbTenSach.Clear();
            tbTacGia.Clear();
            cbIDTheLoai.SelectedIndex = -1;
            tbNhaXuatBan.Clear();
            tbNamXuatBan.Clear();
            tbGiaNhap.Clear();
            tbGiaBan.Clear();
            tbSoLuongTon.Clear();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
         
            if (File.Exists(xmlFilePath))
            {
                XElement sachXml = XElement.Load(xmlFilePath);

                // Xóa dữ liệu cũ trong dataGridViewSach
                dataGridViewSach.Rows.Clear();

                // Lọc sách theo chuỗi nhập vào (không phân biệt chữ hoa/thường)
                var matchedBooks = sachXml.Elements("SACH")
                    .Where(sach =>
                sach.Element("IDSach")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                sach.Element("TenSach")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                sach.Element("TacGia")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0);

                if (matchedBooks.Any())
                {
                    foreach (XElement sach in matchedBooks)
                    {
                        int rowIndex = dataGridViewSach.Rows.Add();
                        dataGridViewSach.Rows[rowIndex].Cells["IDSach"].Value = sach.Element("IDSach")?.Value;
                        dataGridViewSach.Rows[rowIndex].Cells["TenSach"].Value = sach.Element("TenSach")?.Value;
                        dataGridViewSach.Rows[rowIndex].Cells["TacGia"].Value = sach.Element("TacGia")?.Value;
                        dataGridViewSach.Rows[rowIndex].Cells["IDTheLoai"].Value = sach.Element("IDTheLoai")?.Value;
                        dataGridViewSach.Rows[rowIndex].Cells["NhaXuatBan"].Value = sach.Element("NhaXuatBan")?.Value;
                        dataGridViewSach.Rows[rowIndex].Cells["NamXuatBan"].Value = sach.Element("NamXuatBan")?.Value;
                        dataGridViewSach.Rows[rowIndex].Cells["GiaNhap"].Value = sach.Element("GiaNhap")?.Value;
                        dataGridViewSach.Rows[rowIndex].Cells["GiaBan"].Value = sach.Element("GiaBan")?.Value;
                        dataGridViewSach.Rows[rowIndex].Cells["SoLuongTon"].Value = sach.Element("SoLuongTon")?.Value;
                    }
                    MessageBox.Show("Đã tìm thấy sách!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sách với thông tin đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'SACH.xml' không tồn tại");
            }
        }

        private void DisplayBookData(XElement book)
        {
            tbIDSach.Text = book.Element("IDSach")?.Value;
            tbTenSach.Text = book.Element("TenSach")?.Value;
            tbTacGia.Text = book.Element("TacGia")?.Value;
            cbIDTheLoai.SelectedItem = book.Element("IDTheLoai")?.Value;
            tbNhaXuatBan.Text = book.Element("NhaXuatBan")?.Value;
            tbNamXuatBan.Text = book.Element("NamXuatBan")?.Value;
            tbGiaNhap.Text = book.Element("GiaNhap")?.Value;
            tbGiaBan.Text = book.Element("GiaBan")?.Value;
            tbSoLuongTon.Text = book.Element("SoLuongTon")?.Value;
        }

        private void quảnLýKháchHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            QuanLyKhachHang changetoform = new QuanLyKhachHang();
            changetoform.Show();
        }

        private void quảnLýHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            QuanLyHoaDon changetoform = new QuanLyHoaDon();
            changetoform.Show();
        }

        private void quảnLýPhiếuNhậpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            QuanLyPhieuNhap changetoform = new QuanLyPhieuNhap();
            changetoform.Show();
        }

        private void quảnLýNhânViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            QuanLyNhanVien changetoform = new QuanLyNhanVien();
            changetoform.Show();
        }

        private void quảnLýNhàCungCấpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            QuanLyNhaCungCap changetoform = new QuanLyNhaCungCap();
            changetoform.Show();
        }


        //=============================================================Đồng bộ======================================================================//
        private void DongBoDuLieuTuXML()
        {
            if (!File.Exists(xmlFilePath))
            {
                MessageBox.Show("Tệp XML không tồn tại.");
                return;
            }

            // Đọc dữ liệu từ tệp XML vào DataTable
            DataSet ds = new DataSet();
            ds.ReadXml(xmlFilePath);
            DataTable dtSach = ds.Tables["SACH"];

            if (dtSach == null)
            {
                MessageBox.Show("Không có dữ liệu sách trong tệp XML.");
                return;
            }

            var idsInXml = new HashSet<string>(); // Use HashSet for faster lookup
            foreach (DataRow row in dtSach.Rows)
            {
                idsInXml.Add(row["IDSach"].ToString());
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (DataRow row in dtSach.Rows)
                {
                    // Kiểm tra xem sách đã tồn tại chưa dựa vào IDSach
                    string checkQuery = "SELECT COUNT(*) FROM SACH WHERE IDSach = @IDSach";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@IDSach", row["IDSach"].ToString());

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count == 0)
                    {
                        // Nếu sách chưa tồn tại, thêm mới vào SQL Server
                        string insertQuery = "INSERT INTO SACH (IDSach, TenSach, TacGia, IDTheLoai, NhaXuatBan, NamXuatBan, GiaNhap, GiaBan, SoLuongTon) " +
                                             "VALUES (@IDSach, @TenSach, @TacGia, @IDTheLoai, @NhaXuatBan, @NamXuatBan, @GiaNhap, @GiaBan, @SoLuongTon)";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@IDSach", row["IDSach"].ToString());
                        insertCommand.Parameters.AddWithValue("@TenSach", row["TenSach"].ToString());
                        insertCommand.Parameters.AddWithValue("@TacGia", row["TacGia"].ToString());
                        insertCommand.Parameters.AddWithValue("@IDTheLoai", row["IDTheLoai"].ToString());
                        insertCommand.Parameters.AddWithValue("@NhaXuatBan", row["NhaXuatBan"].ToString());
                        insertCommand.Parameters.AddWithValue("@NamXuatBan", int.Parse(row["NamXuatBan"].ToString()));
                        insertCommand.Parameters.AddWithValue("@GiaNhap", decimal.Parse(row["GiaNhap"].ToString()));
                        insertCommand.Parameters.AddWithValue("@GiaBan", decimal.Parse(row["GiaBan"].ToString()));
                        insertCommand.Parameters.AddWithValue("@SoLuongTon", int.Parse(row["SoLuongTon"].ToString()));

                        insertCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        // Nếu sách đã tồn tại, cập nhật dữ liệu
                        string updateQuery = "UPDATE SACH SET TenSach = @TenSach, TacGia = @TacGia, IDTheLoai = @IDTheLoai, " +
                                             "NhaXuatBan = @NhaXuatBan, NamXuatBan = @NamXuatBan, GiaNhap = @GiaNhap, GiaBan = @GiaBan, " +
                                             "SoLuongTon = @SoLuongTon WHERE IDSach = @IDSach";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@IDSach", row["IDSach"].ToString());
                        updateCommand.Parameters.AddWithValue("@TenSach", row["TenSach"].ToString());
                        updateCommand.Parameters.AddWithValue("@TacGia", row["TacGia"].ToString());
                        updateCommand.Parameters.AddWithValue("@IDTheLoai", row["IDTheLoai"].ToString());
                        updateCommand.Parameters.AddWithValue("@NhaXuatBan", row["NhaXuatBan"].ToString());
                        updateCommand.Parameters.AddWithValue("@NamXuatBan", int.Parse(row["NamXuatBan"].ToString()));
                        updateCommand.Parameters.AddWithValue("@GiaNhap", decimal.Parse(row["GiaNhap"].ToString()));
                        updateCommand.Parameters.AddWithValue("@GiaBan", decimal.Parse(row["GiaBan"].ToString()));
                        updateCommand.Parameters.AddWithValue("@SoLuongTon", int.Parse(row["SoLuongTon"].ToString()));

                        updateCommand.ExecuteNonQuery();
                    }
                }
                string sqlDeleteQuery = "SELECT IDSach FROM SACH";
                SqlCommand deleteCheckCommand = new SqlCommand(sqlDeleteQuery, connection);
                SqlDataReader reader = deleteCheckCommand.ExecuteReader();

                var idsInSql = new List<string>();
                while (reader.Read())
                {
                    idsInSql.Add(reader["IDSach"].ToString());
                }
                reader.Close();

                // For each ID in SQL, check if it's missing in XML, and delete if necessary
                foreach (var sqlID in idsInSql)
                {
                    if (!idsInXml.Contains(sqlID))
                    {
                        // If the ID doesn't exist in XML, delete it from SQL Server
                        SqlCommand deleteCommand = new SqlCommand("DELETE FROM SACH WHERE IDSach = @IDSach", connection);
                        deleteCommand.Parameters.AddWithValue("@IDSach", sqlID);
                        deleteCommand.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Đồng bộ dữ liệu từ XML vào SQL Server thành công!");
            }
        }
    


    private void btnDongBoDuLieu_Click(object sender, EventArgs e)
        {
            DongBoDuLieuTuXML();
        }
    }
}
