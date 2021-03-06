﻿/*

The MIT License (MIT)
Copyright (c) 2016 Lentil Sun

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

using System;
using System.Windows.Forms;
using Core;

namespace LentilToolbox
{
    public partial class ftrwin :Form //轴段特征子窗体
    {
        private int rank;
        public Shaftwin Parentwin;
        private int iB_flat=0;
        private double dBeta=0;//轴段螺旋角
        private double dB;//轴段宽度
        private double dd;//轴段直径
        private bool bFace;//轴段锥面朝向
        public ftrwin(Shaftwin oshaftwin)
        {
            InitializeComponent();
            Parentwin = oshaftwin;
        }
        public void initialize()//初始化参数
        {
            ShaftType.Text = "普通轴段";
            GearDir.Text = "(请选择旋向)";
            GearType.Text = "(请选择齿轮类型)";
            GearFace.Text = "(请选择锥面朝向)";
        }

        //--------------------插入删除及排序相关---------------------

        public void RefRank(int a)
        {
            rank=a;
            this.FtrHead.Text = "定义轴特征"+(rank+1);
        }
        private void ADD_Click(object sender, EventArgs e)
        {
            Parentwin.AddAndRefresh(rank+1);
        }
        private void Delete_Click(object sender, EventArgs e)
        {
            Parentwin.DeleteAndRefresh(rank);
        }

        //---------------------下拉选单设置阶段--------------------------

        //可视性控制函数
        private void ShaftType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ShaftType.Text != "(请选择轴段类型)")
            {
                refreshdBAnddd();
                if (ShaftType.Text == "普通轴段")
                {
                    Values_Sha.Visible = true;
                    Values_Cyl.Visible = false;
                    Values_Bev.Visible = false;
                    GearType.Visible = false;
                    GearFace.Visible = false;
                    GearDir.Visible = false;
                    GearType.Text = "(请选择齿轮类型)";
                }
                else
                {
                    if (ShaftType.Text == "圆柱直/斜齿轮")
                    {
                        Values_Sha.Visible = false;
                        Values_Bev.Visible = false;
                        GearType.Visible = true;
                        GearFace.Visible = false;
                    }
                    else
                    {
                        Values_Bev.Visible = true;
                        Values_Sha.Visible = false;
                        Values_Cyl.Visible = false;
                        GearType.Text = "(请选择齿轮类型)";
                        GearFace.Visible = true;
                        GearType.Visible = false;
                        GearDir.Visible = false;
                    }
                }
            }
            else
            {
                GearType.Text = "(请选择齿轮类型)";
                Values_Bev.Visible = false;
                Values_Sha.Visible = false;
                Values_Cyl.Visible = false;
                GearDir.Visible = false;
                GearFace.Visible = false;
                GearType.Visible = false;
            }
        }
        private void GearType_SelectedIndexChanged(object sender, EventArgs e)
        {
            refresh_dBeta();
            if (GearType.Text!="(请选择齿轮类型)")
            {
                if (GearType.Text == "斜齿轮")
                {
                    GearDir.Visible = true;
                    圆柱齿轮螺旋角.Visible = true;
                    L_Beta .Visible = true;
                    Values_Cyl .Height = 200;
                }
                else
                {
                    圆柱齿轮螺旋角.Visible = false;
                    L_Beta.Visible = false;
                    Values_Cyl.Height = 170;
                    GearDir.Visible = false ;
                }
                Values_Cyl.Visible = true;

            }
            else
            {
                Values_Cyl.Visible = false;
                GearDir.Visible = false;
            }
        }
        private void GearDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshiB_flat();
        }
        private void GearFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshbFace();
        }

        //参数更新函数
        public void refreshdBAnddd()//更新轴段基本信息用于绘制预览，每次更新重新绘制预览
        {
            if (ShaftType.Text == "普通轴段")
            {
                dB = 轴段宽度.consult();
                dd = 轴段直径.consult();
            }
            else if(ShaftType.Text== "圆柱直/斜齿轮")
            {
                dd = 圆柱齿轮齿数.consult() * 圆柱齿轮模数.consult() * Math.Cos(dBeta/180*3.14);
                dB = 圆柱齿轮齿宽.consult();
            }
            else if(ShaftType.Text == "圆锥直齿轮")
            {
                dd = 大端模数.consult() * 锥齿轮齿数.consult();
                dB = 齿胚厚.consult() - 齿槽深.consult();
            }
            else
            {
                dd = 0;
                dB = 0;
            }
            Parentwin.regraph();
        }
        public void refresh_dBeta()//更新当前轴段圆柱齿轮螺旋角缓存
        {
            if (GearType.Text == "斜齿轮")
            {
                dBeta = 圆柱齿轮螺旋角.consult();
                refreshdBAnddd();
            }
            else if (GearType.Text == "直齿轮")
            {
                dBeta = 0;
                refreshdBAnddd();
            }
        }
        public void refreshiB_flat()//更新当前轴段圆柱齿轮旋向
        {
            if (GearDir.Text == "(请选择旋向)")
                iB_flat = 0;
            else if (GearDir.Text == "左旋")
                iB_flat = 1;
            else
                iB_flat = 2;
        }
        public void refreshbFace()//更新当前圆锥齿轮锥面朝向
        {
            if (GearFace.Text == "向左")
                bFace = true;
            else
                bFace = false;
        }

        //-----------------------预览图查询参数-------------------
        
        //供Preview类查询用
        public double consultB()
        {
            return dB;
        }
        public double consultd()
        {
            return dd;
        }
        public int consultdir()
        {
            if (GearFace.Text == "向右")
                return 2;
            else if (GearFace.Text == "向左")
                return 1;
            else
                return 0;
        }

        //--------------------------建模阶段------------------------------

        public double modeling(double Height,iPartDoc oiPartDoc)
        {
            if (ShaftType.Text!= "(请选择轴段类型)")
            {
                if(ShaftType.Text == "普通轴段")//普通轴段
                {
                    shaft_section oshaft_section = new shaft_section();
                    oshaft_section.SetValues(dB, 轴段直径.consult());
                    oshaft_section.modeling(Height, oiPartDoc);
                }
                else if (ShaftType.Text== "圆柱直/斜齿轮")//圆柱齿轮
                {
                    cyl_gear ocyl_gear = new cyl_gear();
                    ocyl_gear.SetValues(dB, 圆柱齿轮齿数 .consult(), 圆柱齿轮模数.consult(), 圆柱齿轮压力角.consult(), dBeta, iB_flat);
                    ocyl_gear.modeling(Height, oiPartDoc);
                }
                else//圆锥齿轮
                {
                    bev_gear obev_gear = new bev_gear();
                    if (GearFace.Text == "(请选择锥面朝向)")
                    {
                        MessageBox.Show("未选择锥面朝向");
                        return -1;
                    }
                    obev_gear.SetValues(齿胚厚.consult(), 锥齿轮齿宽.consult(), 齿槽深.consult(), 锥齿轮齿数.consult(), 大端模数.consult(), 锥齿轮压力角.consult(),分锥角.consult(), bFace);
                    obev_gear.modeling(Height, oiPartDoc);
                }
            }
            else
            {
                return -1;
            }
            return dB;
        }
    }
}
