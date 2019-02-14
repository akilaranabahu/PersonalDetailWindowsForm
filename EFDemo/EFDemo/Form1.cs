using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFDemo
{
    public partial class Form1 : Form
    {
        DemoEntities db;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Enabled = false;
            db = new DemoEntities();
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
            customerBindingSource.DataSource = db.Customers.Include("Addresses").ToList();
            showAddress();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void showAddress()
        {
            Customer obj = customerBindingSource.Current as Customer;
            if(obj != null)
            {
                if(obj.Addresses != null)
                {
                    addressBindingSource.DataSource = obj.Addresses.ToList();
                }
            }
        }

        private void dataGridViewCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                showAddress();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = false;
            panel1.Enabled = true;
            txtFullName.Focus();
            Customer c = new Customer();
            db.Customers.Add(c);
            customerBindingSource.Add(c);
            customerBindingSource.MoveLast();
            addressBindingSource.DataSource = null;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnEdit.Enabled = false;
            panel1.Enabled = true;
            txtFullName.Focus();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panel1.Enabled = false;
            btnAdd.Enabled = true;
            btnEdit.Enabled = true;
            customerBindingSource.ResetBindings(false);
            foreach(DbEntityEntry entry in db.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case System.Data.Entity.EntityState.Added:
                        entry.State = System.Data.Entity.EntityState.Detached;
                        break;

                    case System.Data.Entity.EntityState.Modified:
                        entry.State = System.Data.Entity.EntityState.Unchanged;
                        break;

                    case System.Data.Entity.EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                await db.SaveChangeAsync();
                panel1.Enabled = false;
                btnAdd.Enabled = true;
                btnEdit.Enabled = true;
                MessageBox.Show("Your data has been sucessfully saved","Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

        private void btnAddAdress_Click(object sender, EventArgs e)
        {
            Customer c = customerBindingSource.Current as Customer;
            if(c != null)
            {
                if (addressBindingSource.DataSource == null)
                    addressBindingSource.DataSource = c.Addresses.ToList();

                Address a = new Address() { Customer = c };
                addressBindingSource.Add(a);
                db.Addresses.Add(a);
            }
        }

        private void dataGridViewCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewCustomer.Columns[e.ColumnIndex].Name == "Delete")
            {
                if(MessageBox.Show("Are you sure want to delete this record", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    db.Customers.Remove(customerBindingSource.Current as Customer);
                    customerBindingSource.RemoveCurrent();
                }
            }
        }

        private void dataGridViewAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Are you sure want to delete this record", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    db.Addresses.Remove(addressBindingSource.Current as Address);
                    addressBindingSource.RemoveCurrent();
                }
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    customerBindingSource.DataSource = db.Customers.Include("Addresses").ToList();
                    showAddress();
                }
                else
                {
                    var query = from o in db.Customers.Include("Addresses")
                                where o.FullName.Contains(txtSearch.Text)
                                select o;
                    customerBindingSource.DataSource = query.ToList();
                    showAddress();
                }
            }
        }
    }
}
