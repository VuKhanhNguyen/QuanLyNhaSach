using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace QuanLyNhaSach_Nhom24
{
    public partial class QuanLyKhachHang : Form
    {
        private readonly string connectionString = "Data Source=LAPTOP-Q12JULH6\\KHANHKHIEMTON;Initial Catalog=dbQUANLYNHASACH;Integrated Security=True";
        private readonly string xmlFilePath = Path.Combine(Application.StartupPath, "KHACHHANG.xml");
        public QuanLyKhachHang()
        {
            InitializeComponent();
            
            if (!File.Exists(xmlFilePath))
            {
                ExportKhachHangToXml();
            }
            LoadKhachHangData();
        }


        private void ExportKhachHangToXml()
        {
            try
            {
                string query = "SELECT * FROM KHACHHANG";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    XElement sachXml = new XElement("KHACHHANGES",
                        dataTable.AsEnumerable().Select(row =>
                            new XElement("KHACHHANG",
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
        }//end ExportSachToXml

        private void LoadKhachHangData()
        {
            
            // Xóa dữ liệu cũ trong dataGridViewSach
            dataGridViewKhachHang.Rows.Clear();

            // Kiểm tra và thêm cột nếu chưa có
            if (dataGridViewKhachHang.Columns.Count == 0)
            {
                dataGridViewKhachHang.Columns.Add("IDKhachHang", "ID Khách hàng");
                dataGridViewKhachHang.Columns.Add("HoTen", "Họ tên");
                dataGridViewKhachHang.Columns.Add("DiaChi", "Địa chỉ");
                dataGridViewKhachHang.Columns.Add("SoDienThoai", "Số điện thoại");
                dataGridViewKhachHang.Columns.Add("Email", "Email");
                
            }

            // Tải dữ liệu từ file XML
            XElement khachhangXml = XElement.Load(xmlFilePath);

            // Thêm từng sách vào dataGridViewSach
            foreach (XElement khachhang in khachhangXml.Elements("KHACHHANG"))
            {
                int rowIndex = dataGridViewKhachHang.Rows.Add();
                dataGridViewKhachHang.Rows[rowIndex].Cells["IDKhachHang"].Value = khachhang.Element("IDKhachHang")?.Value;
                dataGridViewKhachHang.Rows[rowIndex].Cells["HoTen"].Value = khachhang.Element("HoTen")?.Value;
                dataGridViewKhachHang.Rows[rowIndex].Cells["DiaChi"].Value = khachhang.Element("DiaChi")?.Value;
                dataGridViewKhachHang.Rows[rowIndex].Cells["SoDienThoai"].Value = khachhang.Element("SoDienThoai")?.Value;
                dataGridViewKhachHang.Rows[rowIndex].Cells["Email"].Value = khachhang.Element("Email")?.Value;
                
            }
        }

        private void QuanLyKhachHang_Load(object sender, EventArgs e)
        {
            label2.Parent = pictureBox1;
            label2.BackColor = Color.Transparent;
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
       

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            tbIDKhachHang.Clear();
            tbHoTen.Clear();
            tbDiaChi.Clear();
            tbSoDienThoai.Clear();
            tbEmail.Clear();
        }

        private void dataGridViewSach_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow row = dataGridViewKhachHang.Rows[e.RowIndex];
                tbIDKhachHang.Text = row.Cells["IDKhachHang"].Value.ToString();
                tbHoTen.Text = row.Cells["HoTen"].Value.ToString();
                tbDiaChi.Text = row.Cells["DiaChi"].Value.ToString();
                tbSoDienThoai.Text = row.Cells["SoDienThoai"].Value.ToString();
                tbEmail.Text = row.Cells["Email"].Value.ToString();             
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement khachhangXml = XElement.Load(xmlFilePath);

                bool exists = khachhangXml.Elements("KHACHHANG").Any(x => x.Element("IDKhachHang")?.Value == tbIDKhachHang.Text);
                if (exists)
                {
                    MessageBox.Show("Mã khách hàng đã tồn tại, vui lòng nhập mã khác.");
                    return;
                }

                XElement newCustomer = new XElement("KHACHHANG",
                    new XElement("IDKhachHang", tbIDKhachHang.Text),
                    new XElement("HoTen", tbHoTen.Text),
                    new XElement("DiaChi", tbDiaChi.Text),
                    new XElement("SoDienThoai", tbSoDienThoai.Text),
                    new XElement("Email", tbEmail.Text)
                );

                khachhangXml.Add(newCustomer);
                khachhangXml.Save(xmlFilePath);

                MessageBox.Show("Thêm khách hàng thành công!");
                LoadKhachHangData();
            }
            else
            {
                MessageBox.Show("File XML 'KHACHHANG.xml' không tồn tại");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement khachhangXml = XElement.Load(xmlFilePath);

                XElement customer = khachhangXml.Elements("KHACHHANG").FirstOrDefault(x => x.Element("IDKhachHang")?.Value == tbIDKhachHang.Text);
                if (customer != null)
                {
                    customer.Element("HoTen")?.SetValue(tbHoTen.Text);
                    customer.Element("DiaChi")?.SetValue(tbDiaChi.Text);
                    customer.Element("SoDienThoai")?.SetValue(tbSoDienThoai.Text);
                    customer.Element("Email")?.SetValue(tbEmail.Text);
                    khachhangXml.Save(xmlFilePath);

                    MessageBox.Show("Cập nhật khách hàng thành công!");
                    LoadKhachHangData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng với mã khách hàng đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'KHACHHANG.xml' không tồn tại");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement khachhangXml = XElement.Load(xmlFilePath);

                XElement customer = khachhangXml.Elements("KHACHHANG").FirstOrDefault(x => x.Element("IDKhachHang")?.Value == tbIDKhachHang.Text);
                if (customer != null)
                {
                    customer.Remove();
                    khachhangXml.Save(xmlFilePath);

                    MessageBox.Show("Xóa khách hàng thành công!");
                    LoadKhachHangData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng với mã khách hàng đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'KHACHHANG.xml' không tồn tại");
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement khachhangXml = XElement.Load(xmlFilePath);

                // Xóa dữ liệu cũ trong dataGridViewKhachHang
                dataGridViewKhachHang.Rows.Clear();

                // Lọc khách hàng theo chuỗi nhập vào
                var matchedCustomers = khachhangXml.Elements("KHACHHANG")
                    .Where(kh =>
                        kh.Element("IDKhachHang")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        kh.Element("HoTen")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        kh.Element("DiaChi")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        kh.Element("SoDienThoai")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        kh.Element("Email")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0);

                if (matchedCustomers.Any())
                {
                    foreach (XElement kh in matchedCustomers)
                    {
                        int rowIndex = dataGridViewKhachHang.Rows.Add();
                        dataGridViewKhachHang.Rows[rowIndex].Cells["IDKhachHang"].Value = kh.Element("IDKhachHang")?.Value;
                        dataGridViewKhachHang.Rows[rowIndex].Cells["HoTen"].Value = kh.Element("HoTen")?.Value;
                        dataGridViewKhachHang.Rows[rowIndex].Cells["DiaChi"].Value = kh.Element("DiaChi")?.Value;
                        dataGridViewKhachHang.Rows[rowIndex].Cells["SoDienThoai"].Value = kh.Element("SoDienThoai")?.Value;
                        dataGridViewKhachHang.Rows[rowIndex].Cells["Email"].Value = kh.Element("Email")?.Value;
                    }
                    MessageBox.Show("Đã tìm thấy khách hàng!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng với thông tin đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'KHACHHANG.xml' không tồn tại");
            }
        }

        private void DisplayCustomerData(XElement customer)
        {
            tbIDKhachHang.Text = customer.Element("IDKhachHang")?.Value;
            tbHoTen.Text = customer.Element("HoTen")?.Value;
            tbDiaChi.Text = customer.Element("DiaChi")?.Value;
            tbSoDienThoai.Text = customer.Element("SoDienThoai")?.Value;
            tbEmail.Text = customer.Element("Email")?.Value;
        }

        private void quảnLýSáchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            QuanLySach changetoform = new QuanLySach();
            changetoform.Show();
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
            DataTable dtKhachHang = ds.Tables["KHACHHANG"];

            if (dtKhachHang == null)
            {
                MessageBox.Show("Không có dữ liệu khách hàng trong tệp XML.");
                return;
            }

            var idsInXml = new HashSet<string>(); // Use HashSet for faster lookup
            foreach (DataRow row in dtKhachHang.Rows)
            {
                idsInXml.Add(row["IDKhachHang"].ToString());
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (DataRow row in dtKhachHang.Rows)
                {
                    string checkQuery = "SELECT COUNT(*) FROM KHACHHANG WHERE IDKhachHang = @IDKhachHang";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@IDKhachHang", row["IDKhachHang"].ToString());

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count == 0)
                    {
                        string insertQuery = "INSERT INTO KHACHHANG (IDKhachHang, HoTen, DiaChi, SoDienThoai, Email) " +
                                             "VALUES (@IDKhachHang, @HoTen, @DiaChi, @SoDienThoai, @Email)";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@IDKhachHang", row["IDKhachHang"].ToString());
                        insertCommand.Parameters.AddWithValue("@HoTen", row["HoTen"].ToString());
                        insertCommand.Parameters.AddWithValue("@DiaChi", row["DiaChi"].ToString());
                        insertCommand.Parameters.AddWithValue("@SoDienThoai", row["SoDienThoai"].ToString());
                        insertCommand.Parameters.AddWithValue("@Email", row["Email"].ToString());
                        insertCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        string updateQuery = "UPDATE KHACHHANG SET HoTen = @HoTen, DiaChi = @DiaChi, SoDienThoai = @SoDienThoai, Email = @Email " +
                     "WHERE IDKhachHang = @IDKhachHang";

                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@IDKhachHang", row["IDKhachHang"].ToString());
                        updateCommand.Parameters.AddWithValue("@HoTen", row["HoTen"].ToString());
                        updateCommand.Parameters.AddWithValue("@DiaChi", row["DiaChi"].ToString());
                        updateCommand.Parameters.AddWithValue("@SoDienThoai", row["SoDienThoai"].ToString());
                        updateCommand.Parameters.AddWithValue("@Email", row["Email"].ToString());
                        updateCommand.ExecuteNonQuery();
                    }
                }
                string sqlDeleteQuery = "SELECT IDKhachHang FROM KHACHHANG";
                SqlCommand deleteCheckCommand = new SqlCommand(sqlDeleteQuery, connection);
                SqlDataReader reader = deleteCheckCommand.ExecuteReader();

                var idsInSql = new List<string>();
                while (reader.Read())
                {
                    idsInSql.Add(reader["IDKhachHang"].ToString());
                }
                reader.Close();

                // For each ID in SQL, check if it's missing in XML, and delete if necessary
                foreach (var sqlID in idsInSql)
                {
                    if (!idsInXml.Contains(sqlID))
                    {
                        // If the ID doesn't exist in XML, delete it from SQL Server
                        SqlCommand deleteCommand = new SqlCommand("DELETE FROM KHACHHANG WHERE IDKhachHang = @IDKhachHang", connection);
                        deleteCommand.Parameters.AddWithValue("@IDKhachHang", sqlID);
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
