using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           OrmLiteConfig.InsertFilter = (dbCmd, row) => {
    var auditRow = row as Customer;
    if (auditRow != null)
        auditRow.CreatedAt = DateTime.UtcNow;
};

              var dbFactory = new OrmLiteConnectionFactory(
                @"Data Source=.\SQLEXPRESS;Initial Catalog=proveormlite1;Integrated Security=True",
                SqlServerDialect.Provider);

              using (IDbConnection db = dbFactory.OpenDbConnection())
            {
                  if(!db.TableExists("customer")) 
                        db.CreateTable<Customer>(true);
                db.Insert(new Customer { Id = Guid.NewGuid(), FirstName = "fname1!", Email =  Guid.NewGuid().ToString()});
                var rows = db.Select<Customer>();
                MessageBox.Show("Count=" + rows.Count);
            }
        }
    }
}
