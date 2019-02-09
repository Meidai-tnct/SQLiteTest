using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteTest
{
    public partial class Form1 : Form
    {
        Sqlite sqlite;
        bool updatemode = false;

        public Form1()
        {
            InitializeComponent();
            sqlite = new Sqlite("test.db");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            int result = sqlite.ExecSQL("CREATE TABLE IF NOT EXISTS ADDRESS ( ID INTEGER NOT NULL UNIQUE, NAME TEXT NOT NULL, FURIGANA TEXT, PASSWORD TEXT NOT NULL, `UPDATE_DT` TEXT NOT NULL);");
            if (result == -1000) MessageBox.Show("テーブルの作成に失敗しました");
            button2.PerformClick(); // テーブルの表示
            this.MinimumSize = this.Size;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql;
            if (!updatemode)
            {
                // 追加
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
                {
                    MessageBox.Show("未入力の項目があります"); return;
                }
                sql = "INSERT INTO ADDRESS(ID, NAME, FURIGANA, PASSWORD, UPDATE_DT) VALUES(" + textBox1.Text + ", '" + textBox2.Text + "', '" + textBox3.Text + "', '" + textBox4.Text + "', '" + DateTime.Now.ToString() + "');";
            }
            else
            {
                // 更新
                sql = "UPDATE ADDRESS SET NAME = '" + textBox2.Text + "', FURIGANA = '" + textBox3.Text + "', PASSWORD = '" + textBox4.Text + "', UPDATE_DT = '" + DateTime.Now.ToString() + "' WHERE ID= " + textBox1.Text + ";";
            }
            int result = sqlite.ExecSQL(sql);
            if (result == -1000) MessageBox.Show("レコードの" + (updatemode ? "更新" : "作成") + "に失敗しました");
            else
            {
                textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = string.Empty; textBox1.Focus();
                button2.PerformClick(); // テーブルの表示
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = sqlite.PullTable("select * from ADDRESS order by ID;");
            dataGridView1.Columns["PASSWORD"].Visible = false;
            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = string.Empty;
            button1.Text = "作成(&U)"; button3.Text = "検索(&S)"; updatemode = false; textBox1.ReadOnly = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!updatemode)
            {
                // 検索
                dataGridView1.DataSource = sqlite.PullTable("select * from ADDRESS where ID like '%" + textBox1.Text + "%' AND NAME like '%"
                    + textBox2.Text + "%' AND FURIGANA like '%" + textBox3.Text + "%' AND PASSWORD like '%" + textBox4.Text + "%'order by ID;");
                dataGridView1.Columns["PASSWORD"].Visible = false;
            }
            else
            {
                // 削除
                int result = sqlite.ExecSQL("DELETE FROM ADDRESS WHERE ID = " + textBox1.Text + ";");
                if (result == -1000) MessageBox.Show("レコードの削除に失敗しました");
                else button2.PerformClick();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            int id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells["ID"].Value.ToString());
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["ID"].Value.ToString();
            textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["NAME"].Value.ToString();
            textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells["FURIGANA"].Value.ToString();
            textBox4.Text = dataGridView1.Rows[e.RowIndex].Cells["PASSWORD"].Value.ToString();
            dataGridView1.Rows[e.RowIndex].Selected = true;
            button1.Text = "更新(&U)"; button3.Text = "削除(&D)"; updatemode = true; textBox1.ReadOnly = true;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !updatemode)
            {
                button3.PerformClick();
            }
            else if (e.KeyChar != (char)Keys.Back)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "[0-9]"))
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !updatemode)
            {
                button3.PerformClick();
            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            textBox4.Focus();
            textBox4.SelectAll();
        }
    }

}
