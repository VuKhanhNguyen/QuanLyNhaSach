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
    public partial class QuanLyHoaDon : Form
    {
        private readonly string connectionString = "Data Source=LAPTOP-Q12JULH6\\KHANHKHIEMTON;Initial Catalog=dbQUANLYNHASACH;Integrated Security=True";
        private readonly string hoaDonFilePath = Path.Combine(Application.StartupPath, "HOADON.xml");
        private readonly string chiTietHoaDonFilePath = Path.Combine(Application.StartupPath, "CHITIETHOADON.xml");
        private readonly string maKhachHangFilePath = Path.Combine(Application.StartupPath, "KHACHHANG.xml");
        private readonly string maSachFilePath = Path.Combine(Application.StartupPath, "SACH.xml");
        private readonly string maNhanVienFilePath = Path.Combine(Application.StartupPath, "NHANVIEN.xml");

        public QuanLyHoaDon()
        {
            InitializeComponent();
            // Tạo file XML nếu chưa tồn tại
            if (!File.Exists(hoaDonFilePath))
            {
                ExportHoaDonToXml();
            }
            if (!File.Exists(chiTietHoaDonFilePath))
            {
                ExportChiTietHoaDonToXml();
            }

            //Tải dữ liệu vào Combobox
            LoadIDKhachHang();
            LoadIDSach();
            LoadIDNhanVien();


            // Tải dữ liệu vào DataGridView
            LoadHoaDonData();
            LoadChiTietHoaDonData();
        }


        private void LoadIDKhachHang()
        {
            if (File.Exists(maKhachHangFilePath))
            {
                XElement maKhachHangXml = XElement.Load(maKhachHangFilePath);
                var ids = maKhachHangXml.Elements("KHACHHANG").Select(x => x.Element("IDKhachHang")?.Value);
                foreach (var id in ids)
                {
                    cbIDKhachHang.Items.Add(id);
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



        private void ExportHoaDonToXml()
        {
            try
            {
                string query = "SELECT * FROM HOADON";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    XElement hoaDonXml = new XElement("HOADONES",
                        dataTable.AsEnumerable().Select(row =>
                            new XElement("HOADON",
                                row.Table.Columns.Cast<DataColumn>().Select(col =>
                                    new XElement(col.ColumnName, row[col])
                                )
                            )
                        )
                    );

                    hoaDonXml.Save(hoaDonFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất dữ liệu HÓA ĐƠN sang XML: " + ex.Message);
            }
        }
        private void ExportChiTietHoaDonToXml()
        {
            try
            {
                string query = "SELECT * FROM CHITIETHOADON";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    XElement chiTietHoaDonXml = new XElement("CHITIETHOADONES",
                        dataTable.AsEnumerable().Select(row =>
                            new XElement("CHITIETHOADON",
                                row.Table.Columns.Cast<DataColumn>().Select(col =>
                                    new XElement(col.ColumnName, row[col])
                                )
                            )
                        )
                    );

                    chiTietHoaDonXml.Save(chiTietHoaDonFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất dữ liệu CHI TIẾT HÓA ĐƠN sang XML: " + ex.Message);
            }
        }

        private void LoadHoaDonData()
        {
            dataGridViewHoaDon.Rows.Clear();

            if (dataGridViewHoaDon.Columns.Count == 0)
            {
                dataGridViewHoaDon.Columns.Add("IDHoaDon", "ID Hóa Đơn");
                dataGridViewHoaDon.Columns.Add("NgayLapHD", "Ngày Lập Hóa Đơn");
                dataGridViewHoaDon.Columns.Add("IDKhachHang", "ID Khách Hàng");
                dataGridViewHoaDon.Columns.Add("IDNhanVien", "ID Nhân Viên");
                dataGridViewHoaDon.Columns.Add("TongTien", "Tổng Tiền");
            }

            XElement hoaDonXml = XElement.Load(hoaDonFilePath);

            foreach (XElement hoaDon in hoaDonXml.Elements("HOADON"))
            {
                int rowIndex = dataGridViewHoaDon.Rows.Add();
                dataGridViewHoaDon.Rows[rowIndex].Cells["IDHoaDon"].Value = hoaDon.Element("IDHoaDon")?.Value;
                dataGridViewHoaDon.Rows[rowIndex].Cells["NgayLapHD"].Value = hoaDon.Element("NgayLapHD")?.Value;
                dataGridViewHoaDon.Rows[rowIndex].Cells["IDKhachHang"].Value = hoaDon.Element("IDKhachHang")?.Value;
                dataGridViewHoaDon.Rows[rowIndex].Cells["IDNhanVien"].Value = hoaDon.Element("IDNhanVien")?.Value;
                dataGridViewHoaDon.Rows[rowIndex].Cells["TongTien"].Value = hoaDon.Element("TongTien")?.Value;
            }
        }

        private void LoadChiTietHoaDonData()
        {
            dataGridViewChiTietHoaDon.Rows.Clear();

            if (dataGridViewChiTietHoaDon.Columns.Count == 0)
            {
                dataGridViewChiTietHoaDon.Columns.Add("IDHoaDon", "ID Hóa Đơn");
                dataGridViewChiTietHoaDon.Columns.Add("IDSach", "ID Sách");
                dataGridViewChiTietHoaDon.Columns.Add("SoLuong", "Số Lượng");
                dataGridViewChiTietHoaDon.Columns.Add("DonGia", "Đơn Giá");
                dataGridViewChiTietHoaDon.Columns.Add("ThanhTien", "Thành Tiền");
            }

            XElement chiTietHoaDonXml = XElement.Load(chiTietHoaDonFilePath);

            foreach (XElement chiTietHoaDon in chiTietHoaDonXml.Elements("CHITIETHOADON"))
            {
                int rowIndex = dataGridViewChiTietHoaDon.Rows.Add();
                dataGridViewChiTietHoaDon.Rows[rowIndex].Cells["IDHoaDon"].Value = chiTietHoaDon.Element("IDHoaDon")?.Value;
                dataGridViewChiTietHoaDon.Rows[rowIndex].Cells["IDSach"].Value = chiTietHoaDon.Element("IDSach")?.Value;
                dataGridViewChiTietHoaDon.Rows[rowIndex].Cells["SoLuong"].Value = chiTietHoaDon.Element("SoLuong")?.Value;
                dataGridViewChiTietHoaDon.Rows[rowIndex].Cells["DonGia"].Value = chiTietHoaDon.Element("DonGia")?.Value;
                dataGridViewChiTietHoaDon.Rows[rowIndex].Cells["ThanhTien"].Value = chiTietHoaDon.Element("ThanhTien")?.Value;
            }
        }

        private void QuanLyHoaDon_Load(object sender, EventArgs e)
        {
            label2.Parent = pictureBox1;
            label2.BackColor = Color.Transparent;
            label7.Parent = pictureBox1;
            label7.BackColor = Color.Transparent;
            label10.Parent = pictureBox1;
            label10.BackColor = Color.Transparent;
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

        private void UpdateTongTien(string idHoaDon)
        {
            //if (File.Exists(hoaDonFilePath) && File.Exists(chiTietHoaDonFilePath))
            //{
            //    XElement hoaDonXml = XElement.Load(hoaDonFilePath);
            //    XElement chiTietHoaDonXml = XElement.Load(chiTietHoaDonFilePath);

            //    // Tính tổng tiền từ chi tiết hóa đơn
            //    decimal tongTien = chiTietHoaDonXml.Elements("CHITIETHOADON")
            //        .Where(x => x.Element("IDHoaDon")?.Value == idHoaDon)
            //        .Sum(x => decimal.Parse(x.Element("ThanhTien")?.Value ?? "0"));

            //    // Cập nhật tổng tiền trong hóa đơn
            //    XElement hoaDon = hoaDonXml.Elements("HOADON")
            //        .FirstOrDefault(x => x.Element("IDHoaDon")?.Value == idHoaDon);
            //    if (hoaDon != null)
            //    {
            //        hoaDon.Element("TongTien")?.SetValue(tongTien.ToString());
            //        hoaDonXml.Save(hoaDonFilePath);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("File XML 'HOADON.xml' hoặc 'CHITIETHOADON.xml' không tồn tại");
            //}
            if (File.Exists(hoaDonFilePath))
            {
                XElement hoaDonXml = XElement.Load(hoaDonFilePath);

                // Tìm hóa đơn cần cập nhật tổng tiền
                XElement hoaDon = hoaDonXml.Elements("HOADON")
                    .FirstOrDefault(x => x.Element("IDHoaDon")?.Value == idHoaDon);

                if (hoaDon != null)
                {
                    // Tính lại tổng tiền từ chi tiết hóa đơn
                    decimal tongTien = 0;
                    var chiTietHoaDons = XElement.Load(chiTietHoaDonFilePath)
                        .Elements("CHITIETHOADON")
                        .Where(x => x.Element("IDHoaDon")?.Value == idHoaDon);

                    foreach (var chiTiet in chiTietHoaDons)
                    {
                        decimal thanhTien = decimal.Parse(chiTiet.Element("ThanhTien")?.Value ?? "0");
                        tongTien += thanhTien;
                    }

                    // Cập nhật tổng tiền vào hóa đơn
                    hoaDon.Element("TongTien")?.SetValue(tongTien.ToString());

                    hoaDonXml.Save(hoaDonFilePath);
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra và xử lý file HOADON.xml
                XElement hoaDonXml;
                if (File.Exists(hoaDonFilePath))
                {
                    hoaDonXml = XElement.Load(hoaDonFilePath);
                }
                else
                {
                    hoaDonXml = new XElement("HOADONES");
                }

                // Kiểm tra nếu IDHoaDon đã tồn tại
                bool existsHoaDon = hoaDonXml.Elements("HOADON").Any(x => x.Element("IDHoaDon")?.Value == tbIDHoaDon.Text);
                if (existsHoaDon)
                {
                    MessageBox.Show("Mã hóa đơn đã tồn tại, vui lòng nhập mã khác.");
                    return;
                }

                // Thêm hóa đơn mới
                XElement newHoaDon = new XElement("HOADON",
                    new XElement("IDHoaDon", tbIDHoaDon.Text),
                    new XElement("NgayLapHD", dtpNgayLapHD.Value.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("IDKhachHang", cbIDKhachHang.SelectedItem?.ToString()),
                    new XElement("IDNhanVien", cbIDNhanVien.SelectedItem?.ToString()),
                    new XElement("TongTien", "0") // Tổng tiền sẽ cập nhật sau
                );
                hoaDonXml.Add(newHoaDon);
                hoaDonXml.Save(hoaDonFilePath);

                // Kiểm tra và xử lý file CHITIETHOADON.xml
                XElement chiTietHoaDonXml;
                if (File.Exists(chiTietHoaDonFilePath))
                {
                    chiTietHoaDonXml = XElement.Load(chiTietHoaDonFilePath);
                }
                else
                {
                    chiTietHoaDonXml = new XElement("CHITIETHOADONES");
                }

                // Kiểm tra nếu cùng IDHoaDon và IDSach đã tồn tại
                bool existsChiTiet = chiTietHoaDonXml.Elements("CHITIETHOADON").Any(x =>
                    x.Element("IDHoaDon")?.Value == tbIDHoaDon.Text &&
                    x.Element("IDSach")?.Value == cbIDSach.SelectedValue?.ToString());
                if (existsChiTiet)
                {
                    MessageBox.Show("Chi tiết hóa đơn với mã sách này đã tồn tại trong hóa đơn hiện tại.");
                    return;
                }

                // Thêm chi tiết hóa đơn mới
                XElement newChiTiet = new XElement("CHITIETHOADON",
                    new XElement("IDHoaDon", tbIDHoaDon.Text),
                    new XElement("IDSach", cbIDSach.SelectedItem?.ToString()),
                    new XElement("SoLuong", tbSoLuong.Text),
                    new XElement("DonGia", tbDonGia.Text),
                    new XElement("ThanhTien", (int.Parse(tbSoLuong.Text) * decimal.Parse(tbDonGia.Text)).ToString())
                );
                chiTietHoaDonXml.Add(newChiTiet);
                chiTietHoaDonXml.Save(chiTietHoaDonFilePath);

                // Cập nhật tổng tiền trong hóa đơn
                UpdateTongTien(tbIDHoaDon.Text);

                // Hiển thị thông báo thành công
                MessageBox.Show("Thêm hóa đơn và chi tiết hóa đơn thành công!");

                // Tải lại dữ liệu lên DataGridView
                LoadHoaDonData();
                LoadChiTietHoaDonData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void dataGridViewHoaDon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewHoaDon.Rows[e.RowIndex];

                tbIDHoaDon.Text = row.Cells["IDHoaDon"].Value.ToString();
                dtpNgayLapHD.Value = DateTime.Parse(row.Cells["NgayLapHD"].Value.ToString());
                cbIDKhachHang.SelectedItem = row.Cells["IDKhachHang"].Value.ToString(); 
                cbIDNhanVien.SelectedItem = row.Cells["IDNhanVien"].Value.ToString(); 
                //tbTongTien.Text = row.Cells["TongTien"].Value.ToString();
            }
        }

        private void dataGridViewChiTietHoaDon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewChiTietHoaDon.Rows[e.RowIndex];

                //tbIDSach.Text = row.Cells["IDSach"].Value.ToString();
                tbIDHoaDon.Text = row.Cells["IDHoaDon"].Value.ToString();
                cbIDSach.SelectedItem = row.Cells["IDSach"].Value.ToString();
                tbSoLuong.Text = row.Cells["SoLuong"].Value.ToString();
                tbDonGia.Text = row.Cells["DonGia"].Value.ToString();
                //tbThanhTien.Text = (int.Parse(tbSoLuong.Text) * decimal.Parse(tbDonGia.Text)).ToString(); 
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            tbIDHoaDon.Clear();
            dtpNgayLapHD.Value = DateTime.Now;
            cbIDKhachHang.SelectedIndex = -1;
            cbIDNhanVien.SelectedIndex = -1;
            cbIDSach.SelectedIndex = -1;
            tbSoLuong.Clear();
            tbDonGia.Clear();           
        }




        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Cập nhật HOADON.xml
                if (File.Exists(hoaDonFilePath))
                {
                    XElement hoaDonXml = XElement.Load(hoaDonFilePath);

                    XElement hoaDon = hoaDonXml.Elements("HOADON")
                        .FirstOrDefault(x => x.Element("IDHoaDon")?.Value == tbIDHoaDon.Text);

                    if (hoaDon != null)
                    {
                        // Cập nhật các trường thông tin của hóa đơn
                        hoaDon.Element("NgayLapHD")?.SetValue(dtpNgayLapHD.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        hoaDon.Element("IDKhachHang")?.SetValue(cbIDKhachHang.SelectedValue?.ToString() ?? "");
                        hoaDon.Element("IDNhanVien")?.SetValue(cbIDNhanVien.SelectedValue?.ToString() ?? "");
                        

                        hoaDonXml.Save(hoaDonFilePath);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy hóa đơn với mã hóa đơn đã nhập.");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("File XML 'HOADON.xml' không tồn tại.");
                    return;
                }

                // 2. Cập nhật CHITIETHOADON.xml
                if (File.Exists(chiTietHoaDonFilePath))
                {
                    XElement chiTietHoaDonXml = XElement.Load(chiTietHoaDonFilePath);

                    // Tìm chi tiết hóa đơn cần cập nhật dựa trên IDHoaDon và IDSach
                    XElement chiTiet = chiTietHoaDonXml.Elements("CHITIETHOADON")
                        .FirstOrDefault(x =>
                            x.Element("IDHoaDon")?.Value == tbIDHoaDon.Text &&
                            x.Element("IDSach")?.Value == cbIDSach.SelectedItem?.ToString()
                        );

                    if (chiTiet != null)
                    {
                        // Cập nhật các trường thông tin của chi tiết hóa đơn
                        chiTiet.Element("SoLuong")?.SetValue(tbSoLuong.Text);
                        chiTiet.Element("DonGia")?.SetValue(tbDonGia.Text);

                        // Tính toán ThanhTien
                        decimal soLuong = decimal.Parse(tbSoLuong.Text);
                        decimal donGia = decimal.Parse(tbDonGia.Text);
                        decimal thanhTien = soLuong * donGia;
                        chiTiet.Element("ThanhTien")?.SetValue(thanhTien.ToString());

                        chiTietHoaDonXml.Save(chiTietHoaDonFilePath);

                        // Cập nhật tổng tiền trong HOADON.xml
                        UpdateTongTien(tbIDHoaDon.Text);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy chi tiết hóa đơn với mã hóa đơn và mã sách đã nhập.");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("File XML 'CHITIETHOADON.xml' không tồn tại.");
                    return;
                }

                MessageBox.Show("Cập nhật hóa đơn và chi tiết hóa đơn thành công!");
                LoadHoaDonData();
                LoadChiTietHoaDonData();
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
                if (string.IsNullOrEmpty(tbIDHoaDon.Text))
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn cần xóa.");
                    return;
                }

                string idHoaDon = tbIDHoaDon.Text;

                // Kiểm tra và xử lý file HOADON.xml
                if (File.Exists(hoaDonFilePath))
                {
                    XElement hoaDonXml = XElement.Load(hoaDonFilePath);

                    // Tìm và xóa hóa đơn trong HOADON.xml
                    XElement hoaDonToDelete = hoaDonXml.Elements("HOADON")
                        .FirstOrDefault(x => x.Element("IDHoaDon")?.Value == idHoaDon);

                    if (hoaDonToDelete != null)
                    {
                        hoaDonToDelete.Remove();
                        hoaDonXml.Save(hoaDonFilePath);
                    }
                }

                // Kiểm tra và xử lý file CHITIETHOADON.xml
                if (File.Exists(chiTietHoaDonFilePath))
                {
                    XElement chiTietHoaDonXml = XElement.Load(chiTietHoaDonFilePath);

                    // Tìm và xóa các chi tiết hóa đơn liên quan
                    var chiTietToDelete = chiTietHoaDonXml.Elements("CHITIETHOADON")
                        .Where(x => x.Element("IDHoaDon")?.Value == idHoaDon)
                        .ToList();

                    foreach (var chiTiet in chiTietToDelete)
                    {
                        chiTiet.Remove();
                    }

                    chiTietHoaDonXml.Save(chiTietHoaDonFilePath);
                }

                // Cập nhật lại DataGridView
                LoadHoaDonData();
                LoadChiTietHoaDonData();

                // Xóa dữ liệu trong các TextBox và ComboBox
                tbIDHoaDon.Clear();
                dtpNgayLapHD.Value = DateTime.Now;
                cbIDKhachHang.SelectedIndex = -1;
                cbIDNhanVien.SelectedIndex = -1;
                cbIDSach.SelectedIndex = -1;
                tbSoLuong.Clear();
                tbDonGia.Clear();


                MessageBox.Show("Xóa hóa đơn thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa hóa đơn: " + ex.Message);
            }
        }


        private void DongBoDuLieuTuXML()
        {
            string hoaDonXmlPath = "HOADON.xml";
            string chiTietHoaDonXmlPath = "CHITIETHOADON.xml";

            // Kiểm tra tệp XML có tồn tại không
            if (!File.Exists(hoaDonXmlPath) || !File.Exists(chiTietHoaDonXmlPath))
            {
                MessageBox.Show("Một hoặc cả hai tệp XML không tồn tại.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Đọc và đồng bộ HOADON
                DongBoHoaDon(connection, hoaDonXmlPath);

                // Đọc và đồng bộ CHITIETHOADON
                DongBoChiTietHoaDon(connection, chiTietHoaDonXmlPath);

                MessageBox.Show("Đồng bộ dữ liệu từ XML vào SQL Server thành công!");
            }
        }

        private void DongBoHoaDon(SqlConnection connection, string hoaDonXmlPath)
        {
            // Đọc dữ liệu từ HOADON.xml
            DataSet dsHoaDon = new DataSet();
            dsHoaDon.ReadXml(hoaDonXmlPath);
            DataTable dtHoaDon = dsHoaDon.Tables["HOADON"];

            if (dtHoaDon == null)
            {
                MessageBox.Show("Không có dữ liệu hóa đơn trong tệp XML.");
                return;
            }

            var idsInXml = new HashSet<string>();
            foreach (DataRow row in dtHoaDon.Rows)
            {
                idsInXml.Add(row["IDHoaDon"].ToString());

                // Kiểm tra xem hóa đơn đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM HOADON WHERE IDHoaDon = @IDHoaDon";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@IDHoaDon", row["IDHoaDon"].ToString());

                int count = (int)checkCommand.ExecuteScalar();

                if (count == 0)
                {
                    // Thêm mới hóa đơn
                    string insertQuery = "INSERT INTO HOADON (IDHoaDon, NgayLapHD, IDKhachHang, IDNhanVien, TongTien) " +
                                         "VALUES (@IDHoaDon, @NgayLapHD, @IDKhachHang, @IDNhanVien, @TongTien)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@IDHoaDon", row["IDHoaDon"].ToString());
                    insertCommand.Parameters.AddWithValue("@NgayLapHD", DateTime.Parse(row["NgayLapHD"].ToString()));
                    insertCommand.Parameters.AddWithValue("@IDKhachHang", row["IDKhachHang"].ToString());
                    insertCommand.Parameters.AddWithValue("@IDNhanVien", row["IDNhanVien"].ToString());
                    insertCommand.Parameters.AddWithValue("@TongTien", decimal.Parse(row["TongTien"].ToString()));
                    insertCommand.ExecuteNonQuery();
                }
                else
                {
                    // Cập nhật hóa đơn
                    string updateQuery = "UPDATE HOADON SET NgayLapHD = @NgayLapHD, IDKhachHang = @IDKhachHang, " +
                                         "IDNhanVien = @IDNhanVien, TongTien = @TongTien WHERE IDHoaDon = @IDHoaDon";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@IDHoaDon", row["IDHoaDon"].ToString());
                    updateCommand.Parameters.AddWithValue("@NgayLapHD", DateTime.Parse(row["NgayLapHD"].ToString()));
                    updateCommand.Parameters.AddWithValue("@IDKhachHang", row["IDKhachHang"].ToString());
                    updateCommand.Parameters.AddWithValue("@IDNhanVien", row["IDNhanVien"].ToString());
                    updateCommand.Parameters.AddWithValue("@TongTien", decimal.Parse(row["TongTien"].ToString()));
                    updateCommand.ExecuteNonQuery();
                }
            }

            // Xóa hóa đơn không tồn tại trong XML
            string deleteQuery = "DELETE FROM HOADON WHERE IDHoaDon NOT IN (@HoaDonIDs)";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
            deleteCommand.Parameters.AddWithValue("@HoaDonIDs", string.Join(",", idsInXml));
            deleteCommand.ExecuteNonQuery();
        }

        private void DongBoChiTietHoaDon(SqlConnection connection, string chiTietHoaDonXmlPath)
        {
            // Đọc dữ liệu từ CHITIETHOADON.xml
            DataSet dsChiTiet = new DataSet();
            dsChiTiet.ReadXml(chiTietHoaDonXmlPath);
            DataTable dtChiTiet = dsChiTiet.Tables["CHITIETHOADON"];

            if (dtChiTiet == null)
            {
                MessageBox.Show("Không có dữ liệu chi tiết hóa đơn trong tệp XML.");
                return;
            }

            var idsInXml = new HashSet<string>();
            foreach (DataRow row in dtChiTiet.Rows)
            {

                // Kiểm tra IDHoaDon có tồn tại trong HOADON không
                string checkHoaDonQuery = "SELECT COUNT(*) FROM HOADON WHERE IDHoaDon = @IDHoaDon";
                SqlCommand checkHoaDonCommand = new SqlCommand(checkHoaDonQuery, connection);
                checkHoaDonCommand.Parameters.AddWithValue("@IDHoaDon", row["IDHoaDon"].ToString());

                int hoaDonCount = (int)checkHoaDonCommand.ExecuteScalar();
                if (hoaDonCount == 0)
                {
                    MessageBox.Show($"Hóa đơn với ID '{row["IDHoaDon"]}' không tồn tại.");
                    continue; // Bỏ qua dòng dữ liệu không hợp lệ
                }
                // Kiểm tra xem chi tiết hóa đơn đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM CHITIETHOADON WHERE IDHoaDon = @IDHoaDon AND IDSach = @IDSach";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@IDHoaDon", row["IDHoaDon"].ToString());
                checkCommand.Parameters.AddWithValue("@IDSach", row["IDSach"].ToString());

                int count = (int)checkCommand.ExecuteScalar();

                if (count == 0)
                {
                    // Thêm mới chi tiết hóa đơn
                    string insertQuery = "INSERT INTO CHITIETHOADON (IDHoaDon, IDSach, SoLuong, DonGia) " +
                                         "VALUES (@IDHoaDon, @IDSach, @SoLuong, @DonGia)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@IDHoaDon", row["IDHoaDon"].ToString());
                    insertCommand.Parameters.AddWithValue("@IDSach", row["IDSach"].ToString());
                    insertCommand.Parameters.AddWithValue("@SoLuong", int.Parse(row["SoLuong"].ToString()));
                    insertCommand.Parameters.AddWithValue("@DonGia", decimal.Parse(row["DonGia"].ToString()));
                    insertCommand.ExecuteNonQuery();
                }
                else
                {
                    // Cập nhật chi tiết hóa đơn
                    string updateQuery = "UPDATE CHITIETHOADON SET SoLuong = @SoLuong, DonGia = @DonGia " +
                                         "WHERE IDHoaDon = @IDHoaDon AND IDSach = @IDSach";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@IDHoaDon", row["IDHoaDon"].ToString());
                    updateCommand.Parameters.AddWithValue("@IDSach", row["IDSach"].ToString());
                    updateCommand.Parameters.AddWithValue("@SoLuong", int.Parse(row["SoLuong"].ToString()));
                    updateCommand.Parameters.AddWithValue("@DonGia", decimal.Parse(row["DonGia"].ToString()));
                    updateCommand.ExecuteNonQuery();
                }
                // Xóa chi tiết hóa đơn không tồn tại trong XML
                string deleteQuery = "DELETE FROM CHITIETHOADON WHERE IDHoaDon NOT IN (@HoaDonIDs)";
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                deleteCommand.Parameters.AddWithValue("@HoaDonIDs", string.Join(",", idsInXml));
                deleteCommand.ExecuteNonQuery();
            }
        }


        private void btnDongBoDuLieu_Click(object sender, EventArgs e)
        {
            DongBoDuLieuTuXML();
        }
    }
}
