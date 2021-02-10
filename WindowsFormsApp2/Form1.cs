using Devart.Data.SqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2 {
  public partial class Form1 : Form {
    public Form1() {
      InitializeComponent();
            
      //DbForge's Connection String
      //string connectionString = "Data Source=dbf-test-sql.database.windows.net;Encrypt=False;Enlist=False;Initial Catalog=Harmsh;Integrated Security=False;User ID=devart.azure@gmail.com;Pooling=False;Transaction Scope Local=True;Authentication=\"Active Directory Interactive\";";

      //SSMS Connection String 
      string connectionString = "Connection Timeout=30;Data Source=dbf-test-sql.database.windows.net;Encrypt=True;Packet Size = 4096;MultipleActiveResultSets=False;Initial Catalog=Harmsh;TrustServerCertificate=False;User ID=devart.azure@gmail.com;Authentication= \"Active Directory Interactive\";";
      using (SqlConnection connection = new SqlConnection(connectionString)) {

        connection.Open();
        
      }


    }
  }
}
