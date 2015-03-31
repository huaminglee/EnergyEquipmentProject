﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EntityClassLibrary;
using KuGuanXiTong.Service;
using System.Collections;
using DLLFullPrint;
using CommonMethod;
using System.Linq;

namespace KuGuanXiTong
{
    public partial class Ruku_ShowStockDetail : Form
    {
        List<StockOperationDetail> detaillist = new List<StockOperationDetail>();
        OpStock opp = new OpStock();
        StockOperation sendStocktoshow = new StockOperation();
        List<StockOperationDetail> changeSpd = new List<StockOperationDetail>();
        /// <summary>
        /// 传入先择的Stock
        /// </summary>
        public StockOperation SendStocktoshow
        {
            get { return sendStocktoshow; }
            set { sendStocktoshow = value; }
        }
        public Ruku_ShowStockDetail()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Ruku_ShowStockDetail_Load(object sender, EventArgs e)
        {
            if (Screen.PrimaryScreen.WorkingArea.Width < 800)
                this.Width = (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.6);
            if (SendStocktoshow != null)
            {
                if (SendStocktoshow.IsCanChange == 1)//可以修改
                {
                    this.button5.Visible = true;
                    this.commonDataGridView1.ReadOnly = false;
                }
                IList details = opp.GetOpdetail(SendStocktoshow.Id);

                int num = 1;
                foreach (StockOperationDetail spd in details)
                {
                    detaillist.Add(spd);

                    this.commonDataGridView1.Rows.Add(num, spd.StockId.GoodsBaseInfoID.GoodsClassCode,
                        spd.StockId.GoodsBaseInfoID.GoodsName,
                        spd.GoodsCode,
                        spd.StockId.GoodsBaseInfoID.Specifications,
                        spd.StockId.GoodsBaseInfoID.Material,
                        spd.StockId.GoodsBaseInfoID.Unit,
                        spd.Quantity,
                        spd.TheMoney,
                        spd.Tax,
                        spd.StockId.StorehouseplaceCode,
                        spd.StockId.Quantity);
                    this.commonDataGridView1.Rows[this.commonDataGridView1.Rows.Count - 1].Tag = spd;

                    num++;

                }
            }
        }
        
       
        /// <summary>
        /// 打印按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            PrintDataGridView.PrintTheDataGridView(commonDataGridView1);
        }

        /// <summary>
        /// 按编号查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.commonDataGridView1.Rows.Clear();
            string likeed = this.textBox1.Text.Trim()+"%";
            var resultList = from item in detaillist
                             where item.StockId.GoodsBaseInfoID.GoodsClassCode.StartsWith(this.textBox1.Text.Trim())
                             select item;
            
            var escortList = resultList.ToList();
            
            int num = 1;
            foreach (StockOperationDetail spd in escortList)
            {
                this.commonDataGridView1.Rows.Add(num, spd.StockId.GoodsBaseInfoID.GoodsClassCode,
                    spd.StockId.GoodsBaseInfoID.GoodsName,
                    spd.GoodsCode,
                    spd.StockId.GoodsBaseInfoID.Specifications,
                    spd.StockId.GoodsBaseInfoID.Material,
                    spd.StockId.GoodsBaseInfoID.Unit,
                    spd.Quantity,
                    spd.TheMoney,
                    spd.Tax,
                    spd.StockId.StorehouseplaceCode,
                    spd.StockId.Quantity);
                this.commonDataGridView1.Rows[this.commonDataGridView1.Rows.Count - 1].Tag = spd;

                num++;
            }
        }

        /// <summary>
        /// 显示全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            this.commonDataGridView1.Rows.Clear();
            int num = 1;
            foreach (StockOperationDetail spd in detaillist)
            {
                this.commonDataGridView1.Rows.Add(num, spd.StockId.GoodsBaseInfoID.GoodsClassCode,
                    spd.StockId.GoodsBaseInfoID.GoodsName,
                    spd.GoodsCode,
                    spd.StockId.GoodsBaseInfoID.Specifications,
                    spd.StockId.GoodsBaseInfoID.Material,
                    spd.StockId.GoodsBaseInfoID.Unit,
                    spd.Quantity,
                    spd.TheMoney,
                    spd.Tax,
                    spd.StockId.StorehouseplaceCode,
                    spd.StockId.Quantity);
                this.commonDataGridView1.Rows[this.commonDataGridView1.Rows.Count - 1].Tag = spd;
                num++;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.changeSpd.Count > 0)
            {
                if (MessageBox.Show("提交之后将不能修改，确定要提交吗？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    for (int i = 0; i < changeSpd.Count;i++ )
                    {
                        opp.SaveOrUpdateEntity(changeSpd[i]);
                    }
                    sendStocktoshow.IsCanChange = 0;
                    opp.SaveOrUpdateEntity(sendStocktoshow);
                    this.commonDataGridView1.ReadOnly = true;
                    this.button5.Visible = false;
                }

            }
        }

        private void commonDataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 8 || e.ColumnIndex == 9)
            {
                TestInput test = new TestInput ();
                if (test.TestDecimal(this.commonDataGridView1.Rows[e.RowIndex].Cells[9].Value.ToString()) && test.TestDecimal(this.commonDataGridView1.Rows[e.RowIndex].Cells[10].Value.ToString()))
                {
                    StockOperationDetail cspd = this.commonDataGridView1.Rows[e.RowIndex].Tag as StockOperationDetail;
                    cspd.TheMoney = float.Parse(this.commonDataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString());
                    cspd.Tax = float.Parse(this.commonDataGridView1.Rows[e.RowIndex].Cells[9].Value.ToString());
                    changeSpd.Add(cspd);
                }
                else 
                {
                    MessageBox.Show("您输入的格式有误……");
                }
            }
        }
    }
}
