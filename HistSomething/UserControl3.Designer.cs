namespace HistSomething
{
    partial class UserControl3
    {
        /// <summary> 
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod wygenerowany przez Projektanta składników

        /// <summary> 
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować 
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.zed1 = new ZedGraph.ZedGraphControl();
            this.zed3 = new ZedGraph.ZedGraphControl();
            this.zed2 = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zed1
            // 
            this.zed1.Location = new System.Drawing.Point(3, 3);
            this.zed1.Name = "zed1";
            this.zed1.ScrollGrace = 0D;
            this.zed1.ScrollMaxX = 0D;
            this.zed1.ScrollMaxY = 0D;
            this.zed1.ScrollMaxY2 = 0D;
            this.zed1.ScrollMinX = 0D;
            this.zed1.ScrollMinY = 0D;
            this.zed1.ScrollMinY2 = 0D;
            this.zed1.Size = new System.Drawing.Size(481, 317);
            this.zed1.TabIndex = 0;
            this.zed1.UseExtendedPrintDialog = true;
            // 
            // zed3
            // 
            this.zed3.Location = new System.Drawing.Point(3, 326);
            this.zed3.Name = "zed3";
            this.zed3.ScrollGrace = 0D;
            this.zed3.ScrollMaxX = 0D;
            this.zed3.ScrollMaxY = 0D;
            this.zed3.ScrollMaxY2 = 0D;
            this.zed3.ScrollMinX = 0D;
            this.zed3.ScrollMinY = 0D;
            this.zed3.ScrollMinY2 = 0D;
            this.zed3.Size = new System.Drawing.Size(481, 317);
            this.zed3.TabIndex = 1;
            this.zed3.UseExtendedPrintDialog = true;
            // 
            // zed2
            // 
            this.zed2.Location = new System.Drawing.Point(490, 3);
            this.zed2.Name = "zed2";
            this.zed2.ScrollGrace = 0D;
            this.zed2.ScrollMaxX = 0D;
            this.zed2.ScrollMaxY = 0D;
            this.zed2.ScrollMaxY2 = 0D;
            this.zed2.ScrollMinX = 0D;
            this.zed2.ScrollMinY = 0D;
            this.zed2.ScrollMinY2 = 0D;
            this.zed2.Size = new System.Drawing.Size(481, 317);
            this.zed2.TabIndex = 2;
            this.zed2.UseExtendedPrintDialog = true;
            // 
            // UserControl3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.zed2);
            this.Controls.Add(this.zed3);
            this.Controls.Add(this.zed1);
            this.Name = "UserControl3";
            this.Size = new System.Drawing.Size(975, 648);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zed1;
        private ZedGraph.ZedGraphControl zed3;
        private ZedGraph.ZedGraphControl zed2;
    }
}
