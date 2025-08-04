using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.Utilities
{
    public static class NumberToWords
    {
        static readonly string[] ChuSo = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        static readonly string[] DonVi = { "", "nghìn", "triệu", "tỷ" };

        public static string DocTienBangChu(decimal soTien)
        {
            if (soTien == 0)
                return "Không đồng";

            var so = ((long)soTien).ToString().PadLeft(12, '0'); // đảm bảo đủ nhóm
            var ketQua = "";
            bool daCoNhom = false;

            // Tách 4 nhóm: tỷ - triệu - nghìn - đơn vị
            for (int i = 0; i < 4; i++)
            {
                int start = i * 3;
                int value = int.Parse(so.Substring(start, 3));
                if (value != 0)
                {
                    var group = DocNhom3ChuSo(value, daCoNhom);
                    ketQua += group + " " + DonVi[3 - i] + " ";
                    daCoNhom = true;
                }
            }

            ketQua = ketQua.Trim();
            ketQua = char.ToUpper(ketQua[0]) + ketQua.Substring(1);
            return ketQua + " đồng";
        }

        private static string DocNhom3ChuSo(int number, bool batBuocDocLe)
        {
            int tram = number / 100;
            int chuc = (number % 100) / 10;
            int donvi = number % 10;
            string result = "";

            if (tram > 0)
            {
                result += ChuSo[tram] + " trăm";
            }
            else if (batBuocDocLe && (chuc > 0 || donvi > 0))
            {
                result += "không trăm";
            }

            if (chuc > 1)
            {
                result += " " + ChuSo[chuc] + " mươi";
                if (donvi == 1)
                    result += " mốt";
                else if (donvi == 5)
                    result += " lăm";
                else if (donvi > 0)
                    result += " " + ChuSo[donvi];
            }
            else if (chuc == 1)
            {
                result += " mười";
                if (donvi == 5)
                    result += " lăm";
                else if (donvi > 0)
                    result += " " + ChuSo[donvi];
            }
            else if (chuc == 0)
            {
                if (donvi > 0)
                {
                    if (tram != 0)
                        result += " lẻ";

                    if (donvi == 5)
                        result += " lăm";
                    else
                        result += " " + ChuSo[donvi];
                }
            }

            return result.Trim();
        }
    }
}
