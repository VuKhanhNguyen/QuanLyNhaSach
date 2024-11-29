using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace QuanLyNhaSach_Nhom24
{
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
            cbMatKhau.CheckedChanged += cbMatKhau_CheckedChanged;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Form1 changetoform = new Form1();
            changetoform.Show();
            this.Hide();    
        }

        private void cbMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            tbMatKhau.UseSystemPasswordChar = !cbMatKhau.Checked;
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string taiKhoan = tbTaiKhoan.Text.Trim();
            string matKhau = tbMatKhau.Text.Trim();
            if (string.IsNullOrEmpty(taiKhoan) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //string connectionString = "Data Source=LAPTOP-Q12JULH6\\KHANHKHIEMTON;Initial Catalog=dbQUANLYNHASACH;Integrated Security=True";
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    try
            //    {
            //        connection.Open();
            //        string query = "SELECT * FROM NHANVIEN WHERE TaiKhoan = @TaiKhoan AND MatKhau = @MatKhau";

            //        using (SqlCommand command = new SqlCommand(query, connection))
            //        {
            //            // Sử dụng tham số để tránh SQL Injection
            //            command.Parameters.AddWithValue("@TaiKhoan", taiKhoan);
            //            command.Parameters.AddWithValue("@MatKhau", matKhau);

            //            SqlDataReader reader = command.ExecuteReader();

            //            if (reader.HasRows)
            //            {
            //                // Đăng nhập thành công
            //                while (reader.Read())
            //                {
            //                    string idNhanVien = reader["IDNhanVien"].ToString();
            //                    string hoTen = reader["HoTen"].ToString();

            //                    MessageBox.Show($"Chào mừng {hoTen}!", "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                    TrangChu changetoform = new TrangChu(idNhanVien, hoTen);
            //                    changetoform.Show();
            //                    this.Hide();
            //                }
            //            }
            //            else
            //            {
            //                // Đăng nhập thất bại
            //                MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác.", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            }

            //            reader.Close();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            try
            {
                // Đọc file NHANVIEN.xml
                XDocument doc = XDocument.Load("NHANVIEN.xml");

                // Tìm nhân viên có tài khoản và mật khẩu phù hợp
                var nhanVien = doc.Descendants("NHANVIEN")
                                  .Where(x => x.Element("TaiKhoan").Value == taiKhoan && x.Element("MatKhau").Value == matKhau)
                                  .Select(x => new
                                  {
                                      IDNhanVien = x.Element("IDNhanVien")?.Value,
                                      HoTen = x.Element("HoTen")?.Value
                                  })
                                  .FirstOrDefault();

                if (nhanVien != null)
                {
                    // Đăng nhập thành công
                    MessageBox.Show($"Chào mừng {nhanVien.HoTen}!", "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TrangChu changetoform = new TrangChu(nhanVien.IDNhanVien, nhanVien.HoTen);
                    changetoform.Show();
                    this.Hide();
                }
                else
                {
                    // Đăng nhập thất bại
                    MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác.", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý file XML: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void DangNhap_Load(object sender, EventArgs e)
        {

        }
    }
    
}
