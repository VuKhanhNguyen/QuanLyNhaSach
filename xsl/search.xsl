<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" encoding="UTF-8" indent="yes"/>
    <!-- Khai báo bi?n nh?n giá tr? t? JavaScript -->
    <xsl:param name="searchText" />

    <xsl:template match="/">
        <html>
        <head>
            <title>K?t qu? tìm ki?m</title>
            <style>
                table {
                    width: 100%;
                    border-collapse: collapse;
                }
                table, th, td {
                    border: 1px solid black;
                }
                th, td {
                    padding: 10px;
                    text-align: left;
                }
            </style>
        </head>
        <body>
            <h2>K?t qu? tìm ki?m</h2>
            <table>
                <tr>
                    <th>ID Sách</th>
                    <th>Tên Sách</th>
                    <th>Tác Gi?</th>
                    <th>Th? Lo?i</th>
                    <th>Nhà Xu?t B?n</th>
                    <th>N?m Xu?t B?n</th>
                    <th>Giá Nh?p</th>
                    <th>Giá Bán</th>
                    <th>S? L??ng T?n</th>
                </tr>
                <!-- Duy?t qua các ph?n t? SACH và ki?m tra t? khóa tìm ki?m -->
                <xsl:for-each select="/SACHES/SACH[
                    contains(translate(TenSach, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), translate($searchText, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')) 
                    or 
                    contains(translate(TacGia, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), translate($searchText, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'))
                ]">
                    <tr>
                        <td><xsl:value-of select="IDSach" /></td>
                        <td><xsl:value-of select="TenSach" /></td>
                        <td><xsl:value-of select="TacGia" /></td>
                        <td><xsl:value-of select="IDTheLoai" /></td>
                        <td><xsl:value-of select="NhaXuatBan" /></td>
                        <td><xsl:value-of select="NamXuatBan" /></td>
                        <td><xsl:value-of select="GiaNhap" /></td>
                        <td><xsl:value-of select="GiaBan" /></td>
                        <td><xsl:value-of select="SoLuongTon" /></td>
                    </tr>
                </xsl:for-each>
            </table>
        </body>
        </html>
    </xsl:template>
</xsl:stylesheet>