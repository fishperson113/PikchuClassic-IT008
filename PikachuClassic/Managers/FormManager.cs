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

        public void Initialize()
        {
            // Khởi tạo các thành phần cần thiết
        }

        // Các phương thức điều hướng
        public void NavigateToMainMenu()
        {
            OpenForm(new MainMenu());
        }

        public void NavigateToGame(string gameMode)
        {
            var gameController = new GameForm();
            gameController.SetGameMode(gameMode);
            OpenForm(gameController);
        }

        public void NavigateToSettings()
        {
            //OpenForm(new SettingScreen());
        }

        public void NavigateToGameOver()
        {
            OpenForm(new GameOverScreen());
        }

        public void NavigateToWinScreen()
        {
            OpenForm(new WinScreen());
        }

        public void NavigateToModeSelection()
        {
            OpenForm(new ModeSelectionScreen());
        }

        // Mở một form mới và lưu form cũ vào stack
        public void OpenForm(Form form)
        {
            if (formStack.Count > 0)
            {
                Form currentForm = formStack.Peek();
                currentForm.Hide();
            }
            
            formStack.Push(form);
            form.FormClosed += OnFormClosed;
            form.Show();
        }

        // Quay lại form trước đó
        public void GoBack()
        {
            if (formStack.Count > 1)
            {
                Form currentForm = formStack.Pop();
                currentForm.Close();

                Form previousForm = formStack.Peek();
                previousForm.Show();
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
        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Form closedForm = sender as Form;
            if (formStack.Count == 1 && formStack.Peek() == closedForm)
            {
                Application.Exit();
            }
            else if (formStack.Contains(closedForm))
            {
                formStack = new Stack<Form>(formStack.Where(f => f != closedForm));
            }
        }
    }
}
