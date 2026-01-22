namespace OPCUATestClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnConnection = new Button();
            SuspendLayout();
            // 
            // btnConnection
            // 
            btnConnection.Location = new Point(67, 57);
            btnConnection.Name = "btnConnection";
            btnConnection.Size = new Size(109, 23);
            btnConnection.TabIndex = 0;
            btnConnection.Text = "Connection";
            btnConnection.UseVisualStyleBackColor = true;
            btnConnection.Click += btnConnection_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(245, 139);
            Controls.Add(btnConnection);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button btnConnection;
    }
}