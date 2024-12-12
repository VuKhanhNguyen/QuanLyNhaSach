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
    public partial class QuanLyNhanVien : Form
    {
        private readonly string connectionString = "Data Source=LAPTOP-Q12JULH6\\KHANHKHIEMTON;Initial Catalog=dbQUANLYNHASACH;Integrated Security=True";
        private readonly string xmlFilePath = Path.Combine(Application.StartupPath, "NHANVIEN.xml");
        public QuanLyNhanVien()
        {
            InitializeComponent();
            if (!File.Exists(xmlFilePath))
            {
                ExportNhanVienToXml();
            }
            LoadNhanVienData();
        }

        private void ExportNhanVienToXml()
        {
            try
            {
                string query = "SELECT * FROM NHANVIEN";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    XElement nhanvienXml = new XElement("NHANVIENES",
                        dataTable.AsEnumerable().Select(row =>
                            new XElement("NHANVIEN",
                                row.Table.Columns.Cast<DataColumn>().Select(col =>
                                    new XElement(col.ColumnName, row[col])
                                )
                            )
                        )
                    );

                    nhanvienXml.Save(xmlFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất dữ liệu sang XML: " + ex.Message);
            }
        }



        private void QuanLyNhanVien_Load(object sender, EventArgs e)
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

        private void LoadNhanVienData()
        {
            dataGridViewNhanVien.Rows.Clear();

            if (dataGridViewNhanVien.Columns.Count == 0)
            {
                dataGridViewNhanVien.Columns.Add("IDNhanVien", "ID Nhân viên");
                dataGridViewNhanVien.Columns.Add("HoTen", "Họ tên");
                dataGridViewNhanVien.Columns.Add("SoDienThoai", "Số điện thoại");
                dataGridViewNhanVien.Columns.Add("TaiKhoan", "Tài khoản");
                dataGridViewNhanVien.Columns.Add("MatKhau", "Mật khẩu");
            }

            XElement nhanvienXml = XElement.Load(xmlFilePath);

            foreach (XElement nhanvien in nhanvienXml.Elements("NHANVIEN"))
            {
                int rowIndex = dataGridViewNhanVien.Rows.Add();
                dataGridViewNhanVien.Rows[rowIndex].Cells["IDNhanVien"].Value = nhanvien.Element("IDNhanVien")?.Value;
                dataGridViewNhanVien.Rows[rowIndex].Cells["HoTen"].Value = nhanvien.Element("HoTen")?.Value;
                dataGridViewNhanVien.Rows[rowIndex].Cells["SoDienThoai"].Value = nhanvien.Element("SoDienThoai")?.Value;
                dataGridViewNhanVien.Rows[rowIndex].Cells["TaiKhoan"].Value = nhanvien.Element("TaiKhoan")?.Value;
                dataGridViewNhanVien.Rows[rowIndex].Cells["MatKhau"].Value = nhanvien.Element("MatKhau")?.Value;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement nhanvienXml = XElement.Load(xmlFilePath);

                bool exists = nhanvienXml.Elements("NHANVIEN").Any(x => x.Element("IDNhanVien")?.Value == tbIDNhanVien.Text);
                if (exists)
                {
                    MessageBox.Show("Mã nhân viên đã tồn tại, vui lòng nhập mã khác.");
                    return;
                }

                XElement newEmployee = new XElement("NHANVIEN",
                    new XElement("IDNhanVien", tbIDNhanVien.Text),
                    new XElement("HoTen", tbHoTen.Text),
                    new XElement("SoDienThoai", tbSoDienThoai.Text),
                    new XElement("TaiKhoan", tbTaiKhoan.Text),
                    new XElement("MatKhau", tbMatKhau.Text)
                );

                nhanvienXml.Add(newEmployee);
                nhanvienXml.Save(xmlFilePath);

                MessageBox.Show("Thêm nhân viên thành công!");
                LoadNhanVienData();
            }
            else
            {
                MessageBox.Show("File XML 'NHANVIEN.xml' không tồn tại");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement nhanvienXml = XElement.Load(xmlFilePath);

                XElement employee = nhanvienXml.Elements("NHANVIEN").FirstOrDefault(x => x.Element("IDNhanVien")?.Value == tbIDNhanVien.Text);
                if (employee != null)
                {
                    employee.Element("HoTen")?.SetValue(tbHoTen.Text);
                    employee.Element("SoDienThoai")?.SetValue(tbSoDienThoai.Text);
                    employee.Element("TaiKhoan")?.SetValue(tbTaiKhoan.Text);
                    employee.Element("MatKhau")?.SetValue(tbMatKhau.Text);
                    nhanvienXml.Save(xmlFilePath);

                    MessageBox.Show("Cập nhật nhân viên thành công!");
                    LoadNhanVienData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên với mã nhân viên đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'NHANVIEN.xml' không tồn tại");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement nhanvienXml = XElement.Load(xmlFilePath);

                XElement employee = nhanvienXml.Elements("NHANVIEN").FirstOrDefault(x => x.Element("IDNhanVien")?.Value == tbIDNhanVien.Text);
                if (employee != null)
                {
                    employee.Remove();
                    nhanvienXml.Save(xmlFilePath);

                    MessageBox.Show("Xóa nhân viên thành công!");
                    LoadNhanVienData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên với mã nhân viên đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'NHANVIEN.xml' không tồn tại");
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement nhanvienXml = XElement.Load(xmlFilePath);

                // Xóa dữ liệu cũ trong dataGridViewNhanVien
                dataGridViewNhanVien.Rows.Clear();

                // Lọc nhân viên theo chuỗi nhập vào
                var matchedEmployees = nhanvienXml.Elements("NHANVIEN")
                    .Where(nv =>
                        nv.Element("IDNhanVien")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        nv.Element("HoTen")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        nv.Element("SoDienThoai")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        nv.Element("TaiKhoan")?.Value.IndexOf(tbTimKiem.Text, StringComparison.OrdinalIgnoreCase) >= 0);

                if (matchedEmployees.Any())
                {
                    foreach (XElement nv in matchedEmployees)
                    {
                        int rowIndex = dataGridViewNhanVien.Rows.Add();
                        dataGridViewNhanVien.Rows[rowIndex].Cells["IDNhanVien"].Value = nv.Element("IDNhanVien")?.Value;
                        dataGridViewNhanVien.Rows[rowIndex].Cells["HoTen"].Value = nv.Element("HoTen")?.Value;
                        dataGridViewNhanVien.Rows[rowIndex].Cells["SoDienThoai"].Value = nv.Element("SoDienThoai")?.Value;
                        dataGridViewNhanVien.Rows[rowIndex].Cells["TaiKhoan"].Value = nv.Element("TaiKhoan")?.Value;
                    }
                    MessageBox.Show("Đã tìm thấy nhân viên!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên với thông tin đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'NHANVIEN.xml' không tồn tại");
            }
        }

        private void DisplayEmployeeData(XElement employee)
        {
            tbIDNhanVien.Text = employee.Element("IDNhanVien")?.Value;
            tbHoTen.Text = employee.Element("HoTen")?.Value;
            tbSoDienThoai.Text = employee.Element("SoDienThoai")?.Value;
            tbTaiKhoan.Text = employee.Element("TaiKhoan")?.Value;
            tbMatKhau.Text = employee.Element("MatKhau")?.Value;
        }


        private void dataGridViewSach_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewNhanVien.Rows[e.RowIndex];
                tbIDNhanVien.Text = row.Cells["IDNhanVien"].Value.ToString();
                tbHoTen.Text = row.Cells["HoTen"].Value.ToString();
                tbSoDienThoai.Text = row.Cells["SoDienThoai"].Value.ToString();
                tbTaiKhoan.Text = row.Cells["TaiKhoan"].Value.ToString();
                tbMatKhau.Text = row.Cells["MatKhau"].Value.ToString();
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            tbIDNhanVien.Clear();
            tbHoTen.Clear();
            tbSoDienThoai.Clear();
            tbTaiKhoan.Clear();
            tbMatKhau.Clear();
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
            DataTable dtNhanVien = ds.Tables["NHANVIEN"];

            if (dtNhanVien == null)
            {
                MessageBox.Show("Không có dữ liệu nhân viên trong tệp XML.");
                return;
            }

            var idsInXml = new HashSet<string>(); // Lưu danh sách IDNhanVien trong XML
            foreach (DataRow row in dtNhanVien.Rows)
            {
                idsInXml.Add(row["IDNhanVien"].ToString());
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (DataRow row in dtNhanVien.Rows)
                {
                    string checkQuery = "SELECT COUNT(*) FROM NHANVIEN WHERE IDNhanVien = @IDNhanVien";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@IDNhanVien", row["IDNhanVien"].ToString());

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count == 0)
                    {
                        // Thêm mới dữ liệu nếu không tồn tại trong SQL
                        string insertQuery = "INSERT INTO NHANVIEN (IDNhanVien, HoTen, SoDienThoai, TaiKhoan, MatKhau) " +
                                             "VALUES (@IDNhanVien, @HoTen, @SoDienThoai, @TaiKhoan, @MatKhau)";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@IDNhanVien", row["IDNhanVien"].ToString());
                        insertCommand.Parameters.AddWithValue("@HoTen", row["HoTen"].ToString());
                        insertCommand.Parameters.AddWithValue("@SoDienThoai", row["SoDienThoai"].ToString());
                        insertCommand.Parameters.AddWithValue("@TaiKhoan", row["TaiKhoan"].ToString());
                        insertCommand.Parameters.AddWithValue("@MatKhau", row["MatKhau"].ToString());
                        insertCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        // Cập nhật dữ liệu nếu đã tồn tại trong SQL
                        string updateQuery = "UPDATE NHANVIEN SET HoTen = @HoTen, SoDienThoai = @SoDienThoai, TaiKhoan = @TaiKhoan, MatKhau = @MatKhau " +
                                             "WHERE IDNhanVien = @IDNhanVien";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@IDNhanVien", row["IDNhanVien"].ToString());
                        updateCommand.Parameters.AddWithValue("@HoTen", row["HoTen"].ToString());
                        updateCommand.Parameters.AddWithValue("@SoDienThoai", row["SoDienThoai"].ToString());
                        updateCommand.Parameters.AddWithValue("@TaiKhoan", row["TaiKhoan"].ToString());
                        updateCommand.Parameters.AddWithValue("@MatKhau", row["MatKhau"].ToString());
                        updateCommand.ExecuteNonQuery();
                    }
                }

                // Lấy danh sách IDNhanVien trong SQL để kiểm tra và xóa nếu không tồn tại trong XML
                string sqlDeleteQuery = "SELECT IDNhanVien FROM NHANVIEN";
                SqlCommand deleteCheckCommand = new SqlCommand(sqlDeleteQuery, connection);
                SqlDataReader reader = deleteCheckCommand.ExecuteReader();

                var idsInSql = new List<string>();
                while (reader.Read())
                {
                    idsInSql.Add(reader["IDNhanVien"].ToString());
                }
                reader.Close();

                // Xóa dữ liệu trong SQL nếu không có trong XML
                foreach (var sqlID in idsInSql)
                {
                    if (!idsInXml.Contains(sqlID))
                    {
                        SqlCommand deleteCommand = new SqlCommand("DELETE FROM NHANVIEN WHERE IDNhanVien = @IDNhanVien", connection);
                        deleteCommand.Parameters.AddWithValue("@IDNhanVien", sqlID);
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
