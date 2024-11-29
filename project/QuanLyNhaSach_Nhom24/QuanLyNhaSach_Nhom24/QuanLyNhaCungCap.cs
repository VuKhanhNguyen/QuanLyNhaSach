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
    public partial class QuanLyNhaCungCap : Form
    {
        private readonly string connectionString = "Data Source=LAPTOP-Q12JULH6\\KHANHKHIEMTON;Initial Catalog=dbQUANLYNHASACH;Integrated Security=True";
        private readonly string xmlFilePath = Path.Combine(Application.StartupPath, "NHACUNGCAP.xml");
        public QuanLyNhaCungCap()
        {
            InitializeComponent();
            if (!File.Exists(xmlFilePath))
            {
                ExportNhaCungCapToXml();
            }
            LoadNhaCungCapData();
        }
        private void ExportNhaCungCapToXml()
        {
            try
            {
                string query = "SELECT * FROM NHACUNGCAP";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    XElement sachXml = new XElement("NHACUNGCAPES",
                        dataTable.AsEnumerable().Select(row =>
                            new XElement("NHACUNGCAP",
                                row.Table.Columns.Cast<DataColumn>().Select(col =>
                                    new XElement(col.ColumnName, row[col])
                                )
                            )
                        )
                    );

                    sachXml.Save(xmlFilePath);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất dữ liệu sang XML: " + ex.Message);
            }
        }



        private void QuanLyNhaCungCap_Load(object sender, EventArgs e)
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
        private void LoadNhaCungCapData()
        {
            // Xóa dữ liệu cũ trong dataGridViewSach
            dataGridViewNhaCungCap.Rows.Clear();

            // Kiểm tra và thêm cột nếu chưa có
            if (dataGridViewNhaCungCap.Columns.Count == 0)
            {
                dataGridViewNhaCungCap.Columns.Add("IDNhaCungCap", "ID Nhà cung cấp");
                dataGridViewNhaCungCap.Columns.Add("TenNhaCungCap", "Tên nhà cung cấp");
                dataGridViewNhaCungCap.Columns.Add("DiaChi", "Địa chỉ");
                dataGridViewNhaCungCap.Columns.Add("SoDienThoai", "Số điện thoại");
                dataGridViewNhaCungCap.Columns.Add("Email", "Email");

            }

            // Tải dữ liệu từ file XML
            XElement nhacungcapXml = XElement.Load(xmlFilePath);

            // Thêm từng sách vào dataGridViewSach
            foreach (XElement nhacungcap in nhacungcapXml.Elements("NHACUNGCAP"))
            {
                int rowIndex = dataGridViewNhaCungCap.Rows.Add();
                dataGridViewNhaCungCap.Rows[rowIndex].Cells["IDNhaCungCap"].Value = nhacungcap.Element("IDNhaCungCap")?.Value;
                dataGridViewNhaCungCap.Rows[rowIndex].Cells["TenNhaCungCap"].Value = nhacungcap.Element("TenNhaCungCap")?.Value;
                dataGridViewNhaCungCap.Rows[rowIndex].Cells["DiaChi"].Value = nhacungcap.Element("DiaChi")?.Value;
                dataGridViewNhaCungCap.Rows[rowIndex].Cells["SoDienThoai"].Value = nhacungcap.Element("SoDienThoai")?.Value;
                dataGridViewNhaCungCap.Rows[rowIndex].Cells["Email"].Value = nhacungcap.Element("Email")?.Value;

            }
        }

        private void dataGridViewSach_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewNhaCungCap.Rows[e.RowIndex];
                tbIDNhaCungCap.Text = row.Cells["IDNhaCungCap"].Value.ToString();
                tbTenNhaCungCap.Text = row.Cells["TenNhaCungCap"].Value.ToString();
                tbDiaChi.Text = row.Cells["DiaChi"].Value.ToString();
                tbSoDienThoai.Text = row.Cells["SoDienThoai"].Value.ToString();
                tbEmail.Text = row.Cells["Email"].Value.ToString();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement nhacungcapXml = XElement.Load(xmlFilePath);

                bool exists = nhacungcapXml.Elements("NHACUNGCAP").Any(x => x.Element("IDNhaCungCap")?.Value == tbIDNhaCungCap.Text);
                if (exists)
                {
                    MessageBox.Show("Mã nhà cung cấp đã tồn tại, vui lòng nhập mã khác.");
                    return;
                }

                XElement newCustomer = new XElement("NHACUNGCAP",
                    new XElement("IDNhaCungCap", tbIDNhaCungCap.Text),
                    new XElement("TenNhaCungCap", tbTenNhaCungCap.Text),
                    new XElement("DiaChi", tbDiaChi.Text),
                    new XElement("SoDienThoai", tbSoDienThoai.Text),
                    new XElement("Email", tbEmail.Text)
                );

                nhacungcapXml.Add(newCustomer);
                nhacungcapXml.Save(xmlFilePath);

                MessageBox.Show("Thêm nhà cung cấp thành công!");
                LoadNhaCungCapData();
            }
            else
            {
                MessageBox.Show("File XML 'NHACUNGCAP.xml' không tồn tại");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement nhacungcapXml = XElement.Load(xmlFilePath);

                XElement customer = nhacungcapXml.Elements("NHACUNGCAP").FirstOrDefault(x => x.Element("IDNhaCungCap")?.Value == tbIDNhaCungCap.Text);
                if (customer != null)
                {
                    customer.Element("TenNhaCungCap")?.SetValue(tbTenNhaCungCap.Text);
                    customer.Element("DiaChi")?.SetValue(tbDiaChi.Text);
                    customer.Element("SoDienThoai")?.SetValue(tbSoDienThoai.Text);
                    customer.Element("Email")?.SetValue(tbEmail.Text);
                    nhacungcapXml.Save(xmlFilePath);

                    MessageBox.Show("Cập nhật nhà cung cấp thành công!");
                    LoadNhaCungCapData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhà cung cấp với mã nhà cung cấp đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'NHACUNGCAP.xml' không tồn tại");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement nhacungcapXml = XElement.Load(xmlFilePath);

                XElement customer = nhacungcapXml.Elements("NHACUNGCAP").FirstOrDefault(x => x.Element("IDNhaCungCap")?.Value == tbIDNhaCungCap.Text);
                if (customer != null)
                {
                    customer.Remove();
                    nhacungcapXml.Save(xmlFilePath);

                    MessageBox.Show("Xóa nhà cung cấp thành công!");
                    LoadNhaCungCapData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhà cung cấp với mã nhà cung cấp đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'NHACUNGCAP.xml' không tồn tại");
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            tbIDNhaCungCap.Clear();
            tbTenNhaCungCap.Clear();
            tbDiaChi.Clear();
            tbSoDienThoai.Clear();
            tbEmail.Clear();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                XElement nhacungcapXml = XElement.Load(xmlFilePath);
                var result = nhacungcapXml.Elements("NHACUNGCAP").FirstOrDefault(x => x.Element("IDNhaCungCap")?.Value == tbTimKiem.Text);

                if (result != null)
                {
                    DisplayCustomerData(result);
                    MessageBox.Show("Đã tìm thấy nhà cung cấp!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhà cung cấp với mã nhà cung cấp đã nhập.");
                }
            }
            else
            {
                MessageBox.Show("File XML 'NHACUNGCAP.xml' không tồn tại");
            }
        }

        private void DisplayCustomerData(XElement customer)
        {
            tbIDNhaCungCap.Text = customer.Element("IDNhaCungCap")?.Value;
            tbTenNhaCungCap.Text = customer.Element("TenNhaCungCap")?.Value;
            tbDiaChi.Text = customer.Element("DiaChi")?.Value;
            tbSoDienThoai.Text = customer.Element("SoDienThoai")?.Value;
            tbEmail.Text = customer.Element("Email")?.Value;
        }


        private void DongBoDuLieuTuXML()
        {
            if (!File.Exists(xmlFilePath))
            {
                MessageBox.Show("Tệp XML không tồn tại.");
                return;
            }

            // Read data from XML file into DataTable
            DataSet ds = new DataSet();
            ds.ReadXml(xmlFilePath);
            DataTable dtNhaCungCap = ds.Tables["NHACUNGCAP"];

            if (dtNhaCungCap == null)
            {
                MessageBox.Show("Không có dữ liệu nhà cung cấp trong tệp XML.");
                return;
            }

            var idsInXml = new HashSet<string>();
            foreach (DataRow row in dtNhaCungCap.Rows)
            {
                idsInXml.Add(row["IDNhaCungCap"].ToString());
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (DataRow row in dtNhaCungCap.Rows)
                {
                    string checkQuery = "SELECT COUNT(*) FROM NHACUNGCAP WHERE IDNhaCungCap = @IDNhaCungCap";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@IDNhaCungCap", row["IDNhaCungCap"].ToString());

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count == 0)
                    {
                        string insertQuery = "INSERT INTO NHACUNGCAP (IDNhaCungCap, TenNhaCungCap, DiaChi, SoDienThoai, Email) " +
                                             "VALUES (@IDNhaCungCap, @TenNhaCungCap, @DiaChi, @SoDienThoai, @Email)";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@IDNhaCungCap", row["IDNhaCungCap"].ToString());
                        insertCommand.Parameters.AddWithValue("@TenNhaCungCap", row["TenNhaCungCap"].ToString());
                        insertCommand.Parameters.AddWithValue("@DiaChi", row["DiaChi"].ToString());
                        insertCommand.Parameters.AddWithValue("@SoDienThoai", row["SoDienThoai"].ToString());
                        insertCommand.Parameters.AddWithValue("@Email", row["Email"].ToString());
                        insertCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        string updateQuery = "UPDATE NHACUNGCAP SET TenNhaCungCap = @TenNhaCungCap, DiaChi = @DiaChi, SoDienThoai = @SoDienThoai, Email = @Email " +
                                             "WHERE IDNhaCungCap = @IDNhaCungCap";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@IDNhaCungCap", row["IDNhaCungCap"].ToString());
                        updateCommand.Parameters.AddWithValue("@TenNhaCungCap", row["TenNhaCungCap"].ToString());
                        updateCommand.Parameters.AddWithValue("@DiaChi", row["DiaChi"].ToString());
                        updateCommand.Parameters.AddWithValue("@SoDienThoai", row["SoDienThoai"].ToString());
                        updateCommand.Parameters.AddWithValue("@Email", row["Email"].ToString());
                        updateCommand.ExecuteNonQuery();
                    }
                }

                // Retrieve all IDs in SQL Server to identify entries not present in XML
                string sqlDeleteQuery = "SELECT IDNhaCungCap FROM NHACUNGCAP";
                SqlCommand deleteCheckCommand = new SqlCommand(sqlDeleteQuery, connection);
                SqlDataReader reader = deleteCheckCommand.ExecuteReader();

                var idsInSql = new List<string>();
                while (reader.Read())
                {
                    idsInSql.Add(reader["IDNhaCungCap"].ToString());
                }
                reader.Close();

                // Delete entries in SQL Server that are not in the XML
                foreach (var sqlID in idsInSql)
                {
                    if (!idsInXml.Contains(sqlID))
                    {
                        SqlCommand deleteCommand = new SqlCommand("DELETE FROM NHACUNGCAP WHERE IDNhaCungCap = @IDNhaCungCap", connection);
                        deleteCommand.Parameters.AddWithValue("@IDNhaCungCap", sqlID);
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
