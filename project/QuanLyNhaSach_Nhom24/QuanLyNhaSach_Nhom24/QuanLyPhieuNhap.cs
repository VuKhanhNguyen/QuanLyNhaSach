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
    public partial class QuanLyPhieuNhap : Form
    {
        private readonly string connectionString = "Data Source=LAPTOP-Q12JULH6\\KHANHKHIEMTON;Initial Catalog=dbQUANLYNHASACH;Integrated Security=True";
        private readonly string phieuNhapFilePath = Path.Combine(Application.StartupPath, "PHIEUNHAP.xml");
        private readonly string chiTietPhieuNhapFilePath = Path.Combine(Application.StartupPath, "CHITIETPHIEUNHAP.xml");
        private readonly string maNhaCungCapFilePath = Path.Combine(Application.StartupPath, "NHACUNGCAP.xml");
        private readonly string maSachFilePath = Path.Combine(Application.StartupPath, "SACH.xml");
        private readonly string maNhanVienFilePath = Path.Combine(Application.StartupPath, "NHANVIEN.xml");

        public QuanLyPhieuNhap()
        {
            InitializeComponent();
            // Tạo file XML nếu chưa tồn tại
            if (!File.Exists(phieuNhapFilePath))
            {
                ExportPhieuNhapToXml();
            }
            if (!File.Exists(chiTietPhieuNhapFilePath))
            {
                ExportChiTietPhieuNhapToXml();
            }

            //Tải dữ liệu vào Combobox
            LoadIDNhaCungCap();
            LoadIDSach();
            LoadIDNhanVien();


            // Tải dữ liệu vào DataGridView
            LoadPhieuNhapData();
            LoadChiTietPhieuNhapData();
        }

        private void LoadIDNhaCungCap()
        {
            if (File.Exists(maNhaCungCapFilePath))
            {
                XElement maKhachHangXml = XElement.Load(maNhaCungCapFilePath);
                var ids = maKhachHangXml.Elements("NHACUNGCAP").Select(x => x.Element("IDNhaCungCap")?.Value);
                foreach (var id in ids)
                {
                    cbIDNhaCungCap.Items.Add(id);
                }
            }
            else
            {
                MessageBox.Show("File XML 'KHACHHANG.xml' không tồn tại");
            }
        }
        private void LoadIDSach()
        {
            if (File.Exists(maSachFilePath))
            {
                XElement maSachXml = XElement.Load(maSachFilePath);
                var ids = maSachXml.Elements("SACH").Select(x => x.Element("IDSach")?.Value);
                foreach (var id in ids)
                {
                    cbIDSach.Items.Add(id);
                }
            }
            else
            {
                MessageBox.Show("File XML 'SACH.xml' không tồn tại");
            }
        }
        private void LoadIDNhanVien()
        {
            if (File.Exists(maNhanVienFilePath))
            {
                XElement maNhanVienXml = XElement.Load(maNhanVienFilePath);
                var ids = maNhanVienXml.Elements("NHANVIEN").Select(x => x.Element("IDNhanVien")?.Value);
                foreach (var id in ids)
                {
                    cbIDNhanVien.Items.Add(id);
                }
            }
            else
            {
                MessageBox.Show("File XML 'NHANVIEN.xml' không tồn tại");
            }
        }

        private void ExportPhieuNhapToXml()
        {
            try
            {
                string query = "SELECT * FROM PHIEUNHAP";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    XElement phieuNhapXml = new XElement("PHIEUNHAPES",
                        dataTable.AsEnumerable().Select(row =>
                            new XElement("PHIEUNHAP",
                                row.Table.Columns.Cast<DataColumn>().Select(col =>
                                    new XElement(col.ColumnName, row[col])
                                )
                            )
                        )
                    );

                    phieuNhapXml.Save(phieuNhapFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất dữ liệu PHIẾU NHẬP sang XML: " + ex.Message);
            }
        }
        private void ExportChiTietPhieuNhapToXml()
        {
            try
            {
                string query = "SELECT * FROM CHITIETPHIEUNHAP";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    XElement chiTietPhieuNhapXml = new XElement("CHITIETPHIEUNHAPES",
                        dataTable.AsEnumerable().Select(row =>
                            new XElement("CHITIETPHIEUNHAP",
                                row.Table.Columns.Cast<DataColumn>().Select(col =>
                                    new XElement(col.ColumnName, row[col])
                                )
                            )
                        )
                    );

                    chiTietPhieuNhapXml.Save(chiTietPhieuNhapFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất dữ liệu CHI TIẾT PHIẾU NHẬP sang XML: " + ex.Message);
            }
        }

        private void LoadPhieuNhapData()
        {
            dataGridViewPhieuNhap.Rows.Clear();

            if (dataGridViewPhieuNhap.Columns.Count == 0)
            {
                dataGridViewPhieuNhap.Columns.Add("IDPhieuNhap", "ID Phiếu Nhập");
                dataGridViewPhieuNhap.Columns.Add("NgayNhap", "Ngày Nhập");
                dataGridViewPhieuNhap.Columns.Add("IDNhaCungCap", "ID Nhà Cung Cấp");
                dataGridViewPhieuNhap.Columns.Add("IDNhanVien", "ID Nhân Viên");
                dataGridViewPhieuNhap.Columns.Add("TongTien", "Tổng Tiền");
            }

            XElement phieuNhapXml = XElement.Load(phieuNhapFilePath);

            foreach (XElement phieuNhap in phieuNhapXml.Elements("HOADON"))
            {
                int rowIndex = dataGridViewPhieuNhap.Rows.Add();
                dataGridViewPhieuNhap.Rows[rowIndex].Cells["IDPhieuNhap"].Value = phieuNhap.Element("IDPhieuNhap")?.Value;
                dataGridViewPhieuNhap.Rows[rowIndex].Cells["NgayNhap"].Value = phieuNhap.Element("NgayNhap")?.Value;
                dataGridViewPhieuNhap.Rows[rowIndex].Cells["IDNhaCungCap"].Value = phieuNhap.Element("IDNhaCungCap")?.Value;
                dataGridViewPhieuNhap.Rows[rowIndex].Cells["IDNhanVien"].Value = phieuNhap.Element("IDNhanVien")?.Value;
                dataGridViewPhieuNhap.Rows[rowIndex].Cells["TongTien"].Value = phieuNhap.Element("TongTien")?.Value;
            }
        }

        private void LoadChiTietPhieuNhapData()
        {
            dataGridViewChiTietPhieuNhap.Rows.Clear();

            if (dataGridViewChiTietPhieuNhap.Columns.Count == 0)
            {
                dataGridViewChiTietPhieuNhap.Columns.Add("IDPhieuNhap", "ID Phiếu Nhập");
                dataGridViewChiTietPhieuNhap.Columns.Add("IDSach", "ID Sách");
                dataGridViewChiTietPhieuNhap.Columns.Add("SoLuongNhap", "Số Lượng Nhập");
                dataGridViewChiTietPhieuNhap.Columns.Add("DonGiaNhap", "Đơn Giá Nhập");
                dataGridViewChiTietPhieuNhap.Columns.Add("ThanhTienNhap", "Thành Tiền Nhập");
            }

            XElement chiTietPhieuNhapXml = XElement.Load(chiTietPhieuNhapFilePath);

            foreach (XElement chiTietPhieuNhap in chiTietPhieuNhapXml.Elements("CHITIETPHIEUNHAP"))
            {
                int rowIndex = dataGridViewChiTietPhieuNhap.Rows.Add();
                dataGridViewChiTietPhieuNhap.Rows[rowIndex].Cells["IDPhieuNhap"].Value = chiTietPhieuNhap.Element("IDPhieuNhap")?.Value;
                dataGridViewChiTietPhieuNhap.Rows[rowIndex].Cells["IDSach"].Value = chiTietPhieuNhap.Element("IDSach")?.Value;
                dataGridViewChiTietPhieuNhap.Rows[rowIndex].Cells["SoLuongNhap"].Value = chiTietPhieuNhap.Element("SoLuongNhap")?.Value;
                dataGridViewChiTietPhieuNhap.Rows[rowIndex].Cells["DonGiaNhap"].Value = chiTietPhieuNhap.Element("DonGiaNhap")?.Value;
                dataGridViewChiTietPhieuNhap.Rows[rowIndex].Cells["ThanhTienNhap"].Value = chiTietPhieuNhap.Element("ThanhTienNhap")?.Value;
            }
        }




        private void pictureBox1_Click(object sender, EventArgs e)
        {

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

        private void QuanLyPhieuNhap_Load(object sender, EventArgs e)
        {
            label11.Parent = pictureBox1;
            label11.BackColor = Color.Transparent;
            label7.Parent = pictureBox1;
            label7.BackColor = Color.Transparent;
            label10.Parent = pictureBox1;
            label10.BackColor = Color.Transparent;
        }

        private void UpdateTongTien(string idPhieuNhap)
        {
            
            if (File.Exists(phieuNhapFilePath))
            {
                XElement phieuNhapXml = XElement.Load(phieuNhapFilePath);

               
                XElement phieuNhap = phieuNhapXml.Elements("PHIEUNHAP")
                    .FirstOrDefault(x => x.Element("IDPhieuNhap")?.Value == idPhieuNhap);

                if (phieuNhap != null)
                {
               
                    decimal tongTien = 0;
                    var chiTietPhieuNhaps = XElement.Load(chiTietPhieuNhapFilePath)
                        .Elements("CHITIETPHIEUNHAP")
                        .Where(x => x.Element("IDPhieuNhap")?.Value == idPhieuNhap);

                    foreach (var chiTiet in chiTietPhieuNhaps)
                    {
                        decimal thanhTien = decimal.Parse(chiTiet.Element("ThanhTienNhap")?.Value ?? "0");
                        tongTien += thanhTien;
                    }

               
                    phieuNhap.Element("TongTien")?.SetValue(tongTien.ToString());

                    phieuNhapXml.Save(phieuNhapFilePath);
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                
                XElement phieuNhapXml;
                if (File.Exists(chiTietPhieuNhapFilePath))
                {
                    phieuNhapXml = XElement.Load(chiTietPhieuNhapFilePath);
                }
                else
                {
                    phieuNhapXml = new XElement("PHIEUNHAPES");
                }

                
                bool existsHoaDon = phieuNhapXml.Elements("PHIEUNHAP").Any(x => x.Element("IDPhieuNhap")?.Value == tbIDPhieuNhap.Text);
                if (existsHoaDon)
                {
                    MessageBox.Show("Mã phiếu nhập đã tồn tại, vui lòng nhập mã khác.");
                    return;
                }

              
                XElement newPhieuNhap = new XElement("PHIEUNHAP",
                    new XElement("IDPhieuNhap", tbIDPhieuNhap.Text),
                    new XElement("NgayNhap", dtpNgayNhap.Value.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("IDNhaCungCap", cbIDNhaCungCap.SelectedItem?.ToString()),
                    new XElement("IDNhanVien", cbIDNhanVien.SelectedItem?.ToString()),
                    new XElement("TongTien", "0") // Tổng tiền sẽ cập nhật sau
                );
                phieuNhapXml.Add(newPhieuNhap);
                phieuNhapXml.Save(phieuNhapFilePath);

          
                XElement chiTietPhieuNhapXml;
                if (File.Exists(chiTietPhieuNhapFilePath))
                {
                    chiTietPhieuNhapXml = XElement.Load(chiTietPhieuNhapFilePath);
                }
                else
                {
                    chiTietPhieuNhapXml = new XElement("CHITIETPHIEUNHAPES");
                }

              
                bool existsChiTiet = chiTietPhieuNhapXml.Elements("CHITIETPHIEUNHAP").Any(x =>
                    x.Element("IDPhieuNhap")?.Value == tbIDPhieuNhap.Text &&
                    x.Element("IDSach")?.Value == cbIDSach.SelectedValue?.ToString());
                if (existsChiTiet)
                {
                    MessageBox.Show("Chi tiết phiếu nhập với mã sách này đã tồn tại trong phiếu nhập hiện tại.");
                    return;
                }

             
                XElement newChiTiet = new XElement("CHITIETPHIEUNHAP",
                    new XElement("IDPhieuNhap", tbIDPhieuNhap.Text),
                    new XElement("IDSach", cbIDSach.SelectedItem?.ToString()),
                    new XElement("SoLuong", tbSoLuongNhap.Text),
                    new XElement("DonGia", tbDonGiaNhap.Text),
                    new XElement("ThanhTienNhap", (int.Parse(tbSoLuongNhap.Text) * decimal.Parse(tbDonGiaNhap.Text)).ToString())
                );
                chiTietPhieuNhapXml.Add(newChiTiet);
                chiTietPhieuNhapXml.Save(chiTietPhieuNhapFilePath);

             
                UpdateTongTien(tbIDPhieuNhap.Text);

             
                MessageBox.Show("Thêm phiếu nhập và chi tiết phiếu nhập thành công!");

          
                LoadPhieuNhapData();
                LoadChiTietPhieuNhapData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void dataGridViewPhieuNhap_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewPhieuNhap.Rows[e.RowIndex];

                tbIDPhieuNhap.Text = row.Cells["IDPhieuNhap"].Value.ToString();
                dtpNgayNhap.Value = DateTime.Parse(row.Cells["NgayNhap"].Value.ToString());
                cbIDNhaCungCap.SelectedItem = row.Cells["IDNhaCungCap"].Value.ToString();
                cbIDNhanVien.SelectedItem = row.Cells["IDNhanVien"].Value.ToString();
                //tbTongTien.Text = row.Cells["TongTien"].Value.ToString();
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            tbIDPhieuNhap.Clear();
            dtpNgayNhap.Value = DateTime.Now;
            cbIDNhaCungCap.SelectedIndex = -1;
            cbIDNhanVien.SelectedIndex = -1;
            cbIDSach.SelectedIndex = -1;
            tbSoLuongNhap.Clear();
            tbDonGiaNhap.Clear();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (File.Exists(phieuNhapFilePath))
                {
                    XElement phieuNhapXml = XElement.Load(phieuNhapFilePath);

                    XElement phieuNhap = phieuNhapXml.Elements("PHIEUNHAP")
                        .FirstOrDefault(x => x.Element("IDPhieuNhap")?.Value == tbIDPhieuNhap.Text);

                    if (phieuNhap != null)
                    {

                        phieuNhap.Element("NgayNhap")?.SetValue(dtpNgayNhap.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        phieuNhap.Element("IDNhaCungCap")?.SetValue(cbIDNhaCungCap.SelectedValue?.ToString() ?? "");
                        phieuNhap.Element("IDNhanVien")?.SetValue(cbIDNhanVien.SelectedValue?.ToString() ?? "");


                        phieuNhapXml.Save(phieuNhapFilePath);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy phiếu nhập với mã phiếu nhập đã nhập.");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("File XML 'PHIEUNHAP.xml' không tồn tại.");
                    return;
                }

               
                if (File.Exists(chiTietPhieuNhapFilePath))
                {
                    XElement chiTietPhieuNhapXml = XElement.Load(chiTietPhieuNhapFilePath);

                   
                    XElement chiTiet = chiTietPhieuNhapXml.Elements("CHITIETPHIEUNHAP")
                        .FirstOrDefault(x =>
                            x.Element("IDPhieuNhap")?.Value == tbIDPhieuNhap.Text &&
                            x.Element("IDSach")?.Value == cbIDSach.SelectedItem?.ToString()
                        );

                    if (chiTiet != null)
                    {
                        
                        chiTiet.Element("SoLuongNhap")?.SetValue(tbSoLuongNhap.Text);
                        chiTiet.Element("DonGiaNhap")?.SetValue(tbDonGiaNhap.Text);

                        decimal soLuong = decimal.Parse(tbSoLuongNhap.Text);
                        decimal donGia = decimal.Parse(tbDonGiaNhap.Text);
                        decimal thanhTienNhap = soLuong * donGia;
                        chiTiet.Element("ThanhTienNhap")?.SetValue(thanhTienNhap.ToString());

                        chiTietPhieuNhapXml.Save(chiTietPhieuNhapFilePath);

                        
                        UpdateTongTien(tbIDPhieuNhap.Text);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy chi tiết phiếu nhập với mã phiếu nhập và mã sách đã nhập.");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("File XML 'CHITIETPHIEUNHAP.xml' không tồn tại.");
                    return;
                }

                MessageBox.Show("Cập nhật phiếu nhập và chi tiết phiếu nhập thành công!");
                LoadPhieuNhapData();
                LoadChiTietPhieuNhapData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu chưa chọn hóa đơn nào
                if (string.IsNullOrEmpty(tbIDPhieuNhap.Text))
                {
                    MessageBox.Show("Vui lòng chọn phiếu nhập cần xóa.");
                    return;
                }

                string idPhieuNhap = tbIDPhieuNhap.Text;

                // Kiểm tra và xử lý file HOADON.xml
                if (File.Exists(phieuNhapFilePath))
                {
                    XElement phieuNhapXml = XElement.Load(phieuNhapFilePath);

                    // Tìm và xóa hóa đơn trong HOADON.xml
                    XElement phieuNhapToDelete = phieuNhapXml.Elements("PHIEUNHAP")
                        .FirstOrDefault(x => x.Element("IDPhieuNhap")?.Value == idPhieuNhap);

                    if (phieuNhapToDelete != null)
                    {
                        phieuNhapToDelete.Remove();
                        phieuNhapXml.Save(phieuNhapFilePath);
                    }
                }

                // Kiểm tra và xử lý file CHITIETHOADON.xml
                if (File.Exists(chiTietPhieuNhapFilePath))
                {
                    XElement chiTietPhieuNhapXml = XElement.Load(chiTietPhieuNhapFilePath);

                    // Tìm và xóa các chi tiết hóa đơn liên quan
                    var chiTietToDelete = chiTietPhieuNhapXml.Elements("CHITIETPHIEUNHAP")
                        .Where(x => x.Element("IDPhieuNhap")?.Value == idPhieuNhap)
                        .ToList();

                    foreach (var chiTiet in chiTietToDelete)
                    {
                        chiTiet.Remove();
                    }

                    chiTietPhieuNhapXml.Save("CHITIETPHIEUNHAP");
                }

                // Cập nhật lại DataGridView
                LoadPhieuNhapData();
                LoadChiTietPhieuNhapData();

                // Xóa dữ liệu trong các TextBox và ComboBox
                tbIDPhieuNhap.Clear();
                dtpNgayNhap.Value = DateTime.Now;
                cbIDNhaCungCap.SelectedIndex = -1;
                cbIDNhanVien.SelectedIndex = -1;
                cbIDSach.SelectedIndex = -1;
                tbSoLuongNhap.Clear();
                tbDonGiaNhap.Clear();


                MessageBox.Show("Xóa hóa đơn thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa hóa đơn: " + ex.Message);
            }
        }


        private void DongBoDuLieuTuXML()
        {
            string nhaCungCapXmlPath = "NHACUNGCAP.xml";

            // Kiểm tra tệp XML có tồn tại không
            if (!File.Exists(nhaCungCapXmlPath))
            {
                MessageBox.Show("Tệp XML NHACUNGCAP không tồn tại.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Đọc và đồng bộ NHACUNGCAP
                    DongBoNhaCungCap(connection, nhaCungCapXmlPath);

                    MessageBox.Show("Đồng bộ dữ liệu từ XML vào SQL Server thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi trong quá trình đồng bộ: {ex.Message}");
                }
            }
        }




        private void DongBoNhaCungCap(SqlConnection connection, string nhaCungCapXmlPath)
        {
            // Đọc dữ liệu từ NHACUNGCAP.xml
            DataSet dsNhaCungCap = new DataSet();
            dsNhaCungCap.ReadXml(nhaCungCapXmlPath);
            DataTable dtNhaCungCap = dsNhaCungCap.Tables["NHACUNGCAP"];

            if (dtNhaCungCap == null)
            {
                MessageBox.Show("Không có dữ liệu nhà cung cấp trong tệp XML.");
                return;
            }

            var idsInXml = new HashSet<string>();
            foreach (DataRow row in dtNhaCungCap.Rows)
            {
                idsInXml.Add(row["IDNhaCungCap"].ToString());

                // Kiểm tra xem nhà cung cấp đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM NHACUNGCAP WHERE IDNhaCungCap = @IDNhaCungCap";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@IDNhaCungCap", row["IDNhaCungCap"].ToString());

                int count = (int)checkCommand.ExecuteScalar();

                if (count == 0)
                {
                    // Thêm mới nhà cung cấp
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
                    // Cập nhật nhà cung cấp
                    string updateQuery = "UPDATE NHACUNGCAP SET TenNhaCungCap = @TenNhaCungCap, DiaChi = @DiaChi, " +
                                         "SoDienThoai = @SoDienThoai, Email = @Email WHERE IDNhaCungCap = @IDNhaCungCap";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@IDNhaCungCap", row["IDNhaCungCap"].ToString());
                    updateCommand.Parameters.AddWithValue("@TenNhaCungCap", row["TenNhaCungCap"].ToString());
                    updateCommand.Parameters.AddWithValue("@DiaChi", row["DiaChi"].ToString());
                    updateCommand.Parameters.AddWithValue("@SoDienThoai", row["SoDienThoai"].ToString());
                    updateCommand.Parameters.AddWithValue("@Email", row["Email"].ToString());
                    updateCommand.ExecuteNonQuery();
                }
            }

            // Xóa nhà cung cấp không tồn tại trong XML
            string deleteQuery = "DELETE FROM NHACUNGCAP WHERE IDNhaCungCap NOT IN (@NhaCungCapIDs)";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
            deleteCommand.Parameters.AddWithValue("@NhaCungCapIDs", string.Join(",", idsInXml));
            deleteCommand.ExecuteNonQuery();
        }






        private void btnDongBoDuLieu_Click(object sender, EventArgs e)
        {
            DongBoDuLieuTuXML();
        }
    }
}