using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fmrkbremove
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // QUAN TRỌNG: Thêm dòng này ở đầu file

// Hàm trả về True nếu là Win 11 24H2 (Build 26100) trở lên
        private bool IsWin11_24H2_OrNewer()
            {
                try
                {
                    // Đường dẫn Registry chứa thông tin phiên bản Windows
                    const string keyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
                    {
                        if (key != null)
                        {
                            // Lấy Build Number hiện tại
                            object currentBuildObj = key.GetValue("CurrentBuild");

                            if (currentBuildObj != null && int.TryParse(currentBuildObj.ToString(), out int buildNumber))
                            {
                                // 26100 là Build của Windows 11 24H2
                                return buildNumber >= 26100;
                            }
                        }
                    }
                }
                catch
                {
                    // Nếu không đọc được Registry vì lý do gì đó, trả về false cho an toàn
                    return false;
                }

                return false;
            }
    private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Win11 KB Checker"; // Tiêu đề cửa sổ
            this.StartPosition = FormStartPosition.CenterScreen;
            btnCheck.PerformClick();

            // Setup nhanh sự kiện phím Enter cho tiện
           // AcceptButton = btnCheck;
            // Kiểm tra ngay khi mở app
            if (!IsWin11_24H2_OrNewer())
            {
                MessageBox.Show(
                     "Current OS is not Windows 11 24H2 or newer!",
                    "ERROR!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);

                // Đóng ứng dụng
                Application.Exit();
            }
        }


// Hàm trả về True nếu là Win 11 24H2 (Build 26100) trở lên

        private void btnCheck_Click(object sender, EventArgs e)
        {
            string input = txtKbInput.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Vui lòng nhập mã KB!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKbInput.Focus();
                return;
            }

            // Chuẩn hóa: Thêm KB nếu thiếu, viết hoa
            string kbID = input.ToUpper().StartsWith("KB") ? input.ToUpper() : "KB" + input;

            lblStatus.Text = "Đang kiểm tra " + kbID + "...";
            lblStatus.ForeColor = Color.Blue;
            Application.DoEvents(); // Giúp giao diện không bị đơ khi đang quét

            bool isInstalled = CheckKB(kbID);

            if (isInstalled)
            {
                lblStatus.Text = $"✅ {kbID} ĐÃ ĐƯỢC CÀI ĐẶT.";
                lblStatus.ForeColor = Color.Green;
                btnFix.Enabled=true;
            }
            else
            {
                lblStatus.Text = $"❌ KHÔNG TÌM THẤY {kbID}.";
                lblStatus.ForeColor = Color.Red;
            }
            btnFix.Enabled = false;
        }

        private bool CheckKB(string hotFixID)
        {
            try
            {
                // Query WMI: Chỉ lấy đúng KB đang tìm, không load hết nên rất nhanh
                string query = $"SELECT HotFixID FROM Win32_QuickFixEngineering WHERE HotFixID = '{hotFixID}'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

                // Nếu Count > 0 nghĩa là có dữ liệu -> Đã cài
                return searcher.Get().Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi WMI: " + ex.Message);
                return false;
            }
        }
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Lấy ngày giờ hiện tại
            DateTime currentTime = DateTime.Now;

            // Định dạng chuỗi hiển thị (ví dụ: Giờ:Phút:Giây)
            string timeString = currentTime.ToString("HH:mm:ss");

            // Gán chuỗi đó cho Label
            this.Text = "Win11 KB Checker" + " - " + timeString;
        }
        // Hàm chạy lệnh PowerShell ẩn (không hiện cửa sổ đen)
        private void RunPowerShell(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "powershell.exe";
            psi.Arguments = $"-ExecutionPolicy Bypass -Command \"{command}\"";
            psi.UseShellExecute = false; // Bắt buộc false để ẩn cửa sổ
            psi.CreateNoWindow = true;   // Ẩn cửa sổ
            psi.Verb = "runas";          // Chạy với quyền Admin (quan trọng)

            using (Process process = Process.Start(psi))
            {
                process.WaitForExit(); // Chờ lệnh chạy xong mới chạy lệnh tiếp theo
            }
        }
        private void btnFix_Click(object sender, EventArgs e)
        {
            // 1. Khóa nút để tránh bấm nhiều lần
            btnFix.Enabled = false;
            btnFix.Text = "Đang sửa lỗi...";

            try
            {
                // --- LỆNH 1: Register MicrosoftWindows.Client.CBS ---
                string cmd1 = @"Add-AppxPackage -Register -Path 'C:\Windows\SystemApps\MicrosoftWindows.Client.CBS_cw5n1h2txyewy\appxmanifest.xml' -DisableDevelopmentMode";
                RunPowerShell(cmd1);

                // --- LỆNH 2: Register Microsoft.UI.Xaml.CBS ---
                string cmd2 = @"Add-AppxPackage -Register -Path 'C:\Windows\SystemApps\Microsoft.UI.Xaml.CBS_8wekyb3d8bbwe\appxmanifest.xml' -DisableDevelopmentMode";
                RunPowerShell(cmd2);

                // --- LỆNH 3: Register MicrosoftWindows.Client.Core ---
                string cmd3 = @"Add-AppxPackage -Register -Path 'C:\Windows\SystemApps\MicrosoftWindows.Client.Core_cw5n1h2txyewy\appxmanifest.xml' -DisableDevelopmentMode";
                RunPowerShell(cmd3);

                MessageBox.Show("Đã thực thi xong các lệnh sửa lỗi!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Mở lại nút
                btnFix.Enabled = true;
                btnFix.Text = "Fix"; // Hoặc tên gốc của nút
            }
        }
    }
}
