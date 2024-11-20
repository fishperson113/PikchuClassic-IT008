using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic
{
    public class FormManager
    {
        #region Singleton
        private static FormManager instance;
        public static FormManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FormManager();
                }
                return instance;
            }
        }
        #endregion

        private Stack<Form> formStack;

        // Khởi tạo FormManager
        private FormManager()
        {
            formStack = new Stack<Form>();
        }

        // Mở một form mới và lưu form cũ vào stack
        public void OpenForm(Form newForm)
        {
            if (formStack.Count > 0)
            {
                // Nếu có form đang mở, đóng form hiện tại trước
                formStack.Peek().Hide();
            }

            // Đăng ký sự kiện khi form bị đóng
            newForm.FormClosed += (sender, e) => OnFormClosed(newForm);

            // Hiển thị form mới
            newForm.Show();
            formStack.Push(newForm);
        }

        // Quay lại form trước đó
        public void GoBack()
        {
            if (formStack.Count > 1)
            {
                // Đóng form hiện tại
                formStack.Pop().Close();

                // Hiển thị form trước đó
                formStack.Peek().Show();
            }
            else
            {
                // Nếu không còn form nào trước đó, thoát ứng dụng
                Application.Exit();
            }
        }

        // Đóng tất cả các form (thoát ứng dụng)
        public void CloseAllForms()
        {
            while (formStack.Count > 0)
            {
                formStack.Pop().Close();
            }
        }

        // Xử lý khi form đóng
        private void OnFormClosed(Form form)
        {
            // Khi form đóng, loại bỏ form khỏi stack
            if (formStack.Contains(form))
            {
                formStack = new Stack<Form>(formStack.Where(f => f != form));
            }
        }
    }
}
