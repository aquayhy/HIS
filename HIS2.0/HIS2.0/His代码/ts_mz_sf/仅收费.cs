using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using TrasenFrame.Classes;
using TrasenClasses.GeneralControls;
using TrasenClasses.GeneralClasses;
using ts_mz_class;
using TrasenFrame.Forms;
using YpClass;
using ts_mzys_class;
using ts_yb_interface;
using ts_pjq;
using TrasenClasses.DatabaseAccess;
using ts_mzys_yjsqd;
using Ts_zyys_jcsq;
using System.Net;
using ts_mzys_blcflr;
using ts_mz_kgl;

namespace ts_mz_sf
{
    public partial class Frmsf_only  : Form
    {
        private Form _mdiParent;
        private MenuTag _menuTag;
        private string _chineseName;
        private DataSet PubDset = new DataSet();
        public struct Cf
        {
            public Guid brxxid;
            public Guid ghxxid;
            public int js;
            public int ksdm;
            public int ysdm;
            public int zxksid;
            public int zyksid;
            public int xmly;
            public long tcid;
            public string fpcode;
            public string tjdxmdm;
            public string cfh;
        }
        public Cf Dqcf = new Cf();
        public DataTable Tab; //所有未收费的处方明细
        public SystemCfg ConfigGhts = new SystemCfg(1007);//挂号有效天数
        public SystemCfg ConfigLgts = new SystemCfg(1047);//留观有效天数
        public SystemCfg ConfigBfp = new SystemCfg(1048);//不分票选项可见性
        public SystemCfg pssf = new SystemCfg(1019);//皮试是否允许收费
        public SystemCfg Config1056 = new SystemCfg(1056);//回车调收银窗
        private SystemCfg cfgsfy = new SystemCfg(3016);
        private SystemCfg cfg3030 = new SystemCfg(3030); //门诊药房划价是否允许无号 Add By Zj 2012-03-06
        public SystemCfg cfg1063 = new SystemCfg(1063);// 是否自动确费 Add By Zj 2012-07-02
        public SystemCfg cfg1065 = new SystemCfg(1065);//门诊本机收费数据下载间隔提醒时间 Add By Zj 2012-10-08
        public SystemCfg cfg1068 = new SystemCfg(1068);//门诊收费是否打印导引单 Add By Zj 2013-01-16
        public SystemCfg cfg1070 = new SystemCfg(1070);//门诊收费界面是否允许修改病人姓名 Add By Zj 2013-01-16
        public SystemCfg cfg1092 = new SystemCfg(1092);//门诊收费是否只允许刷卡消费 0否 1是 默认0 Add By zp 2013-09-22
        public SystemCfg cfg1095 = new SystemCfg(1095);//门诊收费开医技项目保存后是否向医技申请表插入记录0否 1是 默认0 Add By zp 2013-10-09
        public SystemCfg cfg1096 = new SystemCfg(1096);//门诊收费发票是否打印水晶报表 0否 1是 默认0 Add by zp 2013-10-15
        public SystemCfg cfg1097 = new SystemCfg(1097);//医保病人门诊收费是否验证收费项目是否已匹配 规格 医保接口类型id,医保接口类型id Add By Zp 2013-10-23
        public SystemCfg cfg1100 = new SystemCfg(1100);//门诊划价窗口是否启用模板录入功能 0不启用1启用 默认0
        public SystemCfg cfg3039 = new SystemCfg(3039);//是否启动模板自动合并处方 自动合并处方就是如果是西药和成药 自动合在一张处方里 可以将2张模板里的处方合并在一起 0 不启用 1启用 默认为0 Add by zp 2013-11-22
        public SystemCfg cfg3048 = new SystemCfg(3048); //门诊医生站开处方 是否将医嘱化验类型同样的分为一张处方 0 否 1 是 默认为0 
        public SystemCfg cfg1098 = new SystemCfg(1098); //是否允许本地收费机器开放无号 格式:ip地址1,ip地址2 为空则不允许本地收费机器无号 默认为空 Add by zp 2013-11-29
        public SystemCfg cfg1109 = new SystemCfg(1109);
        public SystemCfg kckz = new SystemCfg(3004);
        public SystemCfg cfg1028 = new SystemCfg(1028);
        private DataTable Tbks;//挂号科室数据
        private DataTable Tbys;//挂号医生数据
        public SystemCfg cfg1112 = new SystemCfg(1112);

        private string Bxm = "";//姓名处停留
        private string Bkh = "";//卡号优先获得焦点
        private string Bview = "";//发票预览
        private FrmCard f;//选项卡
        private string sNum = "";//当前单元格的数量
        private string Bkyhlx = ""; //启用卡优惠类型

        private int Xmly = 0;//项目来源　用于控制项目选择范围 0 全部 1 药品  2 项目

        private bool BDelRow = false; //是否正在删除行

        private DataTable tbk = null;//卡信息

        private long Jgbm = 0;//机构编码

        private string Bwh = "false";//是否允许无号 

        private ts_yb_mzgl.BRXX brxx = new ts_yb_mzgl.BRXX();

        private ts_yb_mzgl.CFMX[] cfmx;
        private ts_yb_mzgl.JSXX jsxx;
        private DataTable Ybcard;//医保卡信息
        private DataTable Ybbrxx;//医保病人信息
        private IntPtr Pint;

        private string DqMzh;//当前门诊号
        private bool Scjz = true;
        private SystemCfg _cfg1046;

        //private bool IsUpdateYjghid = false; //是否保存挂号信息后更改处方内的医技申请表记录的挂号信息id为保存后的挂号id (因为开医技项目时是构造出虚拟的挂号id) Modify By zp 2013-10-11
        private Guid UnYjJzid = Guid.Empty; //虚拟构造出的接诊id 用于先保存挂号、接诊记录和医技处方记录用

        private bool IsRowAdd = false; //Add by Zp 2013-10-17
        string bqybjq = ApiFunction.GetIniString("报价器文件路径", "启用报价器", Constant.ApplicationDirectory + "//ClientWindow.ini");
        string bjqxh = ApiFunction.GetIniString("报价器文件路径", "报价器型号", Constant.ApplicationDirectory + "//ClientWindow.ini");

        public Frmsf_only(MenuTag menuTag, string chineseName, Form mdiParent)
        {
            InitializeComponent();
            _menuTag = menuTag;
            _chineseName = chineseName;
            _mdiParent = mdiParent;

            if (_menuTag.Function_Name == "Fun_ts_mz_sf_not1112")////add by jiangzf 因原代码中跟_menuTag有关，所以这里修改Function_Name、MenuName值
            {
                _menuTag.Function_Name = "Fun_ts_mz_sf";
                _menuTag.MenuName = "门诊收费";
            }

            this.Text = _chineseName;
            _cfg1046 = new SystemCfg(1046);
            if (Scjz)
            {
                
                this.backgroundWorker1.RunWorkerAsync();
                Scjz = false;
            }
        }

        public Frmsf_only(MenuTag menuTag, string chineseName, Form mdiParent, Guid ghxxid, RelationalDatabase _DataBase, User _BCurrentUser, Department _BCurrentDept)
        {
            InitializeComponent();
            _menuTag = menuTag;

            _menuTag.DllName = "ts_mz_sf";
            _menuTag.Jgbm = 1000;

            
            _chineseName = chineseName;
            _mdiParent = mdiParent;

            InstanceForm.BDatabase = _DataBase;
            InstanceForm.BCurrentUser = _BCurrentUser;
            InstanceForm.BCurrentDept = _BCurrentDept;


            this.Text = _chineseName;
            _cfg1046 = new SystemCfg(1046);
            if (Scjz)
            {

                this.backgroundWorker1.RunWorkerAsync();
                Scjz = false;
            }
            txtmzh.Text = Fun.GetMzh(ghxxid, InstanceForm.BDatabase);
        }

        private void Frmhjsf_Load(object sender, EventArgs e)
        {

            try
            {
                panel_yanzheng.Height = 0;

                Jgbm = TrasenFrame.Forms.FrmMdiMain.Jgbm;

                //初始化网格，邦定一个空结果集
                Tab = mz_sf.Select_Wsfcf(0, Guid.Empty, Guid.Empty, 0, 0, Guid.Empty, InstanceForm.BDatabase);
                Fun.AddDataTableColumn(Tab, "分方状态", "");//Add by zp 2013-11-24 新增分方状态列
                Fun.AddDataTableColumn(Tab, "自备药", "0");//Add by zp 2013-11-24 新增自备药列
                AddPresc(Tab);
                //挂号科室
                Tbks = Fun.GetGhks(false, InstanceForm.BDatabase);
                //挂号医生
                Tbys = Fun.GetGhys(0, InstanceForm.BDatabase);

                //医保类型
                FunAddComboBox.AddYblx(false, 0, cmbyblx, InstanceForm.BDatabase);
                cmbyblx.SelectedIndex = -1;
                //添加优惠方案
                FunAddComboBox.AddYhfa(0, Guid.Empty, 0, 0, 0, _menuTag.Function_Name, cmbyhlx, InstanceForm.BDatabase);
                this.WindowState = FormWindowState.Maximized;

                //ini文件读取
                Bxm = ApiFunction.GetIniString("划价收费", "姓名处停留", Constant.ApplicationDirectory + "//ClientWindow.ini");
                Bkh = ApiFunction.GetIniString("划价收费", "卡号优先获得焦点", Constant.ApplicationDirectory + "//ClientWindow.ini");
                Bview = ApiFunction.GetIniString("划价收费", "发票预览", Constant.ApplicationDirectory + "//ClientWindow.ini");
                Bwh = new SystemCfg(1017).Config == "1" ? "true" : "false";
                #region 是否允许本地无号 Add by zp 2013-11-06
                if (Bwh == "false")
                {
                    string iparry = this.cfg1098.Config.Trim();
                    string[] ippar = iparry.Split(',');
                    IPAddress[] addressList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
                    for (int k = 0; k < ippar.Length; k++)
                    {
                        if (ippar[k].Trim() == addressList[0].ToString())
                        {
                            Bwh = "true";
                            break;
                        }
                    }
                }
                #endregion

                Bkyhlx = new SystemCfg(1023).Config == "1" ? "true" : "false";
                string Yxfss = ApiFunction.GetIniString("划价收费", "划价优先非实时查询", Constant.ApplicationDirectory + "//ClientWindow.ini");
                string yflx = ApiFunction.GetIniString("划价收费", "划价优先住院药房", Constant.ApplicationDirectory + "//ClientWindow.ini");
                if (yflx.Trim() == "true")
                    this.rdozyyf.Checked = true;
                else
                    this.rdomzyf.Checked = true;
                string xmly = "";
                if (_menuTag.Function_Name.Trim() == "Fun_ts_mz_sf")
                    xmly = ApiFunction.GetIniString("Fun_ts_mz_sf", "项目来源", Constant.ApplicationDirectory + "//ClientWindow.ini");
                else
                    xmly = ApiFunction.GetIniString("Fun_ts_mz_hj", "项目来源", Constant.ApplicationDirectory + "//ClientWindow.ini");
                if (xmly == "0") Xmly = 0;
                if (xmly == "1") Xmly = 1;
                if (xmly == "2") Xmly = 2;


                //是否允许无号 Modify By Tany 2008-12-26
                if (Bwh == "true")
                    butwh.Enabled = true;
                else
                    butwh.Enabled = false;

                //获得可用发票号
                //int err_code; string err_text;
                //DataTable tb = Fun.GetFph(InstanceForm.BCurrentUser.EmployeeId, 1, 1, out err_code, out err_text);
                //if (tb.Rows.Count != 0)
                //    txtfph.Text = Convertor.IsNull(tb.Rows[0]["QZ"], "") + tb.Rows[0]["fph"].ToString().Trim();
                //else
                //    txtfph.Text = "无可用票据";

                if (ConfigBfp.Config == "1")
                    chkbfp.Visible = true;
                else
                    chkbfp.Visible = false;

                if (_menuTag.Function_Name.Trim() == "Fun_ts_mz_hj" || _menuTag.Function_Name.Trim() == "Fun_ts_mz_hj_Lg" || _menuTag.Function_Name.Trim() == "Fun_ts_mz_hj_ypxmhj")
                {
                    if (cfg3030.Config == "0")
                        butwh.Enabled = false;
                    else
                        butwh.Enabled = true;
                    butsf.Enabled = false;
                    butsf.Visible = false;
                    //lblfph.Visible = false;
                    txtfph.Visible = false;
                    //buttzfph.Visible = false;
                    chkbfp.Visible = false;
                }
                if (_menuTag.Function_Name.Trim() == "Fun_ts_mz_hj_Lg")
                    txtys.Enabled = false;

                try
                {
                    mz_sf.CKJZKZ(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase);
                }
                catch (System.Exception err)
                {
                    for (int i = 0; i <= panel1.Controls.Count - 1; i++)
                        panel1.Controls[i].Enabled = false;
                    MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                f = new FrmCard(_menuTag, "", _mdiParent, InstanceForm.BDatabase);
                if (Yxfss.Trim() == "true")
                    f.checkBox1.Checked = true;

                //刷新内存收费项目
                butsxxm_Click(sender, e);

                //卡类型
                FunAddComboBox.AddKlx(false, 0, cmbklx, InstanceForm.BDatabase);

                if (Bkh == "true")
                    txtkh.Focus();
                else
                    txtmzh.Focus();

                #region 报价器窗体加载时候调用 show出图片
                string bqybjq = ApiFunction.GetIniString("报价器文件路径", "启用报价器", Constant.ApplicationDirectory + "//ClientWindow.ini");
                if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
                {
                    try
                    {
                        string bjqxh = ApiFunction.GetIniString("报价器文件路径", "报价器型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
                        ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);
                        call.SetPicture(InstanceForm.BCurrentUser.EmployeeId.ToString());// Add By zp 2013-08-13

                    }
                    catch (Exception ea)
                    {
                        MessageBox.Show("报价器出现异常!原因:" + ea.Message, "提示");
                    }
                }
                #endregion
                //Add By Zj 2012-10-08 本机收费数据下载
                if (cfg1065.Config != "0")
                {
                    string applicationName = "";
                    string link = "";
                    string LinkDBName = "";
                    RelationalDatabase Database = null;
                    ParameterEx[] parameters = new ParameterEx[1];
                    try
                    {
                        applicationName = ApiFunction.GetIniString("SERVER_NAME", "NAME", Constant.ApplicationDirectory + "\\LocalClientConfig.ini");
                        link = TrasenClasses.GeneralClasses.Crypto.Instance().Decrypto(ApiFunction.GetIniString(applicationName, "Link", Constant.ApplicationDirectory + "\\LocalClientConfig.ini"));//Add By Zj 2012-10-07
                        LinkDBName = TrasenClasses.GeneralClasses.Crypto.Instance().Decrypto(ApiFunction.GetIniString(applicationName, "LinkDBName", Constant.ApplicationDirectory + "\\LocalClientConfig.ini"));//Add By Zj 2012-10-07
                        parameters[0].Text = "@servername";
                        parameters[0].Value = "[" + link + "].[" + LinkDBName + "]";

                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show("连接本地数据失败!请打开本地数据库或者尝试联系管理员!" + Ex.Message, "提示");
                    }
                    MessageBox.Show("请仔细核对您的电脑发票号是否和实际发票号一致!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    if (link != "")
                    {
                        Database = ts_Local_Charge.DbHelperSQL.Database();
                        string ssql = "select top 1 operator_type,starttime,DATEDIFF(HOUR,STARTTIME,GETDATE()) as JG from his_log where operator_type='本机收费基础数据下载' order by starttime desc";
                        DataTable tblog = InstanceForm.BDatabase.GetDataTable(ssql);
                        if (tblog.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(tblog.Rows[0]["JG"]) >= Convert.ToInt32(cfg1065.Config))
                            {
                                if (MessageBox.Show("是否下载基础数据到本地以方便意外情况发生能够本地收费?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                {

                                    Database.DoCommand("SP_Data_Update", parameters, 60);
                                    SystemLog systemLog = new SystemLog(-1, FrmMdiMain.CurrentDept.DeptId, FrmMdiMain.CurrentUser.EmployeeId, "本机收费基础数据下载", "本机收费基础数据更新", System.DateTime.Now, 0, "主机名：" + System.Environment.MachineName, -1);
                                    systemLog.Save();
                                    systemLog = null;
                                }
                            }
                        }
                        else//第一次同步
                        {
                            if (MessageBox.Show("是否下载基础数据到本地以方便意外情况发生能够本地收费?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                Database.DoCommand("SP_Data_Update", parameters, 60);
                                SystemLog systemLog = new SystemLog(-1, FrmMdiMain.CurrentDept.DeptId, FrmMdiMain.CurrentUser.EmployeeId, "本机收费基础数据下载", "本机收费基础数据更新", System.DateTime.Now, 0, "主机名：" + System.Environment.MachineName, -1);
                                systemLog.Save();
                                systemLog = null;
                            }
                        }
                    }
                }

                //自动读射频卡，放到load最后，等所有数据加载完成后再来启动外接设备,防止设备过早启动，发生异常后影响数据加载
                string sbxh = ApiFunction.GetIniString("医院健康卡", "设备型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
                ts_Read_hospitalCard.Icall ReadCard = ts_Read_hospitalCard.CardFactory.NewCall(sbxh);
                if (ReadCard != null)
                    ReadCard.AutoReadCard(_menuTag.Function_Name, cmbklx, txtkh);
                Select_Yjxm(); //Add By zp 2013-10-08
                if (cfg1100.Config.Trim() == "0") //Add by zp 2013-11-18
                {
                    存为模板ToolStripMenuItem.Visible = false;
                    ToolStrip_SelectMb.Visible = false;
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            plSyxx.VisibleChanged += new EventHandler( plSyxx_VisibleChanged );

            if(txtmzh.Text.Trim()!="")
                txtmzh_KeyPress(null, new KeyPressEventArgs((char)Keys.Enter));
        }


        //新增医技项目内存表 Add By Zp 2013-10-08
        private void Select_Yjxm()
        {
            try
            {
                DataTable tb = ts_mz_class.Fun.GetYJItemInfo(InstanceForm.BDatabase);
                 if (PubDset.Tables.Contains("ITEM_YJ"))
                     PubDset.Tables.Remove("ITEM_YJ");
                 PubDset.Tables.Add(tb);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        void plSyxx_VisibleChanged( object sender, EventArgs e )
        {
            if (plSyxx.Visible)
            {
                plSyxx.Left = ( plSyxx.Parent.Width - plSyxx.Width ) / 2;
                plSyxx.Top = ( plSyxx.Parent.Height - plSyxx.Height ) / 2;
            }
        }

        //窗体键盘事件
        private void Frmhjsf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F1 && buthelp.Enabled == true)
                buthelp_Click(sender, e);

            if (e.KeyCode == Keys.F2 && butsave.Enabled == true)
            {
                butsave_Click(sender, e);
            }
            if (e.KeyCode == Keys.F3 && butnew.Enabled == true)
            {
                butnew_Click(sender, e);
            }
            if (e.KeyCode == Keys.F4 && butref.Enabled == true)
            {
                butref_Click(sender, e);
            }
            if (e.KeyCode == Keys.F5 && butsxxm.Enabled == true)
            {
                butsxxm_Click(sender, e);
            }
            if (e.KeyCode == Keys.F7 && butreadcard.Enabled == true)
            {
                butreadcard_Click(sender, e);
            }
            if (e.KeyCode == Keys.F8 && butsf.Enabled == true)
            {
                butsf_Click(sender, e);
            }
            if (e.KeyCode == Keys.F11 && butwh.Enabled == true)
            {
                butwh_Click(sender, e);
            }

            //if (ModifierKeys == Keys.Alt && e.KeyCode == Keys.D2)
            //{
            //    if (txtkh.Enabled == true) txtkh.Focus(); else txtmzh.Focus();
            //}
            //if (ModifierKeys == Keys.Alt && e.KeyCode == Keys.D1)
            //{
            //    txtmzh.Focus();
            //}
            //if (ModifierKeys == Keys.Alt && e.KeyCode == Keys.C)
            if (e.KeyCode == Keys.F9)
            {
                ClearForm();
                if (txtkh.Enabled == true) txtkh.Focus(); else txtmzh.Focus();
                //txtxm.Focus();
                //txtxm.SelectAll();

                if (_menuTag.Function_Name == "Fun_ts_mz_hj_Lg")
                {
                    txtks.Tag = InstanceForm.BCurrentDept.DeptId.ToString();
                    txtks.Text = InstanceForm.BCurrentDept.DeptName;
                }

                //添加优惠方案
                FunAddComboBox.AddYhfa(Convert.ToInt32(Convertor.IsNull(lblklx.Tag, "0")), new Guid(Convertor.IsNull(lblkh.Tag, Guid.Empty.ToString())), Convert.ToInt32(Convertor.IsNull(lblbrlx.Tag, "0")), Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0")), Convert.ToInt32(Convertor.IsNull(lblhtdwlx.Tag, "0")),_menuTag.Function_Name, cmbyhlx, InstanceForm.BDatabase);


                DataTable tb = (DataTable)this.dataGridView1.DataSource;
                tb.Rows.Clear();
            }

            if (e.KeyCode == Keys.F12)
            {
                for (int i = 0; i <= _mdiParent.MdiChildren.Length - 1; i++)
                {
                    if (_mdiParent.MdiChildren[i].Name == "Frmghdj")
                    {
                        _mdiParent.MdiChildren[i].Activate();
                        _mdiParent.MdiChildren[i].Show();
                    }
                }
            }
        }

        /// <summary>
        ///  新开处方按钮事件            
        /// </summary>  
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butnew_Click(object sender, EventArgs e)
        {
            if (cfg1112.Config == "1") return;
            try
            {
                if (Dqcf.ghxxid == Guid.Empty)
                {
                    if (lblmzh.Text.Trim() == "" || txtxm.Text.Trim() == "" || txtmzh.Text == "")
                    {
                        MessageBox.Show("没有输入病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if ((Convert.ToInt32(txtys.Tag) == 0 || txtys.Text.Trim() == "") && _menuTag.Function_Name != "Fun_ts_mz_hj_Lg")
                {
                    MessageBox.Show("请输入医生");
                    txtys.Focus();
                    return;
                }
                if (Convert.ToInt32(txtks.Tag) == 0 || txtks.Text.Trim() == "")
                {
                    MessageBox.Show("请输入科室");
                    txtks.Focus();
                    return;
                }

                DataTable tb = (DataTable)dataGridView1.DataSource;
                int nrow = tb.Rows.Count - 1;
                if (nrow > tb.Rows.Count - 1 || nrow >= 0)
                {
                    if (Convertor.IsNull(tb.Rows[nrow]["序号"], "") != "小计")
                    {
                        Dqcf.cfh = "New";
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["编码"];
                        dataGridView1.Focus();
                        return;
                    }
                }
                DataRow row = tb.NewRow();
                tb.Rows.Add(row);
                dataGridView1.DataSource = tb;
                Dqcf.cfh = "New";
                //((FrmMdiMain)_mdiParent).sttbpDescription.Text = "";
                this.ToolState_Txt.Text = "";
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["编码"];
                dataGridView1.Focus();
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }



        //添加网格行
        private void Addrow(DataRow ReturnRow)
        {
            DataTable tb = (DataTable)dataGridView1.DataSource;
            //int nrow = dataGridView1.CurrentCell.RowIndex;  //Modify by zp 2013-11-24
            int nrow = dataGridView1.CurrentCell == null ? 0 : dataGridView1.CurrentCell.RowIndex;
            if (tb.Rows.Count == 0)
            {
                DataRow dr = tb.NewRow();
                tb.Rows.Add(dr);
            }
            //Modify by zp end 
            DataRow row = tb.Rows[nrow];

            row["序号"] = "1";
            //如果已有划价ID则不替换
            if (new Guid(Convertor.IsNull(row["HJID"], Guid.Empty.ToString())) == Guid.Empty)
                row["HJID"] = Guid.Empty.ToString();
            row["拼音码"] = ReturnRow["拼音码"];
            row["编码"] = ReturnRow["编码"];
            row["项目名称"] = ReturnRow["品名"];
            row["商品名"] = ReturnRow["商品名"];
            row["规格"] = ReturnRow["规格"];
            row["厂家"] = ReturnRow["厂家"];
            if (Convert.ToDecimal(ReturnRow["单价"]) != 0)
            {
                row["单价"] = Convert.ToDecimal(ReturnRow["单价"]) / Convert.ToDecimal(ReturnRow["单位比例"]);
                row["单价可改"] = false;
            }
            else
            {
                row["单价"] = "";
                row["单价可改"] = true;
            }
            row["批发价"] = Convert.ToDecimal(ReturnRow["批发价"]) / Convert.ToDecimal(ReturnRow["单位比例"]);

            row["剂量"] = "0";
            row["剂量单位"] = "";
            row["剂量单位ID"] = "0";
            row["dwlx"] = "0";
            row["频次"] = "";
            row["频次ID"] = "0";
            row["天数"] = "0";
            row["用法"] = "";
            row["用法ID"] = "0";

            if (Convert.ToString(Convertor.IsNull(ReturnRow["statitem_code"], "")) == "03")
                row["数量"] = "10";
            else
                row["数量"] = "1";
            row["剂数"] = Dqcf.js.ToString();

            row["单位"] = Convertor.IsNull(ReturnRow["最小单位"], "");
            row["YDWBL"] = ReturnRow["单位比例"];

            decimal je = Math.Round(Convert.ToDecimal(Convertor.IsNull(row["单价"], "0")) * Convert.ToDecimal(Convertor.IsNull(row["数量"], "0")) * Convert.ToDecimal(Convertor.IsNull(row["剂数"], "0")), 3);
            row["金额"] = je.ToString("0.000");

            decimal pfje = Math.Round(Convert.ToDecimal(Convertor.IsNull(row["批发价"], "0")) * Convert.ToDecimal(Convertor.IsNull(row["数量"], "0")) * Convert.ToDecimal(Convertor.IsNull(row["剂数"], "0")), 3);
            row["批发金额"] = pfje.ToString("0.000");

            row["统计大项目"] = Convertor.IsNull(ReturnRow["statitem_code"], "");
            row["项目ID"] = ReturnRow["项目id"];
            //如果已有划价明细ID则不替换
            if (Convertor.IsNull(row["HJMXID"], "").Trim() == "99999999")
                row["HJMXID"] = Guid.Empty.ToString();
            if (new Guid(Convertor.IsNull(row["HJMXID"], Guid.Empty.ToString())) == Guid.Empty)
                row["HJMXID"] = Guid.Empty.ToString();
            //row["国家编码"]=ReturnRow[""];
            //row["皮试用药"] = "0";
            row["皮试标志"] = "-1";
            //row["免试标志"] = "0";
            row["嘱托"] = "";
            row["处方分组序号"] = "0";
            row["排序序号"] = "0";
            if (Convertor.IsNull(ReturnRow["执行科室id"], "0") == "0")
            {
                row["执行科室"] = "";
                row["执行科室id"] = 0;
            }
            else
            {
                row["执行科室"] = Convertor.IsNull(ReturnRow["执行科室"], "");
                row["执行科室id"] = Convertor.IsNull(ReturnRow["执行科室id"], "0");
            }
            //row["科室"]=txtks.Text.Trim();
            row["科室ID"] = Dqcf.ksdm.ToString();//Convertor.IsNull(txtks.Tag,"0");
            //row["医生"]=txtys.Text.Trim();
            row["医生ID"] = Dqcf.ysdm;//Convertor.IsNull(txtys.Tag,"0");
            // row["住院科室"]="";
            row["住院科室ID"] = Dqcf.zyksid;
            row["项目来源"] = ReturnRow["项目来源"];
            row["套餐ID"] = Convertor.IsNull(ReturnRow["套餐"], "0");
            row["pshjmxid"] = Guid.Empty.ToString();
            row["yzid"] = ReturnRow["YZID"];
            row["医嘱内容"] = ReturnRow["YZMC"];
            row["byscf"] = "0";
            row["hiscode"] = ReturnRow["hiscode"];
            row["选择"] = true;
            row["修改"] = true;

           
            Dqcf.tcid = Convert.ToInt32(ReturnRow["套餐"]);
            Dqcf.zxksid = Convert.ToInt32(Convertor.IsNull(ReturnRow["执行科室id"], "0"));
            Dqcf.xmly = Convert.ToInt32(Convertor.IsNull(ReturnRow["项目来源"], "0"));
            Dqcf.tjdxmdm = Convert.ToString(Convertor.IsNull(ReturnRow["statitem_code"], ""));
            //Dqcf.fpcode = Convert.ToString(Convertor.IsNull(ReturnRow["code"],""));
            tb.AcceptChanges();
            dataGridView1.DataSource = tb;

            Yblx yblx = new Yblx(Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0")), InstanceForm.BDatabase);
            YBPP(yblx, ReturnRow["hiscode"].ToString(), Convert.ToInt64(Convertor.IsNull(row["项目ID"], "0")), Convert.ToInt32(Convertor.IsNull(row["项目来源"], "0")));
        }

        //清除页面
        private void ClearForm()
        {
            tbk = null;
            plSyxx.Visible = false;
            Dqcf.brxxid = Guid.Empty;
            Dqcf.ghxxid = Guid.Empty;

            lblmzh.Text = "";
            lblkh.Text = "";
            lblkh.Tag = "";
            txtmzh.Text = "";
            txtxm.Text = "";
            txtkh.Text = "";
            lblkye.Text = "";
            lblgzdw.Text = "";
            lbllxdh.Text = "";
            txtys.Text = "";
            txtys.Tag = "0";
            txtks.Text = "";
            txtks.Tag = "0";
            

            lblbrlx.Text = "";
            lblbrlx.Tag = "0";
            lblzyks.Text = "";
            lblzyks.Tag = "0";
            lblmzh.Text = "";
            lblkh.Text = "";
            lblkh.Tag = "";
            lblklx.Text = "";
            lblklx.Tag = "0";

            lblsfzh.Text = "";
            lblybkh.Text = "";
            lblybrylx.Text = "";

            lblypcfs.Visible = false;
            label24.Visible = false;

            lblhtdwlx.Text = "";
            lblhtdwlx.Tag = "0";
            lblhtdw.Text = "";
            lblhtdw.Tag = "0";
            FunAddComboBox.AddYhfa(0, Guid.Empty, 0, 0, 0, _menuTag.Function_Name, cmbyhlx, InstanceForm.BDatabase);

            jsxx.GRZF = 0;
            jsxx.ZHZF = 0;
            jsxx.TCZF = 0;
            jsxx.JSDH = "";
            jsxx.HisJsdid = 0;
            jsxx.HisJsid_Old = 0;
            jsxx.YBZF = 0;
            jsxx.ZJE = 0;

            brxx.BRXXID = Guid.Empty;
            brxx.GHXXID = Guid.Empty;
            brxx.GRBH = "";
            brxx.GSYWSQH = "";
            brxx.GZDW = "";
            brxx.ICD = "";
            brxx.ICDMC = "";
            brxx.JSSJH = "";
            brxx.KH = "";
            brxx.KLX = 0;
            brxx.KYE = "";
            brxx.LXDH = "";
            brxx.RYLB = "";
            brxx.RYLBMC = "";
            brxx.SFZH = "";
            brxx.XB = "";
            brxx.YLBZKKH = "";
            brxx.YLZH = "";
            brxx.YWLX = "";
            brxx.YWLXMC = "";
            brxx.YWSQH = "";
            brxx.YWZLX = "";
            brxx.YWZLXMC = "";

            cmbyblx.SelectedIndex = -1;

            butreadcard.Tag = "";//存储医保卡号
            lbljzh.Text = "";
            lblbrxm_yb.Text = "";
            lblybkye.Text = "";
            cmbtb.Tag = "0";
            cmbtb.DataSource = null;
        }
        //获得病人信息
        private void GetBrxx(string mzh, int klx, string kh, string sfzh, string cbkh)
        {


            DataTable tbmx = (DataTable)dataGridView1.DataSource;
            if(tbmx != null)tbmx.Rows.Clear();

            chkyb.Checked = true;

            if (klx == 0 && kh.Trim() != "") MessageBox.Show("请选择卡类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (klx != 0 && kh.Trim() == "" && mzh.Trim() == "" && sfzh == "" && cbkh == "") return;


            if (mzh.Trim() == "" && kh.Trim() == "" && sfzh == "" && cbkh == "") return;

            if (_menuTag.Function_Name == "Fun_ts_mz_hj_Lg")
            {
                txtks.Tag = InstanceForm.BCurrentDept.DeptId.ToString();
                txtks.Text = InstanceForm.BCurrentDept.DeptName;
            }

            //挂号有效天数
            if (Convertor.IsNumeric(ConfigGhts.Config) == false)
            {
                MessageBox.Show("参数1007的值必须是数值型"); return;
            }
            //if (_menuTag.Function_Name == "Fun_ts_mz_hj_Lg")
            //{

            //}
            string _mzh = Fun.returnMzh(mzh, InstanceForm.BDatabase);

            string _kh = kh.Trim() == "" ? "" : Fun.returnKh(klx, kh, InstanceForm.BDatabase);

            ReadCard readcard;
            readcard = new ReadCard(Guid.Empty, InstanceForm.BDatabase);

            //查询病人信息
            if (sfzh != "" || cbkh != "")
            {
                string ssSQ = "select * from yy_brxx a left join yy_kdjb b on a.brxxid=b.brxxid  and b.zfbz=0 where a.brxxid is not null ";
                if (sfzh != "") ssSQ = ssSQ + " and sfzh='" + sfzh + "'";
                if (cbkh != "") ssSQ = ssSQ + " and cbkh='" + cbkh + "'";
                DataTable tbyy = InstanceForm.BDatabase.GetDataTable(ssSQ);
                if (tbyy.Rows.Count != 0)
                {
                    Dqcf.brxxid = new Guid(tbyy.Rows[0]["brxxid"].ToString());
                    klx = Convert.ToInt32(Convertor.IsNull(tbyy.Rows[0]["klx"], "0"));
                    _kh = Convertor.IsNull(tbyy.Rows[0]["kh"], "");
                    kh = Convertor.IsNull(tbyy.Rows[0]["kh"], "");
                }
            }

            if (kh.Trim() != "")
            {
                string ssq = "";
                ssq = "select * from YY_KDJB   where klx=" + klx + " and kh='" + _kh.Trim() + "'  and ZFBZ=0 ";
                tbk = InstanceForm.BDatabase.GetDataTable(ssq);
                if (tbk.Rows.Count != 0)
                    readcard = new ReadCard(new Guid(tbk.Rows[0]["kdjid"].ToString()), InstanceForm.BDatabase);

                if (tbk.Rows.Count == 0)
                {
                    MessageBox.Show("没有找到卡信息，请确认卡号是否正确或卡没有作废");
                    return;
                }
                if (tbk.Rows.Count > 1)
                {
                    MessageBox.Show("找到多张同时有效的卡,请和系统管理员联系");
                    return;
                }
                if (readcard.sdbz == 1)
                {
                    MessageBox.Show("这张卡已被冻结,不能消费.请先激活", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (readcard.sdbz == 2)
                {
                    MessageBox.Show("这张卡已被挂失,不能消费.请先激活", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                txtkh.Text = tbk.Rows[0]["kh"].ToString();
                lblkh.Text = tbk.Rows[0]["kh"].ToString();
                lblklx.Text = Fun.SeekKlxmc(Convert.ToInt32(tbk.Rows[0]["klx"]), InstanceForm.BDatabase);
                lblklx.Tag = tbk.Rows[0]["klx"].ToString();
                lblkye.Text = tbk.Rows[0]["kye"].ToString();
                txtxm.Text = tbk.Rows[0]["ckrxm"].ToString();
                Dqcf.brxxid = new Guid(tbk.Rows[0]["brxxid"].ToString());

                Getypcfs(new Guid(tbk.Rows[0]["kdjid"].ToString()));
            }



            //查询选择挂号信息
            string ssql = "select (select name from jc_brlx where code=brlx) 病人类型,blh 门诊号,brxm 姓名,dbo.fun_getdeptname(ghks) 挂号科室,ghks,dbo.fun_getempname(ghys) 挂号医生 ,ghys,(select top 1 type_name from jc_doctor_type where type_id=ghjb) 挂号级别,ghsj 挂号时间,zdmc 诊断,dbo.fun_getempname(jzys) 接诊医生,jzys ,dbo.fun_getdeptname(jzks) 接诊科室,jzks,jzsj 接诊时间,ghxxid,a.brxxid,gzdw 工作单位,gzdwdh 联系电话,jtdz 家庭地址,jtdh 家庭电话,brlxfs 本人联系方式,kdjid,brlx,HTDWLX,HTDWID,sfzh 身份证号,cbkh 参保卡号,yb_lx,yb_dylxmc,yb_bzxx,yb_dylx from YY_BRXX a inner join mz_ghxx b on a.brxxid=b.brxxid  where bqxghbz=0 ";
            ssql = ssql + " and ( ghsj>=getdate()-" + ConfigGhts.Config + " or ( blgDJ=1 and ghsj>=getdate()-" + ConfigLgts.Config + ") )";

            if (readcard.kdjid != Guid.Empty)
                ssql = ssql + " and kdjid='" + readcard.kdjid + "' ";
            if (mzh.Trim() != "")
                ssql = ssql + " and blh='" + _mzh + "' ";
            if (sfzh.Trim() != "")
                ssql = ssql + " and sfzh='" + sfzh + "' ";
            if (cbkh.Trim() != "")
                ssql = ssql + " and cbkh='" + cbkh + "' ";
            DataTable tb = (DataTable)InstanceForm.BDatabase.GetDataTable(ssql);
            DataRow row = null;
            if (tb.Rows.Count == 1)
                row = tb.Rows[0];
            if (tb.Rows.Count > 1)
            {
                Frmghjl f = new Frmghjl(_menuTag, _chineseName, _mdiParent);
                tb.TableName = "tb";

                f.dataGridView1.DataSource = tb;

                f.ShowDialog();
                if (f.Bok == false) return;
                row = f.ReturnRow;
            }

            //找不到挂号信息，则找病人信息
            if (tb.Rows.Count == 0 && (Dqcf.brxxid != Guid.Empty))
            {

                ssql = "select (select name from jc_brlx where code=brlx) 病人类型,'' 门诊号,brxm 姓名,'' 挂号科室,0 ghks,'' 挂号医生 ,0 ghys,'' 挂号级别,'' 挂号时间,'' 诊断,'' 接诊医生,0 jzys ,'' 接诊科室,0 jzks,'' 接诊时间,null ghxxid, brxxid,gzdw 工作单位,gzdwdh 联系电话,jtdz 家庭地址,jtdh 家庭电话,brlxfs 本人联系方式,null kdjid,brlx,0 HTDWLX,0 HTDWID,sfzh 身份证号,cbkh 参保卡号,0 yb_lx,'' yb_dylxmc,'' yb_bzxx,'' yb_dylx from YY_BRXX where  brxxid='" + Dqcf.brxxid + "'";
                DataTable tb_brxx = (DataTable)InstanceForm.BDatabase.GetDataTable(ssql);
                if (tb_brxx.Rows.Count != 0)
                    row = tb_brxx.Rows[0];
                else
                    throw new Exception("错误,没有找到病人信息");

                if (Bwh == "false") //
                {
                    MessageBox.Show("病人没有挂号，请挂号后再收费！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (Bkh == "true")
                        txtkh.Focus();
                    else
                        txtmzh.Focus();
                    return;
                }

                if (MessageBox.Show(this, "没有找到病人挂号信息,需要产生一个新的门诊号吗?", "确认", MessageBoxButtons.YesNo) == DialogResult.No) return;
                if (tb.Rows.Count != 0)
                {
                    row["门诊号"] = Fun.GetNewMzh(InstanceForm.BDatabase);
                    txtxm.Focus();
                }
                else
                {
                    txtmzh.Text = Fun.GetNewMzh(InstanceForm.BDatabase);
                    lblmzh.Text = txtmzh.Text;
                    row["门诊号"] = txtmzh.Text;
                    txtxm.Focus();
                    // return;
                }
            }

            if (tb.Rows.Count == 0 && mzh.Trim() != "")
            {
                MessageBox.Show("没有找到病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (Bkh == "true")
                    txtkh.Focus();
                else
                    txtmzh.Focus();
                return;
            }


            //string ss = Convertor.IsNull(row["kdjid"], Guid.Empty.ToString());
            //if (new Guid(Convertor.IsNull(row["kdjid"],Guid.Empty.ToString())) !=Guid.Empty)
            //{
            //    readcard = new ReadCard(new Guid(row["kdjid"].ToString()));

            //}


            if (row == null) return;
            txtmzh.Text = row["门诊号"].ToString();
            lblmzh.Text = row["门诊号"].ToString();
            label5.Tag = row["挂号级别"].ToString();//Add By Zj 2012-04-10
            txtxm.Text = row["姓名"].ToString();

            if (new Guid(Convertor.IsNull(row["kdjid"], Guid.Empty.ToString())) != Guid.Empty && readcard.kdjid == Guid.Empty)
            {
                readcard = new ReadCard(new Guid(row["kdjid"].ToString()), InstanceForm.BDatabase);
            }

            if (readcard.klx > 0)
                cmbklx.SelectedValue = readcard.klx;

            txtkh.Text = readcard.kh.Trim();
            lblkh.Text = readcard.kh.Trim();
            lblkh.Tag = readcard.kdjid.ToString();
            lblklx.Text = Fun.SeekKlxmc(readcard.klx, InstanceForm.BDatabase);
            lblklx.Tag = readcard.klx;

            if (Convert.ToInt32(row["jzys"]) == 0)
            {
                txtys.Text = row["挂号医生"].ToString();
                txtys.Tag = row["ghys"].ToString();
                txtks.Text = row["挂号科室"].ToString();
                txtks.Tag = row["ghks"].ToString();
            }
            else
            {

                if (new SystemCfg(3053).Config == "1")//add By Zj 2013-03-05
                {
                    ssql = "select top 1 JSKSDM,dbo.fun_getDeptname(JSKSDM) 接诊科室,JSYSDM,dbo.fun_getEmpName( JSYSDM) 接诊医生 from MZYS_JZJL where GHXXID='" + row["ghxxid"].ToString() + "' and bscbz=0 and bz<>'转诊' order by DJSJ desc ";
                    DataRow lastdr = InstanceForm.BDatabase.GetDataRow(ssql);

                    txtys.Text = lastdr["接诊医生"].ToString();
                    txtys.Tag = lastdr["JSYSDM"].ToString();
                    txtks.Text = lastdr["接诊科室"].ToString();
                    txtks.Tag = lastdr["JSKSDM"].ToString();
                }
                else
                {
                    txtys.Text = row["接诊医生"].ToString();
                    txtys.Tag = row["jzys"].ToString();
                    txtks.Text = row["接诊科室"].ToString();
                    txtks.Tag = row["jzks"].ToString();
                }
            }

            lblgzdw.Text = row["工作单位"].ToString();
            if (lblgzdw.Text.Trim() == "") lblgzdw.Text = row["家庭地址"].ToString();

            lbllxdh.Text = row["联系电话"].ToString();
            if (lbllxdh.Text.Trim() == "") lbllxdh.Text = row["家庭电话"].ToString();
            if (lbllxdh.Text.Trim() == "") lbllxdh.Text = row["本人联系方式"].ToString();

            lblbrlx.Text = row["病人类型"].ToString();
            lblbrlx.Tag = row["brlx"].ToString();

            lblsfzh.Text = row["身份证号"].ToString();
            lblybkh.Text = row["参保卡号"].ToString();

            if (Convert.ToInt32(row["yb_lx"]) > 0)
            {
                cmbyblx.SelectedValue = row["yb_lx"].ToString();
                lblybrylx.Text = row["yb_dylxmc"].ToString();
                lblybrylx.Tag = row["yb_dylx"].ToString();
            }

            lblhtdwlx.Text = Fun.SeekHtdwLx(Convert.ToInt32(row["HTDWLX"].ToString()), InstanceForm.BDatabase);
            lblhtdwlx.Tag = row["HTDWLX"].ToString();
            lblhtdw.Text = Fun.SeekHtdwMc(Convert.ToInt32(row["HTDWID"].ToString()), InstanceForm.BDatabase);
            lblhtdw.Tag = row["HTDWID"].ToString();

            Dqcf.brxxid = new Guid(row["brxxid"].ToString());
            Dqcf.ghxxid = new Guid(Convertor.IsNull(row["ghxxid"], Guid.Empty.ToString()));

            Tab = null;
            if (_menuTag.Function_Name.Trim() == "Fun_ts_mz_hj")
                Tab = mz_sf.Select_Wsfcf(InstanceForm.BCurrentDept.DeptId, Guid.Empty, Dqcf.ghxxid, 0, 0, Guid.Empty, InstanceForm.BDatabase);

            else
                Tab = mz_sf.Select_Wsfcf(0, Guid.Empty, Dqcf.ghxxid, 0, 0, Guid.Empty, InstanceForm.BDatabase);
            Fun.AddDataTableColumn(Tab, "分方状态", "");
            Fun.AddDataTableColumn(Tab, "自备药", "0");
            AddPresc(Tab);

            //添加优惠方案
            FunAddComboBox.AddYhfa(readcard.klx, readcard.kdjid, Convert.ToInt32(Convertor.IsNull(lblbrlx.Tag, "0")), Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0")), Convert.ToInt32(Convertor.IsNull(lblhtdwlx.Tag, "0")), _menuTag.Function_Name, cmbyhlx, InstanceForm.BDatabase);

            //报价器 姓名
          
            if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
            {
                try
                {
                    ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);
                    call.SetPicture(InstanceForm.BCurrentUser.EmployeeId.ToString());// Add By zp 2013-08-13
                    call.Call(ts_call.DmType.姓名, txtxm.Text.Trim());
                }
                catch (Exception ea)
                {
                    MessageBox.Show("报价器出现异常!原因:" + ea.Message, "提示");
                }
            }



        }

        /// <summary>
        /// 刷新项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butsxxm_Click(object sender, EventArgs e)
        {
            Cursor.Current = PubStaticFun.WaitCursor();
            try
            {
                DataTable tb;



                //tb = Fun.GetXmYp_YZ(1, 0, 0, 0, InstanceForm.BCurrentDept.DeptId, "", "", "", TrasenFrame.Forms.FrmMdiMain.Jgbm, 1, InstanceForm.BDatabase, _menuTag.Function_Name);

                //tb.TableName = "ITEM";
                //if (PubDset.Tables.Contains("ITEM"))
                //    PubDset.Tables.Remove("ITEM");
                //PubDset.Tables.Add(tb);



                if (_menuTag.Function_Name.Trim() == "Fun_ts_mz_hj")
                    tb = Fun.GetXmYp(1, 0, rdozyyf.Checked == true ? 1 : 0, InstanceForm.BCurrentDept.DeptId, InstanceForm.BCurrentDept.DeptId, "", "", "", _menuTag.Function_Name, Jgbm, InstanceForm.BDatabase);
                else
                    tb = Fun.GetXmYp(1, 0, rdozyyf.Checked == true ? 1 : 0, 0, InstanceForm.BCurrentDept.DeptId, "", "", "", _menuTag.Function_Name, Jgbm, InstanceForm.BDatabase);
                tb.TableName = "ITEM";
                if (PubDset.Tables.Contains("ITEM"))
                    PubDset.Tables.Remove("ITEM");
                PubDset.Tables.Add(tb);


                f.Dset = PubDset;
                Cursor.Current = Cursors.Default;
            }
            catch (System.Exception err)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //门诊号回车事件
        private void txtmzh_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == 13)
                {
                    txtmzh.Text = ToDBC(txtmzh.Text);
                    string mzh = txtmzh.Text;
                    ClearForm();
                    GetBrxx(mzh, 0, "", "", "");
                    if (Dqcf.ghxxid == Guid.Empty) { txtmzh.SelectAll(); return; }
                    if (Bxm == "true")
                    {
                        txtxm.Focus();
                        return;
                    }

                    if (Dqcf.ghxxid == Guid.Empty) return;
                    if (txtys.Text.Trim() != "" && Convert.ToInt32(Convertor.IsNull(txtys.Tag, "0")) > 0)
                        butnew_Click(sender, e);
                    else
                        txtys.Focus();
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        //添加未收费的处方
        private void AddPresc(DataTable tb)
        {
            decimal sumje = 0;
            plSyxx.Visible = false;
            DataTable tbmx = tb.Clone();

            string[] GroupbyField = { "HJID" };
            string[] ComputeField = { "金额" };
            string[] CField = { "sum" };
            TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
            xcset.TsDataTable = tb;
            string ss = "";
            if (pssf.Config != "1") ss = " and (皮试标志=0 or 皮试标志=2) and 项目来源=1";
            DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "序号<>'小计'");
            for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
            {
                if (ss != "")
                {
                    DataRow[] rows_ps = tb.Select("HJID='" + tbcf.Rows[i]["hjid"].ToString().Trim() + "'" + ss);
                    if (rows_ps.Length != 0) continue;
                }

                DataRow[] rows = tb.Select("HJID='" + tbcf.Rows[i]["hjid"].ToString().Trim() + "'");
                for (int j = 0; j <= rows.Length - 1; j++)
                {
                    DataRow row = tb.NewRow();
                    row = rows[j];
                    row["序号"] = j + 1;

                    if (row["皮试标志"].ToString() == "0" && row["项目来源"].ToString() == "1") row["项目名称"] = row["项目名称"] + " 【皮试】";
                    if (row["皮试标志"].ToString() == "1") row["项目名称"] = row["项目名称"] + " 【-】";
                    if (row["皮试标志"].ToString() == "2") row["项目名称"] = row["项目名称"] + " 【+】";
                    if (row["皮试标志"].ToString() == "3") row["项目名称"] = row["项目名称"] + " 【免试】";
                    if (row["皮试标志"].ToString() == "9") row["项目名称"] = row["项目名称"] + " 【皮试液】";

                    tbmx.ImportRow(row);
                }
                if (rows.Length > 0)
                {
                    DataRow sumrow = tbmx.NewRow();
                    sumrow["序号"] = "小计";
                    decimal je = Math.Round(Convert.ToDecimal(tbcf.Rows[i]["金额"]), 2);
                    sumrow["金额"] = je.ToString("0.00");
                    sumje = sumje + je;
                    sumrow["hjid"] = tbcf.Rows[i]["hjid"];
                    tbmx.Rows.Add(sumrow);
                }
            }
            tbmx.AcceptChanges();
            dataGridView1.DataSource = tbmx;
            dataGridView1.CurrentCell = null;


        }

        private string unyzid = "0";
        private string unhjxmid = ""; //Add by zp 2013-10-13
        private string unyznr = "";
        private string unbbmc = "";
        private string unxmid = "";
        private string untcid = "";

        // 保存处方
        private void butsave_Click(object sender, EventArgs e)
        {
            
            string _sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
            Guid Jzid = Guid.Empty;
            DataTable tb = (DataTable)dataGridView1.DataSource;
            try
            {
                if (Dqcf.ghxxid == Guid.Empty)
                {
                    if ((txtmzh.Text.Trim() == "" && txtxm.Text.Trim() == "") || lblmzh.Text == "")
                    {
                        MessageBox.Show("没有输入病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                //修改病人姓名
                if (Dqcf.brxxid != Guid.Empty && Dqcf.brxxid != null)
                {
                    string ssql = "select brxm from YY_BRXX where brxxid='" + Dqcf.brxxid + "'";
                    string brxm = Convert.ToString(InstanceForm.BDatabase.GetDataResult(ssql)).Trim();

                    if (txtxm.Text.Trim() != brxm.Trim() && Dqcf.ghxxid == Guid.Empty)
                        Dqcf.brxxid = Guid.Empty;
                    if (txtxm.Text.Trim() != brxm.Trim() && Dqcf.ghxxid != Guid.Empty && Dqcf.ghxxid != null && cfg1070.Config == "0")//Modify By Zj 2013-01-16 增加参数1070控制 是否允许修改姓名
                    {
                        ssql = "update YY_BRXX set brxm='" + txtxm.Text.Trim() + "' where brxxid='" + Dqcf.brxxid + "'";
                        InstanceForm.BDatabase.DoCommand(ssql);
                        MessageBox.Show("病人姓名修改成功");

                        SystemLog systemLog = new SystemLog(-1, InstanceForm.BCurrentDept.DeptId, InstanceForm.BCurrentUser.EmployeeId, "门诊收费修改病人姓名", "通过门诊收费界面将病人原姓名:" + brxm + "修改为 " + txtxm.Text.Trim(), Convert.ToDateTime(_sDate), 0, "主机名：" + System.Environment.MachineName, _menuTag.ModuleId);
                        systemLog.Save();
                        systemLog = null;

                    }
                    else
                        txtxm.Text = brxm.Trim();//Add BY ZJ 2013-01-31 如果不允许修改 就重置为以前的姓名

                }

                //查找接诊ID
                string sql = "select * from mzys_jzjl(nolock) where ghxxid='" + Dqcf.ghxxid + "' and jsksdm=" + Dqcf.ksdm + " and jsysdm=" + Dqcf.ysdm + " and bjsbz=1 and bscbz=0";
                DataTable tbjz = InstanceForm.BDatabase.GetDataTable(sql);
                if (tbjz.Rows.Count > 0)
                    Jzid = new Guid(tbjz.Rows[0]["jzid"].ToString());
                if (Dqcf.ksdm == 0 || Dqcf.ysdm == 0)
                {
                    sql = "select top 1 * from mzys_jzjl(nolock) where ghxxid='" + Dqcf.ghxxid + "' and bjsbz=1 and bscbz=0 order by jssj";
                    tbjz = InstanceForm.BDatabase.GetDataTable(sql);
                    if (tbjz.Rows.Count > 0)
                        Jzid = new Guid(tbjz.Rows[0]["jzid"].ToString());
                }

                //分组处方
                string[] GroupbyField1 = { "HJID" };
                string[] ComputeField1 = { "金额" };
                string[] CField1 = { "sum" };
                TrasenFrame.Classes.TsSet xcset1 = new TrasenFrame.Classes.TsSet();
                xcset1.TsDataTable = tb;
                DataTable tbcf1 = xcset1.GroupTable(GroupbyField1, ComputeField1, CField1, "修改=true and 项目id>0");
                if (tbcf1.Rows.Count == 0) { return; }

                string[] GroupbyField = { "HJID", "科室ID", "医生ID", "执行科室ID", "住院科室ID", "项目来源", "剂数" };
                string[] ComputeField = { "金额" };
                string[] CField = { "sum" };
                TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
                xcset.TsDataTable = tb;
                DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "修改=true and 项目id>0");
                if (tbcf.Rows.Count == 0) { return; }
               
                if (tbcf1.Rows.Count != tbcf.Rows.Count)
                {
                    MessageBox.Show("请检查处方的数据是否正确,可能存在同一张处方有不同的执行科室或不同的医生或不同的开单科室的情况", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //划价窗口
                string _hjck = "";
                //返回变量
                int _err_code = -1;
                string _err_text = "";
                //时间

                //医保类型
                int _yblx = Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0"));

                ReadCard readcard = new ReadCard(Convert.ToInt32(Convertor.IsNull(lblklx.Tag, "0")), txtkh.Text.Trim(), InstanceForm.BDatabase);

                InstanceForm.BDatabase.BeginTransaction();


                ////病人信息保存 Modify By zp 2013-12-09 重构
                if (Dqcf.brxxid == Guid.Empty || Dqcf.brxxid == null)
                    SaveBrxx();
                if(Dqcf.ghxxid==null || Dqcf.ghxxid==Guid.Empty)
                    SaveGhInfo(ref Jzid, _sDate, readcard, _hjck, _err_code, _err_text);
                //if (Dqcf.brxxid == Guid.Empty || Dqcf.brxxid == null)
                //{
                //    int yblx = Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0"));
                //    string ybkh = lblybkh.Text;
                //    YY_BRXX.BrxxDj(Guid.Empty, txtxm.Text.Trim(), "9", "1900-01-01 00:00:00", "", "", "", "", "", "", "", "", "", "", "", lblgzdw.Text, "", "", "", "", lblsfzh.Text, 0, yblx, ybkh, InstanceForm.BCurrentUser.EmployeeId, 0, out Dqcf.brxxid, out _err_code, out _err_text, InstanceForm.BDatabase);
                //    if (Dqcf.brxxid == Guid.Empty || _err_code != 0) throw new Exception(_err_text);
                //}
                //SaveGhInfo(ref Jzid, _sDate, readcard, _hjck, _err_code, _err_text);
                #region ////挂号信息保存 注释 2013-10-11 重构
                //if (Dqcf.ghxxid == Guid.Empty || Dqcf.ghxxid == null)
                //{
                //    int _ghxh = 0;
                //    //Add By Zj 2012-05-15 添加挂号类别
                //    int _ghlb = 1;
                //    if (Fun.SeekDeptType(Convert.ToInt32(Convertor.IsNull(txtks.Tag, "0")), InstanceForm.BDatabase) == "2")
                //        _ghlb = 2;
                //    mz_ghxx.GhxxDj(Guid.Empty, Dqcf.brxxid, _ghlb, readcard.kdjid, lblmzh.Text.Trim(), Convert.ToInt32(Convertor.IsNull(txtks.Tag, "0")), Convert.ToInt32(Convertor.IsNull(txtys.Tag, "0")), 0, 0, InstanceForm.BCurrentUser.EmployeeId, 0, _hjck, ref _ghxh, 0, "", Jgbm, out Dqcf.ghxxid, out _err_code, out _err_text, 0, 0, 0, "", "", "", 0, "", "", _sDate, InstanceForm.BDatabase);
                //    if (Dqcf.ghxxid == Guid.Empty || _err_code != 0) throw new Exception(_err_text);


                //    //产生接诊记录
                //    if (Jzid == Guid.Empty)
                //    {
                //        //Modify By Zj 2012-11-13 对接诊时间进行格式化处理 
                //        mzys_jzjl.jz(Jgbm, Dqcf.ghxxid, Dqcf.ysdm, Dqcf.ksdm, Convert.ToDateTime(_sDate).ToString("yyyy-MM-dd HH:mm:ss"), "非医生工作站产生", out Jzid, out _err_code, out _err_text, 1, InstanceForm.BDatabase);
                //        if (_err_code == -1 || Jzid == Guid.Empty)
                //            throw new Exception(_err_text);

                //        if (IsUpdateYjghid) //Add By zp 2013-10-11
                //        {
                //            mzys_yjsq.UpdateYjGhxxId(UnYjGhid, Jzid, Dqcf.ghxxid, InstanceForm.BDatabase);
                //            IsUpdateYjghid = false;
                //        }
                //    }
                //}
                #endregion
                for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
                {
                    //插入处方头
                    Guid _NewHjid = Guid.Empty;
                    string _mzh = lblmzh.Text.Trim();
                    Guid _hjid = new Guid(Convertor.IsNull(tbcf.Rows[i]["hjid"], Guid.Empty.ToString()));
                    int _ksdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["科室id"], "0"));
                    int _ysdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["医生id"], "0"));
                    int _zxksdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["执行科室id"], "0"));
                    int _zyksdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["住院科室id"], "0"));
                    int _xmly = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["项目来源"], "0"));
                    int _js = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["剂数"], "0"));
                    decimal _cfje = Math.Round(Convert.ToDecimal(Convertor.IsNull(tbcf.Rows[i]["金额"], "0")), 2);

                    //查找当前处方
                    DataRow[] rows = tb.Select("HJID='" + _hjid + "' and 修改=true and 项目id>0 ");

                    mz_hj.SaveCf(_hjid, Dqcf.brxxid, Dqcf.ghxxid, _mzh, _sDate, InstanceForm.BCurrentUser.EmployeeId, _hjck, _ysdm, _ksdm, _zyksdm, _cfje, _zxksdm, 0, _xmly, _js, Jgbm, 0, Jzid, out _NewHjid, out _err_code, out _err_text, InstanceForm.BDatabase);
                    if ((_NewHjid == Guid.Empty && _hjid == Guid.Empty) || _err_code != 0) throw new Exception(_err_text);

                    if (rows == null) throw new Exception("没有找到行，请刷新数据");
                    if (rows.Length == 0 && _hjid != Guid.Empty) throw new Exception("没有找到行，请刷新数据");
                    //插处方明细表
                    Guid _NewHjmxid = Guid.Empty;
                    for (int j = 0; j <= rows.Length - 1; j++)
                    {
                        int _tcid = Convert.ToInt32(Convertor.IsNull(rows[j]["套餐id"], "0"));
                        if (_tcid > 0)
                        {
                            #region 如果是套餐则分解保存
                            DataRow[] tcrow = tb.Select("HJID='" + _hjid + "' and  套餐id=" + _tcid + "");
                            if (tcrow.Length == 0) throw new Exception("查找套餐次数时出错，没有找到匹配的行");
                            if (tcrow.Length > 1) throw new Exception("一张处方上不能同时有两个一样的套餐");
                            int _js_ts = Convert.ToInt32(Convertor.IsNull(rows[j]["数量"], "0"));

                            DataTable Tabtc = null;
                            if (Convertor.IsNull(rows[j]["划价日期"], "") == "")//还没有保存的套餐
                                Tabtc = mz_sf.Select_Wsfcf(0, Guid.Empty, Dqcf.ghxxid, _tcid, _js_ts, Guid.Empty, InstanceForm.BDatabase);
                            else
                                Tabtc = mz_sf.Select_Wsfcf(0, Guid.Empty, Dqcf.ghxxid, _tcid, _js_ts, _hjid, InstanceForm.BDatabase); //已保存的套餐
                            if (Tabtc.Rows.Count == 0) throw new Exception("没有找到套餐的明细");

                            DataRow[] rows_tc = Tabtc.Select();
                            for (int xx = 0; xx <= rows_tc.Length - 1; xx++)
                            {
                               // Guid _NewHjmxid = Guid.Empty;
                                Guid _hjmxid = new Guid(Convertor.IsNull(rows_tc[xx]["hjmxid"], Guid.Empty.ToString()));
                                string _pym = Convertor.IsNull(rows_tc[xx]["拼音码"], "");
                                string _bm = Convertor.IsNull(rows_tc[xx]["编码"], "");
                                string _pm = Convertor.IsNull(rows_tc[xx]["项目名称"], "");
                                string _spm = Convertor.IsNull(rows_tc[xx]["商品名"], "");
                                string _gg = Convertor.IsNull(rows_tc[xx]["规格"], "");
                                string _cj = Convertor.IsNull(rows_tc[xx]["厂家"], "");
                                decimal _dj = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["单价"], "0"));
                                decimal _sl = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["数量"], "0"));
                                string _dw = Convertor.IsNull(rows_tc[xx]["单位"], "");
                                int _ydwbl = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["ydwbl"], "0"));
                                decimal _je = Math.Round(_dj * _sl * _js_ts, 3);
                                string _tjdxmdm = Convertor.IsNull(rows_tc[xx]["统计大项目"], "");
                                long _xmid = Convert.ToInt64(Convertor.IsNull(rows_tc[xx]["项目id"], "0"));
                                //int _bpsyybz = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["皮试用药"], "0"));
                                int _bpsbz = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["皮试标志"], "-1"));
                                //int _bmsbz = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["免试标志"], "0"));
                                decimal _yl = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["剂量"], "0"));
                                string _yldw = Convertor.IsNull(rows_tc[xx]["剂量单位"], "");
                                if (_hjmxid == Guid.Empty || _hjmxid == null)
                                {
                                    _yl = Convert.ToDecimal(Convertor.IsNull(rows[j]["数量"], "0"));
                                    _yldw = Convertor.IsNull(rows[j]["单位"], "");
                                }
                                int _yldwid = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["剂量单位id"], "0"));
                                int _dwlx = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["dwlx"], "0"));
                                int _yfid = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["用法id"], "0"));
                                string _yfmc = Convert.ToString(Convertor.IsNull(rows_tc[xx]["用法"], ""));
                                int _pcid = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["频次id"], "0"));
                                string _pcmc = Convert.ToString(Convertor.IsNull(rows_tc[xx]["频次"], ""));
                                decimal _ts = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["天数"], "0"));
                                if (_ts == 0) _ts = 1;
                                string _zt = Convert.ToString(Convertor.IsNull(rows_tc[xx]["嘱托"], ""));
                                int _fzxh = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["处方分组序号"], "0"));
                                int _pxxh = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["排序序号"], "0"));
                                decimal _pfj = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["批发价"], "0"));
                                decimal _pfje = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["批发金额"], "0"));
                                long _yzid = Convert.ToInt64(Convertor.IsNull(rows[j]["yzid"], "0"));
                                string _yzmc = Convert.ToString(Convertor.IsNull(rows[j]["医嘱内容"], ""));
                                mz_hj.SaveCfmx(_hjmxid.ToString(), _NewHjid.ToString(), _pym.Trim(), _bm.Trim(), _pm.Trim(), _spm.Trim(), _gg.Trim(), _cj.Trim(), _dj, _sl, _dw.Trim(), _ydwbl, _js_ts, _je, _tjdxmdm.Trim(), _xmid, 0, _bpsbz,
                                    Guid.Empty.ToString(), _yl, _yldw, _yldwid, _dwlx, _yfid, _yfmc, _pcid, _pcmc, _ts, _zt, _fzxh, _pxxh, _pfj, _pfje, _tcid, _yzid, _yzmc, out _NewHjmxid, out _err_code, out _err_text, _yblx,"", InstanceForm.BDatabase);
                                if ((_NewHjmxid == Guid.Empty && _hjmxid == Guid.Empty) || _err_code != 0) throw new Exception(_err_text);
                            }
                            #endregion
                        }
                        else
                        {
                            #region 非套餐
                          
                            Guid _hjmxid = new Guid(Convertor.IsNull(rows[j]["hjmxid"], Guid.Empty.ToString()));
                            string _pym = Convertor.IsNull(rows[j]["拼音码"], "");
                            string _bm = Convertor.IsNull(rows[j]["编码"], "");
                            string _pm = Convertor.IsNull(rows[j]["项目名称"], "");
                            string _spm = Convertor.IsNull(rows[j]["商品名"], "");
                            string _gg = Convertor.IsNull(rows[j]["规格"], "");
                            string _cj = Convertor.IsNull(rows[j]["厂家"], "");
                            decimal _dj = Convert.ToDecimal(Convertor.IsNull(rows[j]["单价"], "0"));
                            decimal _sl = Convert.ToDecimal(Convertor.IsNull(rows[j]["数量"], "0"));
                            string _dw = Convertor.IsNull(rows[j]["单位"], "");
                            int _ydwbl = Convert.ToInt32(Convertor.IsNull(rows[j]["ydwbl"], "0"));
                            decimal _je = Convert.ToDecimal(Convertor.IsNull(rows[j]["金额"], "0"));
                            string _tjdxmdm = Convertor.IsNull(rows[j]["统计大项目"], "");
                            long _xmid = Convert.ToInt64(Convertor.IsNull(rows[j]["项目id"], "0"));
                            //int _bpsyybz = Convert.ToInt32(Convertor.IsNull(rows[j]["皮试用药"], "0"));
                            int _bpsbz = Convert.ToInt32(Convertor.IsNull(rows[j]["皮试标志"], "-1"));
                            //int _bmsbz = Convert.ToInt32(Convertor.IsNull(rows[j]["免试标志"], "0"));
                            decimal _yl = Convert.ToDecimal(Convertor.IsNull(rows[j]["剂量"], "0"));
                            string _yldw = Convertor.IsNull(rows[j]["剂量单位"], "");
                            if (_hjmxid == Guid.Empty)
                            {
                                _yl = Convert.ToDecimal(Convertor.IsNull(rows[j]["数量"], "0"));
                                _yldw = Convertor.IsNull(rows[j]["单位"], "");
                            }
                            int _yldwid = Convert.ToInt32(Convertor.IsNull(rows[j]["剂量单位id"], "0"));
                            int _dwlx = Convert.ToInt32(Convertor.IsNull(rows[j]["dwlx"], "0"));
                            int _yfid = Convert.ToInt32(Convertor.IsNull(rows[j]["用法id"], "0"));
                            string _yfmc = Convert.ToString(Convertor.IsNull(rows[j]["用法"], ""));
                            int _pcid = Convert.ToInt32(Convertor.IsNull(rows[j]["频次id"], "0"));
                            string _pcmc = Convert.ToString(Convertor.IsNull(rows[j]["频次"], ""));
                            decimal _ts = Convert.ToDecimal(Convertor.IsNull(rows[j]["天数"], "0"));
                            if (_ts == 0) _ts = 1;
                            string _zt = Convert.ToString(Convertor.IsNull(rows[j]["嘱托"], ""));
                            int _fzxh = Convert.ToInt32(Convertor.IsNull(rows[j]["处方分组序号"], "0"));
                            int _pxxh = Convert.ToInt32(Convertor.IsNull(rows[j]["排序序号"], "0"));
                            decimal _pfj = Convert.ToDecimal(Convertor.IsNull(rows[j]["批发价"], "0"));
                            decimal _pfje = Convert.ToDecimal(Convertor.IsNull(rows[j]["批发金额"], "0"));
                            Guid _pshjmxid = new Guid(Convertor.IsNull(rows[j]["pshjmxid"], Guid.Empty.ToString()));
                            long _yzid = Convert.ToInt64(Convertor.IsNull(rows[j]["yzid"], "0"));
                            string _yzmc = Convert.ToString(Convertor.IsNull(rows[j]["医嘱内容"], ""));
                            mz_hj.SaveCfmx(_hjmxid.ToString(), _NewHjid.ToString(), _pym.Trim(), _bm.Trim(), _pm.Trim(), _spm.Trim(), _gg.Trim(), _cj.Trim(), _dj, _sl, _dw.Trim(), _ydwbl, _js, _je, _tjdxmdm.Trim(), _xmid, 0, _bpsbz,
                                _pshjmxid.ToString(), _yl, _yldw, _yldwid, _dwlx, _yfid, _yfmc, _pcid, _pcmc, _ts, _zt, _fzxh, _pxxh, _pfj, _pfje, 0, _yzid, _yzmc, out _NewHjmxid, out _err_code, out _err_text, _yblx,"", InstanceForm.BDatabase);
                            if ((_NewHjmxid == Guid.Empty && _hjmxid == Guid.Empty) || _err_code != 0) throw new Exception(_err_text);
                            #endregion
                        }

                        if (_xmly == 2 && cfg1095.Config.Trim()=="1" ) //2非药品
                        {
                            #region 插入和更新医技申请
                            long _OrderId = 0;
                            if (unyzid != "0")
                                _OrderId = long.Parse(Convertor.IsNull(unyzid, "0"));
                            else
                                _OrderId = Convert.ToInt64(Convertor.IsNull(rows[j]["yzid"], "0"));
                            DataRow[] rowyj = PubDset.Tables["ITEM_YJ"].Select("order_id=" + _OrderId + "");
                            if (rowyj.Length > 0 )
                            {
                                int _Djlx = Convert.ToInt32(rowyj[0]["ntype"]);
                                object hj_mxid = null;
                                Guid _NewYjsqID = Guid.Empty;
                                hj_mxid = _NewHjmxid;
                                Guid _YjsqID = Guid.Empty;

                                string _bsjc = "";
                                string _lczd = "";
                                string _zysx = "";
                                string _bbmc = Convert.ToString(rowyj[0]["SAMPLE"]);
                                int _jjbz = 0;
                                string _sqnr = Convertor.IsNull(rows[j]["项目名称"], "");
                                string _sqrq = _sDate;
                                int _sqr = _ysdm;
                                int _sqks = _ksdm;

                                if (_tcid < 1) //非套餐
                                {
                                    DataTable tbyj = null;

                                    tbyj = mzys_yjsq.GetYjsqID(_hjid, new Guid(hj_mxid.ToString()), 0, InstanceForm.BDatabase);
                                    if (tbyj.Rows.Count <= 0 && ((!string.IsNullOrEmpty(unhjxmid) && unhjxmid.Trim() != "99999999")))
                                        tbyj = mzys_yjsq.GetYjsqID(_hjid, new Guid(unhjxmid), 0, InstanceForm.BDatabase);

                                    if (tbyj.Rows.Count > 0)
                                        _YjsqID = new Guid(tbyj.Rows[0]["yjsqid"].ToString());
                                }
                                else //如果是套餐 则通过划价id得到处方内的套餐,选择
                                {
                                    _bbmc = rows[0][0].ToString();
                                    string xmid = Convertor.IsNull(rows[j]["项目ID"], "");
                                    _bbmc = Convertor.IsNull(rows[j]["规格"], "");
                                    DataTable tbyj = mzys_yjsq.GetYjsqInfo(_hjid, _OrderId.ToString(), Guid.Empty, _bbmc, InstanceForm.BDatabase);
                                    if(tbyj.Rows.Count>0)
                                        _YjsqID = new Guid(tbyj.Rows[0]["yjsqid"].ToString());       
                                }
                                DataTable dt_jzinfo = mz_sf.GetJzInfo(Dqcf.ghxxid, InstanceForm.BDatabase);
                                Guid _jzid = new Guid(dt_jzinfo.Rows[dt_jzinfo.Rows.Count - 1]["JZID"].ToString());
                                //=_HjMxID==Guid.Empty ? null:_HjMxID;
                                _OrderId = Convert.ToInt64(Convertor.IsNull(rows[j]["yzid"], "0"));
                                mzys_yjsq.Save(_YjsqID, TrasenFrame.Forms.FrmMdiMain.Jgbm, Dqcf.brxxid, Dqcf.ghxxid, _jzid, _mzh, _Djlx, _sqrq, _sqr, _sqks,
                                    _sqnr, Convert.ToDecimal(Convertor.IsNull(rows[j]["单价"], "0")), Convert.ToDecimal(Convertor.IsNull(rows[j]["数量"], "0")),
                                    Convertor.IsNull(rows[j]["剂量单位"], ""), Convert.ToDecimal(Convertor.IsNull(rows[j]["金额"], "0")),
                                    Convert.ToString(Convertor.IsNull(rows[j]["频次"], "")), _bsjc, _lczd, _zxksdm, _bbmc, _zysx, _jjbz, _NewHjid,
                                    _OrderId, hj_mxid, out _NewYjsqID, out _err_code, out _err_text, InstanceForm.BDatabase);
                                if (_NewYjsqID == Guid.Empty || _err_code != 0) throw new Exception(_err_text);
                                /*更新后 将全局变量重新赋值*/
                                //is_xg = false;

                            }
                            #endregion
                        }

             
                    }

                    mz_hj.UpdateHjCfje(_NewHjid, InstanceForm.BDatabase);
                }

                InstanceForm.BDatabase.CommitTransaction();
                butref_Click(sender, e);
                butnew_Click(sender, e);
             
            }
            catch (System.Exception err)
            {
                InstanceForm.BDatabase.RollbackTransaction();
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
         
        }

        private void SaveBrxx()
        {
            //病人信息保存
            if (Dqcf.brxxid == Guid.Empty || Dqcf.brxxid == null)
            {
                int _err_code = -1;
                string _err_text = "";
                int yblx = Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0"));
                string ybkh = lblybkh.Text;
                YY_BRXX.BrxxDj(Guid.Empty, txtxm.Text.Trim(), "9", "1900-01-01 00:00:00", "", "", "", "", "", "", "", "", "", "", "", lblgzdw.Text, "", "", "", "", lblsfzh.Text, 0, yblx, ybkh, InstanceForm.BCurrentUser.EmployeeId, 0, out Dqcf.brxxid, out _err_code, out _err_text, InstanceForm.BDatabase);
                if (Dqcf.brxxid == Guid.Empty || _err_code != 0) 
                    throw new Exception(_err_text);
            }
            //SaveGhInfo(ref Jzid, _sDate, readcard, _hjck, _err_code, _err_text);
        }

        //保存挂号信息、接诊信息 Modify By zp 2013-10-11 重构
        private void SaveGhInfo(ref Guid Jzid, string _sDate, ReadCard readcard, string _hjck, int _err_code, string _err_text)
        {
            try
            {
                //挂号信息保存
                if (Dqcf.ghxxid == Guid.Empty || Dqcf.ghxxid == null)
                {
                    int _ghxh = 0;
                    //Add By Zj 2012-05-15 添加挂号类别
                    int _ghlb = 1;
                    if (Fun.SeekDeptType(Convert.ToInt32(Convertor.IsNull(txtks.Tag, "0")), InstanceForm.BDatabase) == "2")
                        _ghlb = 2;
                    mz_ghxx.GhxxDj(Guid.Empty, Dqcf.brxxid, _ghlb, readcard.kdjid, lblmzh.Text.Trim(), Convert.ToInt32(Convertor.IsNull(txtks.Tag, "0")), Convert.ToInt32(Convertor.IsNull(txtys.Tag, "0")), 0, 0, InstanceForm.BCurrentUser.EmployeeId, 0, _hjck, ref _ghxh, 0, "", Jgbm, out Dqcf.ghxxid, out _err_code, out _err_text, 0, 0, 0, "", "", "", 0, "", "", _sDate, InstanceForm.BDatabase);
                    if (Dqcf.ghxxid == Guid.Empty || _err_code != 0) throw new Exception(_err_text);


                    //产生接诊记录
                    if (Jzid == Guid.Empty)
                    {
                        //Modify By Zj 2012-11-13 对接诊时间进行格式化处理 
                        mzys_jzjl.jz(Jgbm, Dqcf.ghxxid, Dqcf.ysdm, Dqcf.ksdm, Convert.ToDateTime(_sDate).ToString("yyyy-MM-dd HH:mm:ss"), "非医生工作站产生", out Jzid, out _err_code, out _err_text, 1, InstanceForm.BDatabase);
                        if (_err_code == -1 || Jzid == Guid.Empty)
                            throw new Exception(_err_text);
                    }
                }
            }
            catch (Exception ea)
            {
                throw ea;
            }
        }

        #region 网格的处理

        //改变行颜色
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow dgv in dataGridView1.Rows)
            {
                //if (Convert.ToInt64(Convertor.IsNull(dgv.Cells["项目id"].Value, "0")) > 0 && Convert.ToInt64(Convertor.IsNull(dgv.Cells["hjmxid"].Value, "0")) > 0)
                //{
                //    if (Convertor.IsNull(dgv.Cells["byscf"].Value, "0")=="1")
                //        dgv.DefaultCellStyle.BackColor = Color.LightGray;
                //    else
                //        dgv.DefaultCellStyle.BackColor = Color.LemonChiffon;
                //}
                if (Convert.ToInt64(Convertor.IsNull(dgv.Cells["项目id"].Value, "0")) > 0)
                {
                    dgv.DefaultCellStyle.BackColor = Color.LightGray;
                }
                if (Convert.ToString(Convertor.IsNull(dgv.Cells["序号"].Value, "0")) == "小计")
                {
                    dgv.DefaultCellStyle.BackColor = Color.White; ;
                    //System.Drawing.Font f;
                    //f.Size = 14;
                    //f.Bold = true;
                    //dgv.DefaultCellStyle.Font = f;
                    //dgv.DefaultCellStyle.Format = "粗体";
                }

                //if (Convertor.IsNull(dgv.Cells["hjmxid"].Value, Guid.Empty.ToString()) !=Guid.Empty.ToString() && (Convert.ToBoolean(dgv.Cells["修改"].Value) == true))
                //    dgv.DefaultCellStyle.ForeColor = Color.Blue;
                if (Convertor.IsNull(dgv.Cells["hjmxid"].Value, Guid.Empty.ToString()) == Guid.Empty.ToString() || (Convert.ToBoolean(dgv.Cells["修改"].Value) == true))
                    dgv.DefaultCellStyle.ForeColor = Color.Blue;

            }
        }

        //单无格发生改变时
        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (BDelRow == true) return;
                if (dataGridView1.CurrentCell == null) return;
                DataTable tb = (DataTable)dataGridView1.DataSource;
                int nrow = dataGridView1.CurrentCell.RowIndex;
                int ncol = dataGridView1.CurrentCell.ColumnIndex;
                if (nrow > dataGridView1.Rows.Count) return;

                sNum = "";

                mnuAddrow.Enabled = true;
                mnuDelrow.Enabled = true;
                mnuDelPresc.Enabled = true;

                DataRow[] rows = tb.Select("hjid='" + Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()) + "' ");
                //如果划价明细id=0 划价id=0 则是新处方
                if (rows.Length == 0)
                {
                    Dqcf.cfh = "0";
                    Dqcf.ysdm = Convert.ToInt32(Convertor.IsNull(txtys.Tag, "0"));
                    Dqcf.ksdm = Convert.ToInt32(Convertor.IsNull(txtks.Tag, "0"));
                    Dqcf.zyksid = 0;
                    Dqcf.xmly = 0;
                    Dqcf.tcid = 0;
                    Dqcf.zxksid = 0;
                    Dqcf.tjdxmdm = "";
                    Dqcf.js = 1;
                    //this.Text = Dqcf.zxksid.ToString();
                }
                else
                {
                    DataRow[] rowsx = tb.Select("hjid='" + Convertor.IsNull(tb.Rows[nrow]["hjid"], "0") + "' and  执行科室id<>'0' ");
                    Dqcf.cfh = Convert.ToString(rows[0]["HJID"]);
                    Dqcf.ysdm = Convert.ToInt32(rows[0]["医生id"]);
                    Dqcf.ksdm = Convert.ToInt32(rows[0]["科室id"]);
                    Dqcf.zyksid = Convert.ToInt32(rows[0]["住院科室id"]);
                    Dqcf.xmly = Convert.ToInt32(rows[0]["项目来源"]);


                    Dqcf.tcid = Convert.ToInt64(rows[0]["套餐id"]);
                    Dqcf.zxksid = Convert.ToInt32(Convertor.IsNull(rows[0]["执行科室id"], "0"));
                    if (rowsx.Length > 0)
                        Dqcf.zxksid = Convert.ToInt32(Convertor.IsNull(rowsx[0]["执行科室id"], "0"));
                    Dqcf.tjdxmdm = Convertor.IsNull(rows[0]["统计大项目"], "");
                    Dqcf.js = Convert.ToInt32(Convertor.IsNull(rows[0]["剂数"], "0"));

                    //this.Text = Dqcf.zxksid.ToString();
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //修改小计
        private void ModifCfje(DataTable tb, string hjid)
        {
            //修改小计
            decimal sumje = 0;
            if (hjid == "") hjid = Convertor.IsNull(hjid, Guid.Empty.ToString());
            DataRow[] rows = tb.Select("hjid='" + hjid + "' and 序号='小计' ");
            sumje = Convert.ToDecimal(Convertor.IsNull(tb.Compute("sum(金额)", "序号<>'小计'  and hjid='" + hjid + "' "), "0"));
            if (rows.Length == 1) rows[0]["金额"] = sumje.ToString("0.00");
            DataRow[] rows1 = tb.Select("hjid='" + hjid + "' and 序号<>'小计' ");

            int x = 0;
            for (int i = 0; i <= tb.Rows.Count - 1; i++)
            {
                if (hjid == Convertor.IsNull(tb.Rows[i]["hjid"], Guid.Empty.ToString()) && tb.Rows[i]["序号"].ToString() != "小计")
                {
                    x = x + 1;
                    tb.Rows[i]["序号"] = x.ToString();
                    tb.Rows[i]["排序序号"] = x.ToString();
                    if (tb.Rows[i]["hjmxid"].ToString() != "")
                        tb.Rows[i]["修改"] = true;
                }
            }
        }


        //网格行处理事件
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        //网格右键菜单的可见性
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                mnuDelrow.Enabled = true;
                mnuDelPresc.Enabled = true;
                mnuAddrow.Enabled = true;
                存为模板ToolStripMenuItem.Enabled = true;
                if (dataGridView1.CurrentCell == null)
                {
                    mnuDelrow.Enabled = false;
                    mnuDelPresc.Enabled = false;
                    mnuAddrow.Enabled = false;
                    //存为模板ToolStripMenuItem.Enabled = false;
                    return;
                }
                DataTable tb = (DataTable)dataGridView1.DataSource;
                int nrow = dataGridView1.CurrentCell.RowIndex;
                if (nrow > dataGridView1.Rows.Count) return;
                if (Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0")) == 0 && Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()).ToString() != Guid.Empty.ToString())
                    mnuDelrow.Enabled = false;
                if (Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0")) == 0 && Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()).ToString() == Guid.Empty.ToString())
                {
                    mnuDelrow.Enabled = false;
                    mnuDelPresc.Enabled = false;
                    mnuAddrow.Enabled = false;
                    //存为模板ToolStripMenuItem.Enabled = false;
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //当网格丢失焦点时
        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell = null;
        }

        #endregion

        #region 菜单事件
        private void mnuAddrow_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null) return;
            DataTable tb = (DataTable)dataGridView1.DataSource;
            int nrow = dataGridView1.CurrentCell.RowIndex;
            if (nrow > tb.Rows.Count) return;
            DataRow row = tb.NewRow();
            row["HJID"] = Dqcf.cfh;

            DataRow[] rows = tb.Select("hjid='" + Dqcf.cfh + "'");
            if (rows.Length > 0)
            {
                long byscf = Convert.ToInt64(Convertor.IsNull(rows[0]["byscf"], "0"));
                if (byscf == 1)
                {
                    MessageBox.Show("医生开具的处方您不能修改", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            tb.Rows.InsertAt(row, nrow);
            dataGridView1.DataSource = tb;
            dataGridView1.CurrentCell = dataGridView1.Rows[nrow].Cells["编码"];
            dataGridView1.Focus();
            IsRowAdd = true;
        }

        private void mnuDelrow_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null) return;
            BDelRow = true;
            DataTable tb = (DataTable)dataGridView1.DataSource;
            int nrow = dataGridView1.CurrentCell.RowIndex;
            if (nrow > tb.Rows.Count) return;
            string hjmxid = Convertor.IsNull(tb.Rows[nrow]["hjmxid"], Guid.Empty.ToString());
            Guid hjid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()));
            long tcid = Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["套餐ID"], "0"));
            long byscf = Convert.ToInt64(Convertor.IsNull(tb.Rows[nrow]["byscf"], "0"));
            if (byscf == 1)
            {
                MessageBox.Show("医生开具的处方您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string ssql = "";
            if (hjmxid == Guid.Empty.ToString())
            {
                try
                {
                    DataRow row = tb.Rows[nrow];
                    tb.Rows.Remove(row);
                    ModifCfje(tb, hjid.ToString());
                    ComputerJE(tb, hjid.ToString());//Modify By Tany 2009-01-05
                    BDelRow = false;
                    return;
                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            try
            {
                
                InstanceForm.BDatabase.BeginTransaction();
                string bbmc = Convertor.IsNull(tb.Rows[nrow]["规格"], "");
                if (Convertor.IsNull(tb.Rows[nrow]["项目来源"], "1") == "2" && cfg1095.Config.Trim() == "1")
                {
                    //同时作废医技申请表记录
                    //Guid hj_mxid=
                    if (hjmxid != Guid.Empty.ToString() && hjmxid != "99999999") //删除非套餐的医技申请项目
                    {
                        Guid _hjmxid = new Guid(hjmxid);
                        mzys_yjsq.DeleteDj(Guid.Empty, hjid, _hjmxid,"","", InstanceForm.BDatabase);
                    }
                    else //删除套餐医技申请项目
                    {
                        string xmid = Convertor.IsNull(tb.Rows[nrow]["项目ID"], "");
                        DataTable dt_order = mz_sf.GetFeeItemToOrder(xmid, tcid.ToString(), InstanceForm.BDatabase);
                        if (dt_order.Rows.Count > 0)
                            xmid = dt_order.Rows[0]["HOITEM_ID"].ToString();

                        bool Isdel_yj = false;
                        DataTable dtb = mzys_yjsq.GetYjsqInfo(hjid, xmid, Guid.Empty, bbmc, InstanceForm.BDatabase); //先获取所有医技申请项目 通过名称和标本名称精确到一条
                        if (dtb.Rows.Count > 0)
                        {
                            Guid yjid = new Guid(dtb.Rows[0]["YJSQID"].ToString());
                            mzys_yjsq.DeleteDj(yjid, hjid, Guid.Empty,"","", InstanceForm.BDatabase);
                            Isdel_yj = true;
                        }
                        if (!Isdel_yj)//还未删除则 直接通过划价id 删除
                            mzys_yjsq.DeleteDj(Guid.Empty, hjid, Guid.Empty,bbmc,"", InstanceForm.BDatabase);
                    }

                    //GetYjsqInfo
                }
                DataRow row = tb.Rows[nrow];
                tb.Rows.Remove(row);
                if (hjmxid.ToString() != "99999999")
                    ssql = "delete from mz_hjb_mx where hjmxid='" + hjmxid + "' and hjid in(select hjid from mz_hjb where hjid='" + hjid + "' and  bsfbz=0 and bfybz=0)";
                else
                {
                    if (cfg1095.Config.Trim() == "1")
                    {
                        if(!string.IsNullOrEmpty(bbmc))
                            ssql = @"delete from mz_hjb_mx where hjid='" + hjid + "' and GG='"+bbmc+"' and tcid=" + tcid + " and hjid in(select hjid from mz_hjb where hjid='" + hjid + "' and  bsfbz=0 and bfybz=0)";
                        else
                            ssql = "delete from mz_hjb_mx where hjid='" + hjid + "' and tcid=" + tcid + " and hjid in(select hjid from mz_hjb where hjid='" + hjid + "' and  bsfbz=0 and bfybz=0)"; 
                    }
                    else
                       ssql = "delete from mz_hjb_mx where hjid='" + hjid + "' and tcid=" + tcid + " and hjid in(select hjid from mz_hjb where hjid='" + hjid + "' and  bsfbz=0 and bfybz=0)";
                }
                int i = InstanceForm.BDatabase.DoCommand(ssql);

                if (i == 0) throw new Exception("该行可能已收费，没有删除成功，请刷新数据后重试");

                mz_hj.UpdateHjCfje(hjid, InstanceForm.BDatabase);

                ssql = "select * from mz_hjb_mx where hjid='" + hjid + "'";
                DataTable tab = InstanceForm.BDatabase.GetDataTable(ssql);
                if (tab.Rows.Count == 0)
                {
                    DataRow[] rows = tb.Select("hjid='" + hjid + "'");
                    for (int x = 0; x <= rows.Length - 1; x++)
                    {
                        tb.Rows.Remove(rows[x]);
                    }
                    ssql = "delete from mz_hjb where hjid='" + hjid + "' and bsfbz=0 and bfybz=0 ";
                    i = InstanceForm.BDatabase.DoCommand(ssql);
                }
                InstanceForm.BDatabase.CommitTransaction();

                ModifCfje(tb, hjid.ToString());
                ComputerJE(tb, hjid.ToString());//Modify By Tany 2009-01-05
                BDelRow = false;
            }
            catch (System.Exception err)
            {
                BDelRow = false;
                InstanceForm.BDatabase.RollbackTransaction();
                butref_Click(sender, e);
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void mnuDelPresc_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null) return;
            BDelRow = true;
            DataTable tb = (DataTable)dataGridView1.DataSource;
            int nrow = dataGridView1.CurrentCell.RowIndex;
            if (nrow > tb.Rows.Count) return;
            Guid hjid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()));

            string ssql = "";
            DataRow[] rows = tb.Select("hjid='" + hjid + "'");
            bool Isyj = false; //是否为非药品 如果是,则删除医技申请记录 add by zp 2013-10-16
            if (rows.Length > 0)
            {
                int byscf = Convert.ToInt16(Convertor.IsNull(rows[0]["byscf"], "0"));
                if (byscf == 1)
                {
                    MessageBox.Show("医生开具的处方您不能删除", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Convertor.IsNull(rows[0]["项目来源"], "1").Trim() == "2")
                    Isyj = true;
            }
        
            if (hjid == Guid.Empty)
            {
                for (int x = 0; x <= rows.Length - 1; x++)
                {
                    tb.Rows.Remove(rows[x]);
                }

                ModifCfje(tb, hjid.ToString());
                ComputerJE(tb, hjid.ToString());//Modify By Tany 2009-01-05
                BDelRow = false;
                return;
            }
        
            try
            {
                InstanceForm.BDatabase.BeginTransaction();
                ssql = "delete from mz_hjb_mx where hjid='" + hjid + "'";
                int i = InstanceForm.BDatabase.DoCommand(ssql);

                ssql = "delete from mz_hjb where hjid='" + hjid + "' and bsfbz=0 and bfybz=0 ";
                i = InstanceForm.BDatabase.DoCommand(ssql);
                if (i == 0) throw new Exception("当前处方可能已收费，没有删除成功，请刷新数据后重试");
                /*删除医技申请项目 Add by Zp 2013-10-16*/
                if (Isyj)
                    mzys_yjsq.DeleteDj(Guid.Empty,hjid, Guid.Empty,"", "", InstanceForm.BDatabase);
                InstanceForm.BDatabase.CommitTransaction();

                for (int x = 0; x <= rows.Length - 1; x++)
                {
                    tb.Rows.Remove(rows[x]);
                }

                ModifCfje(tb, hjid.ToString());
                ComputerJE(tb, hjid.ToString());//Modify By Tany 2009-01-05
                BDelRow = false;
            }
            catch (System.Exception err)
            {
                BDelRow = false;
                InstanceForm.BDatabase.RollbackTransaction();
                butref_Click(sender, e);
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void butsf_Click(object sender, EventArgs e)
        {
            SystemCfg cfg1046 = new SystemCfg(1046); //收费完成后打印方式 0-不打印任何凭据，1-打印发票，2-打印小票

            butsave_Click(sender, e);

            #region 变量付值

            SystemCfg sss;
            try
            {
                //门诊处方收费时是否立即发药
                sss = new SystemCfg(8013);
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string ssql = "";
            string Msg = "";
            DataTable tb = (DataTable)dataGridView1.DataSource;
            //查处是否有未保存的处方
            DataRow[] modifrow = tb.Select("修改=true");
            if (modifrow.Length>0)
                return;
            //分组处方
            string[] GroupbyField = { "HJID", "科室ID", "医生ID", "执行科室ID", "住院科室ID", "项目来源", "剂数", "划价日期", "hjy", "划价窗口" };
            string[] ComputeField = { "金额", "hjmxid" };
            string[] CField = { "sum", "count" };
            TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
            xcset.TsDataTable = tb;
            DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "选择=true");
            if (tbcf.Rows.Count == 0) { MessageBox.Show("没有要收费的处方"); return; }

            Guid _hjid = Guid.Empty;
            int _xmly = 0;
            //Add By Zj 2013-02-04  发生情况:当病人有多个处方的时候，前几个通过医保接口结算，后面几个自费结算，这样自费结算的处方退费时 就会有医保单据号，所以收费前需要先清空。如果不清空会造成退费时提示 医保单据号不为空，不能退费。
            lbljzh.Text = "";
            jsxx.JSDH = "";

            //划价窗口
            string sfck = "";

            //返回变量
            int err_code = -1;
            string err_text = "";
            //时间
            string sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();

            //要收费的处方字符串
            string shjid = "('";
            for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
                shjid += Convert.ToString(tbcf.Rows[i]["hjid"]) + "','";
            shjid = shjid.Substring(0, shjid.Length - 2);
            shjid += ")";

            //发票结果集
            DataSet dset = null;

            //收银金额
            decimal zje = 0;//总金额
            decimal yhje = 0;//优惠金额
            decimal srje = 0;//舍入金额
            decimal zfje = 0;//自付金额
            decimal ylkzf = 0;//银联
            decimal ybzhzf = 0;//医保帐户支付
            decimal ybjjzf = 0;//医保基金支付
            decimal ybbzzf = 0;//医保补助支付
            decimal ybzje = 0;//医保总金额
            decimal cwjz = 0;//财务记帐
            decimal qfgz = 0;//欠费挂帐
            decimal ssxj = 0;//实收现金
            decimal zpzf = 0;//支票支付
            decimal zlje = 0;//找零金额
            decimal xjzf = 0;//现金自付

            int fpzs = 0;//发票张数

            decimal ybkye = Convert.ToDecimal(Convertor.IsNull(lblybkye.Text, "0"));

            //卡属性
            int klx = Convert.ToInt32(Convertor.IsNull(lblklx.Tag, "0"));
            string kh = lblkh.Text.Trim();
            mz_card card = new mz_card(klx, InstanceForm.BDatabase);

            //读取病人卡余额
            ReadCard readcard = new ReadCard(klx, lblkh.Text.Trim(), InstanceForm.BDatabase);
            decimal ye = readcard.kye;
            if (readcard.sdbz == 1)
            {
                MessageBox.Show("病人卡已冻结，暂不能消费。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (readcard.sdbz == 2)
            {
                MessageBox.Show("病人卡已挂失，暂不能消费。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lblkh.Text.Trim() != "" && readcard.kdjid == Guid.Empty)
            {
                MessageBox.Show("没有找到卡信息，请确认卡是否输入正确。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            #endregion

            #region 处方审核
            try
            {
                //处方审核控制
                //医保病人处方需要审核
                SystemCfg syscfg1 = new SystemCfg(1042);
                if (syscfg1.Config == "1" && Convertor.IsNull(cmbyblx.SelectedValue, "0") != "0")
                {
                    DataRow[] rows = tb.Select(" 审核状态=0 or 审核状态=2");
                    if (rows.Length > 0)
                    {
                        MessageBox.Show("该病人有处方未通过审核,不能收费", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                //所有病人的药品处方需要审核
                SystemCfg syscfg2 = new SystemCfg(1043);
                if (syscfg2.Config == "1")
                {
                    DataRow[] rows = tb.Select(" (审核状态=0 or 审核状态=2) and 项目来源=1");
                    if (rows.Length > 0)
                    {
                        MessageBox.Show("该病人有药品处方未通过审核,不能收费", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region 验证是否更改处方
            for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
            {
                Guid yz_hjid = new Guid(Convertor.IsNull(tbcf.Rows[i]["hjid"], Guid.Empty.ToString()));
                decimal yz_cfje = Math.Round(Convert.ToDecimal(Convertor.IsNull(tbcf.Rows[i]["金额"], "0")), 2);
                ssql = "select * from mz_hjb where hjid='" + yz_hjid + "'";
                DataTable yz_tb = InstanceForm.BDatabase.GetDataTable(ssql);
                if (yz_tb.Rows.Count > 0)
                {
                    if (Convert.ToDecimal(yz_tb.Rows[0]["cfje"]) != yz_cfje)
                    {
                        MessageBox.Show("处方可能已更改,请重新刷新数据后重试！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (yz_tb.Rows[0]["bsfbz"].ToString() == "1")
                    {
                        MessageBox.Show("处方可能已收费,请重新刷新数据后重试！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("处方可能已删除,请刷新数据后重试！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // //add by zouchihua 2013-4-9 增加项目的判断 yzid
                ssql = "select * from mz_hjb_mx where hjid='" + yz_hjid + "' and  yzid='" + tbcf.Rows[i]["yzid"].ToString() + "'";
                DataTable hjmx = InstanceForm.BDatabase.GetDataTable(ssql);
                if (hjmx.Rows.Count <= 0)
                {
                    MessageBox.Show("处方可能已经修改,请刷新数据后重试！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            #endregion

            #region 医保预算

            int _yblx = Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0"));
            Yblx yblx = new Yblx(_yblx, InstanceForm.BDatabase);
            string HDGS_MZH = ""; //Add by zp 2013-12-30
            if (yblx.ybjklx > 0)
            {
                SystemCfg s = new SystemCfg(1031);
                if (s.Config == "1" && yblx.issf == true && lblbrxm_yb.Text.Trim() != txtxm.Text.Trim() && lblbrxm_yb.Text.Trim() != "")
                {
                    MessageBox.Show("不能收费！系统要求医保病人姓名与HIS系统中使用的姓名相同!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (yblx.issf == true)//收费是否使用医保
            {
                #region 医保预算
                try
                {
                    DataTable Tab_yb = tb.Clone();
                    DataRow[] rows_cf = tb.Select("选择=True");//rows_cf = 
                  
                    for (int i = 0; i <= rows_cf.Length - 1; i++)
                    {
                        
                        //如果是套餐则分解保存
                        int tcid = Convert.ToInt32(rows_cf[i]["套餐id"]); ;
                        if (tcid > 0)
                        {
                            int tccs = Convert.ToInt32(rows_cf[i]["数量"].ToString());
                            Guid tc_hjid = new Guid(rows_cf[i]["hjid"].ToString());
                            DataTable Tabtc = mz_sf.Select_Wsfcf(0, Guid.Empty, Dqcf.ghxxid, tcid, tccs, tc_hjid, InstanceForm.BDatabase);//Modify by zp 2014-01-10 以前传的hjid为空guid,因为需要传输划价明细id给创智 所以必须在数据库里将实际的划价id、划价明细id检索出来
                            for (int j = 0; j <= Tabtc.Rows.Count - 1; j++)
                            {
                                //MessageBox.Show(Tabtc.Rows.Count.ToString()+"  " + Tabtc.Rows[j]["hjid"].ToString());
                                Tab_yb.ImportRow(Tabtc.Rows[j]);
                            }
                        }
                        else
                            Tab_yb.ImportRow(rows_cf[i]);
                    }

                    cfmx = new ts_yb_mzgl.CFMX[Tab_yb.Rows.Count];
                    for (int i = 0; i <= cfmx.Length - 1; i++)
                    {
                        cfmx[i].HJID = Tab_yb.Rows[i]["hjid"].ToString();//Add By Tany 2010-08-06
                        cfmx[i].TJDXM = Tab_yb.Rows[i]["统计大项目"].ToString();
                        if (Tab_yb.Rows[i]["项目来源"].ToString() == "1")
                            cfmx[i].BM = Convertor.IsNull(yblx.ypbm, "") + Tab_yb.Rows[i]["HISCODE"].ToString();
                        else
                            cfmx[i].BM = Convertor.IsNull(yblx.xmbm, "") + Tab_yb.Rows[i]["HISCODE"].ToString();
                        if (Convertor.IsNull(Tab_yb.Rows[i]["项目来源"], "") == "1")
                            cfmx[i].MC = Convertor.IsNull(Tab_yb.Rows[i]["医嘱内容"], "").ToString().Trim();
                        else
                            cfmx[i].MC = Convertor.IsNull(Tab_yb.Rows[i]["项目名称"], "").ToString().Trim();
                         
                        cfmx[i].GG = Convertor.IsNull(Tab_yb.Rows[i]["规格"], "").ToString().Trim();
                        cfmx[i].JX = "";
                        cfmx[i].DJ = Tab_yb.Rows[i]["单价"].ToString();
                        decimal sl = Convert.ToDecimal(Tab_yb.Rows[i]["数量"]) * Convert.ToDecimal(Tab_yb.Rows[i]["剂数"]);
                        cfmx[i].SL = sl.ToString();
                        cfmx[i].JE = Tab_yb.Rows[i]["金额"].ToString();
                        cfmx[i].DW = Convertor.IsNull(Tab_yb.Rows[i]["单位"], "").ToString().Trim();
                        cfmx[i].SCCJ = Tab_yb.Rows[i]["厂家"].ToString();
                        cfmx[i].YSDM = Tab_yb.Rows[i]["医生id"].ToString();
                        cfmx[i].YSXM = Fun.SeekEmpName(Convert.ToInt32(Tab_yb.Rows[i]["医生id"]), InstanceForm.BDatabase);
                        cfmx[i].KSDM = Tab_yb.Rows[i]["科室id"].ToString();
                        cfmx[i].KSMC = Fun.SeekDeptName(Convert.ToInt32(Tab_yb.Rows[i]["科室id"]), InstanceForm.BDatabase);
                        cfmx[i].FSSJ = sDate;
                        cfmx[i].HJMXID = Tab_yb.Rows[i]["HJMXID"].ToString(); //Modify by zp 2013-12-13 创智医保需要hjmxid 以前该行注释了
                        //MessageBox.Show("" + cfmx[i].HJMXID + "");

                    }
                 
                    brxx.BRXXID = Dqcf.brxxid;
                    brxx.GHXXID = Dqcf.ghxxid;
                    brxx.BLH = lblmzh.Text;
                    brxx.ICD = Convertor.IsNull(cmbtb.SelectedValue, "");
                    brxx.ICDMC = Convertor.IsNull(cmbtb.Text, "");
                    //add by zouchihua 增加科室代码 和医生代码
                    brxx.KSDM = Dqcf.ksdm.ToString();
                    brxx.YSDM = Dqcf.ysdm.ToString();
                    //MessageBox.Show(Dqcf.ksdm.ToString());
                    ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                    //增加验证是否匹配项目
                    string pp="";
                    for (int i = 0; i < cfmx.Length; i++)
                    {
                        int _xmly1 = 0;
                        if (cfmx[i].BM.IndexOf(yblx.ypbm) >= 0)
                        {
                            _xmly1 = 1;
                        }
                        else
                            _xmly1 = 2;
                        string sql = "select a.*,c.* from jc_yb_bl a inner join jc_yblx b on a.yblx = b.id inner join jc_yb_match_record c on b.ybjklx = c.ybjklx and a.hsbm = c.yydm "
                                     + "  where a.xmid = '" + cfmx[i].BM.ToString().Replace(yblx.ypbm, "").Replace(yblx.xmbm, "") + "'  and a.xmly=" + _xmly1 + " and a.yblx =  " + yblx.yblx.ToString();
                        DataTable tbtemp=FrmMdiMain.Database.GetDataTable(sql);
                        if (tbtemp.Rows.Count == 0)
                            pp += "【 " + cfmx[i].MC + " " + " 】\r\n";
                        else
                        {
                            //add by zouchihua 2013-5-20
                            cfmx[i].YBBM = tbtemp.Rows[0]["YBBM"].ToString();
                            cfmx[i].YBMC = tbtemp.Rows[0]["YBMC"].ToString();
                        }
                        
                    }
                    //通过参数进行验证  因为有些医保的匹配信息HIS数据库没存储 Modify By zp 2013-10-23
                    if (pp != "" && cfg1097.Config.Trim().Length>0)
                    {
                        string[] par = cfg1097.Config.Split(',');
                        for (int y = 0; y < par.Length; y++)
                        {
                            if (par[y].Trim() == yblx.ybjklx.ToString())
                            {
                                MessageBox.Show("以下项目没有匹配:\r\n" + pp
                                           + "\r\n请刷新匹配关系或者重新匹配！");
                                return;
                            }
                        }
                    }
                    bool bok = ybjk.Compute(false, yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid, InstanceForm.BCurrentUser.EmployeeId.ToString(), cfmx, brxx, ref jsxx);
                    if (bok == false) throw new Exception("医保预算没有成功,操作中断");
                    ybzhzf = jsxx.ZHZF;
                    ybjjzf = jsxx.TCZF;
                    ybbzzf = jsxx.QTZF;
                    HDGS_MZH = jsxx.HDGS_MZH; //Add by zp 2013-12-30 华东工伤所需
                    zfje = jsxx.GRZF;
                    //hdgs_mzh=jsxx.
                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "医保预算错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                #endregion
            }
            #endregion

            sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();

            #region 显示收银对话框
            try
            {
                //返回发票相关信息
                dset = mz_sf.GetFpResult(shjid, _yblx, ybzhzf + ybjjzf + ybbzzf, 0, Guid.Empty, new Guid(Convertor.IsNull(cmbyhlx.SelectedValue, Guid.Empty.ToString())), TrasenFrame.Forms.FrmMdiMain.Jgbm, out err_code, out err_text, chkbfp.Checked == true ? 1 : 0, InstanceForm.BDatabase);
                 //填写流水号,一张发票对应一个流水号
                    for (int iFp = 0; iFp < dset.Tables[0].Rows.Count; iFp++)
                        dset.Tables[0].Rows[iFp]["dnlsh"] = Fun.GetNewDnlsh(InstanceForm.BDatabase);
                
                if (err_code != 0)
                {
                    MessageBox.Show("返回发票相关信息出现异常!原因:" +err_text, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
                }

                ybzje = ybzhzf + ybjjzf + ybbzzf;
                zje = Convert.ToDecimal(dset.Tables[0].Compute("sum(zje)", ""));
                yhje = Convert.ToDecimal(dset.Tables[0].Compute("sum(yhje)", ""));
                srje = Convert.ToDecimal(dset.Tables[0].Compute("sum(srje)", ""));
                zfje = Convert.ToDecimal(dset.Tables[0].Compute("sum(zfje)", ""));
                fpzs = dset.Tables[0].Rows.Count;

                if (fpzs > 1)
                {
                    if (yblx.issf == true) { MessageBox.Show("医保处方分票结算,每次只能收取一张发票,请重新选择处方", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    if (zje != ybzje && ybzje != 0) { MessageBox.Show("只有在[全医保支付]的情况下才允许多张发票一次性收费", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    if ((ybzhzf != 0 && ybzhzf != ybzje) || (ybjjzf != 0 && ybjjzf != ybzje) || (ybbzzf != 0 && ybbzzf != ybzje)) { MessageBox.Show("只有当[医保支付项]为单一支付的情况下,才允许多张发票一次性收费,如全部为帐户支付或全部为统筹支付", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                Frmsf f = new Frmsf(_menuTag, _chineseName, _mdiParent, InstanceForm.BDatabase);
               // f.lblybkye = lblybkye;//add by zouchihua 增加医保卡余额 2013-5-23
                f.lblks.Text = txtks.Text.Trim();
                f.lblxm.Text = txtxm.Text.Trim();
                f.lblfph.Text = txtfph.Text.Trim();
                f.lblklx.Text = kh == "" ? "" : lblklx.Text.Trim();
                f.lblkye.Text = ye.ToString();
                f.lblbrlx.Text = lblbrlx.Text.Trim();
                f.txtybzf.Enabled = (yblx.issf == true || (yblx.yblx == 0)) ? false : true;
                f.Bybsf = yblx.yblx > 0 ? true : false;
                if (yblx.issf == true)
                    f.lblbz.Text = "帐户支付:" + ybzhzf.ToString() + " 统筹支付:" + ybjjzf.ToString() + " 其他支付:" + ybbzzf.ToString();
                //add by zouchihua 2013-5-23 如果是为空说明没有获取医保卡余额，所以不做任何判断
                if (lblybkye.Text.Trim()!=""&&decimal.Parse(lblybkye.Text.Trim()) < ybzhzf)
                {
                   //如果医保卡余额小余账户支付
                    MessageBox.Show("医保卡余额小于账户支付，请重试！");
                    return;
                }

                f.lblzje.Text = zje.ToString();
                f.lblyhje.Text = yhje == 0 ? "" : yhje.ToString();
                f.lblsrje.Text = srje == 0 ? "" : srje.ToString();
                f.txtybzf.Text = ybzje == 0 ? "" : ybzje.ToString("0.00");

                f.lblhtdwlx.Text = lblhtdwlx.Text;
                f.lblhtdw.Text = lblhtdw.Text;

                f.klx = klx;
                f.kdjid = readcard.kdjid;

                //判断当前卡是否支持欠费挂帐 如果可以则欠费挂帐输入框可用
                string ssqfgz = new SystemCfg(1025).Config == "0" ? "true" : "false";
                if (ssqfgz == "true") f.txtqfgz.Enabled = true; else f.txtqfgz.Enabled = false;
                if (card.klx > 0 && card.bqfgz == true)
                    f.txtqfgz.Enabled = card.bqfgz;
                if (new SystemCfg(1037).Config == "1") f.txtzpzf.Enabled = false;
                if (new SystemCfg(1038).Config == "1" && lblhtdwlx.Text.Trim() == "") f.txtqfgz.Enabled = false;
                //卡中有余额
                if (ye > 0)
                {
                    if (ye >= zfje)
                    {
                        f.txtcwjz.Text = zfje.ToString();
                        f.lblysxj.Text = "0";
                    }
                    else
                    {
                        f.txtcwjz.Text = ye.ToString();
                        f.lblysxj.Text = Convert.ToDecimal(zfje - ye).ToString();
                    }
                }
                else
                {
                    f.txtcwjz.Enabled = false;
                    f.txtcwjz.Text = "";
                    f.lblysxj.Text = zfje.ToString();
                }

                //合同单位病人收银时,金额输入到挂帐一栏
                if (new SystemCfg(1036).Config == "1" && lblhtdwlx.Text.Trim() != "")
                {
                    f.txtqfgz.Text = zfje.ToString();
                    f.lblysxj.Text = "0";
                }

                f.lblfps.Text = dset.Tables[0].Rows.Count + " 张";
                f.fpzs = dset.Tables[0].Rows.Count;

                //求发票分类明细汇总
                string[] GroupbyField1 = { "code", "item_name" };
                string[] ComputeField1 = { "je" };
                string[] CField1 = { "sum" };
                TrasenFrame.Classes.TsSet xcset1 = new TrasenFrame.Classes.TsSet();
                xcset1.TsDataTable = dset.Tables[1];
                DataTable tbxm = xcset1.GroupTable(GroupbyField1, ComputeField1, CField1, "");

                decimal fpxmje = Convert.ToDecimal(Convertor.IsNull(tbxm.Compute("sum(je)", ""), "0"));
                DataRow row = tbxm.NewRow();
                row["je"] = fpxmje.ToString();
                row["item_name"] = "合计";
                tbxm.Rows.Add(row);

                f.dataGridView1.DataSource = tbxm;

                f.txtssxj.Focus();

                //报价器 应收
                //if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
                //{
                //    ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);
                //    decimal ysje = Convert.ToDecimal(Convertor.IsNull(f.lblysxj.Text, "0"));
                //    /*当卡余额大于应收时：按以下内容显示：第一行：姓名(科室)，
                //     * 第二行：卡余额，第三行：应收，第四行：实收，第五行：找零
                //     * */
                //    if (ye > ysje && bjqxh == "上海通导语音报价器型号Ⅳ")// 如果卡余额大于应收金额
                //    {
                //        decimal _cwjz = Convert.ToDecimal(Convertor.IsNull(f.txtcwjz.Text, "0"));
                //        string par = txtxm.Text.Trim() + ",(" + txtks.Text.Trim() + ")" + "," + (ye - _cwjz) + "元" + "," + ysje.ToString("0.00") + "元";
                //        call.Call(ts_call.DmType.应收, par);
                //    }
                //    else
                //        call.Call(ts_call.DmType.应收, ysje.ToString("0.00"));
                //}
                /*Modify by tck 2013-11 邵阳人医需求改动*/
                if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
                {
                    ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);
                    decimal ysje = Convert.ToDecimal(Convertor.IsNull(f.lblysxj.Text, "0"));
                    decimal _zje = Convert.ToDecimal(Convertor.IsNull(f.lblzje.Text, "0"));
                    ///*当卡余额大于应收时：按以下内容显示：第一行：姓名(科室)，
                    // * 第二行：卡余额，第三行：应收，第四行：实收，第五行：找零
                    // * */
                    if (ye > ysje && bjqxh == "上海通导语音报价器型号Ⅳ")// 如果卡余额大于应收金额
                    {
                        decimal _cwjz = Convert.ToDecimal(Convertor.IsNull(f.txtcwjz.Text, "0"));
                        string par = txtxm.Text.Trim() + ",(" + txtks.Text.Trim() + ")" + "," + (ye - _cwjz) + "元" + "," + ysje.ToString("0.00") + "元";
                        call.Call(ts_call.DmType.应收, par);
                    }
                    else if (ye > _zje && bjqxh == "上海通导语音报价器邵阳第一人民医院")
                    {
                        decimal _cwjz = Convert.ToDecimal(Convertor.IsNull(f.txtcwjz.Text, "0"));
                        //string par = txtxm.Text.Trim() + ",(" + txtks.Text.Trim() + ")" + "," + (ye - _cwjz) + "元" + "," + ysje.ToString("0.00") + "元";
                        //call.Call(ts_call.DmType.应收, par);
                        call.Call(ts_call.DmType.姓名, txtxm.Text.Trim() + "(" + txtks.Text.Trim() + ")");
                        call.Call(ts_call.DmType.总费用, _zje.ToString("0.00") + "元");//.总费用
                    }
                    else
                    {
                        call.Call(ts_call.DmType.姓名, txtxm.Text.Trim() + "(" + txtks.Text.Trim() + ")");
                        call.Call(ts_call.DmType.应收, ysje.ToString("0.00"));
                    }
                }

                f.ShowDialog();

                if (f.Bok == false)
                {
                    //Add By Tany 2010-0630
                    try
                    {
                        if (yblx.ybjklx > 0 && yblx.issf)
                        {
                            ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                            //bool bok = ybjk.DeleteYbInfo(yblx.insureCentral, yblx.hospid, brxx);
                            bool bok = ybjk.DeleteYbInfo(yblx.insureCentral, yblx.hospid, brxx, ref jsxx); //Modify By zp 2013-08-30

                            if (bok == false)
                            {
                                MessageBox.Show("取消医保登记没有成功，请重新读卡确认医保病人信息！");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    return;
                }

                //add by wangzhi 20101213 防止在收银期间处方被更改
                DataSet dset1 = mz_sf.GetFpResult(shjid, _yblx, ybzhzf + ybjjzf + ybbzzf, 0, Guid.Empty, new Guid(Convertor.IsNull(cmbyhlx.SelectedValue, Guid.Empty.ToString())), TrasenFrame.Forms.FrmMdiMain.Jgbm, out err_code, out err_text, chkbfp.Checked == true ? 1 : 0, InstanceForm.BDatabase);
                if (err_code != 0)
                {
                    MessageBox.Show("调用GetFpResult出错!" + err_text, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                decimal zje1 = Convert.ToDecimal(dset1.Tables[0].Compute("sum(zje)", ""));
                if (zje1 != zje)
                    throw new Exception("金额验证有误，可能处方已近被修改，请刷新后重试");


                if (Convert.ToDecimal(Convertor.IsNull(f.txtybzf.Text, "0")) != ybzje && ybzje > 0)
                    throw new Exception("医保预算金额和医保收银金额不一致," + f.txtybzf.Text.Trim() + "  " + ybzje.ToString() + "  " + "请和管理员联系");
                if ((ybzhzf + ybjjzf + ybbzzf) == 0 && Convert.ToDecimal(Convertor.IsNull(f.txtybzf.Text, "0")) > 0)
                {
                    ybjjzf = Convert.ToDecimal(Convertor.IsNull(f.txtybzf.Text, "0"));
                    ybzje = Convert.ToDecimal(Convertor.IsNull(f.txtybzf.Text, "0"));//医保支付 
                    //throw new Exception("医保分项不等总额,请重试");
                }


                yhje = Convert.ToDecimal(Convertor.IsNull(f.lblyhje.Text, "0"));//优惠金额
                srje = Convert.ToDecimal(Convertor.IsNull(f.lblsrje.Text, "0"));//舍入金额
                ylkzf = Convert.ToDecimal(Convertor.IsNull(f.txtpos.Text, "0"));//银联
                cwjz = Convert.ToDecimal(Convertor.IsNull(f.txtcwjz.Text, "0"));//财务记帐
                qfgz = Convert.ToDecimal(Convertor.IsNull(f.txtqfgz.Text, "0"));//欠费挂帐
                ssxj = Convert.ToDecimal(Convertor.IsNull(f.txtssxj.Text, "0"));//实收现金
                zpzf = Convert.ToDecimal(Convertor.IsNull(f.txtzpzf.Text, "0"));//支票支付
                zlje = Convert.ToDecimal(Convertor.IsNull(f.lblzl.Text, "0"));//找零金额
                xjzf = Convert.ToDecimal(Convertor.IsNull(f.lblysxj.Text, "0"));//现金自付

                plSyxx.上一病人 = txtxm.Text.Trim() + "(" + fpzs + "张)";
                plSyxx.总金额 = zje == 0 ? "" : zje.ToString();
                plSyxx.现金支付 = xjzf == 0 ? "" : xjzf.ToString("");
                plSyxx.实收现金 = ssxj == 0 ? "" : ssxj.ToString("");
                plSyxx.找零金额 = zlje == 0 ? "" : zlje.ToString("");
                plSyxx.银联支付 = ylkzf == 0 ? "" : ylkzf.ToString("");

                plSyxx.支票支付 = (zpzf) == 0 ? "" : zpzf.ToString("0.00"); ;
                plSyxx.医保支付 = (ybzje) == 0 ? "" : ybzje.ToString("0.00"); ;
                plSyxx.财务记账 = cwjz == 0 ? "" : cwjz.ToString("0.00");
                plSyxx.欠费挂账 = qfgz == 0 ? "" : qfgz.ToString("0.00");
                plSyxx.优惠金额 = yhje == 0 ? "" : yhje.ToString("0.00");
                plSyxx.舍入金额 = srje == 0 ? "" : srje.ToString("0.00");


                ////报价器 实收
                //if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
                //{
                //    ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);
                //    decimal ssje = ssxj + ylkzf + zpzf + cwjz;
                //    //call.Call(ssje.ToString("0.00"), zlje.ToString("0.00"));
                //    if (bjqxh.Trim() == "上海通导语音报价器型号Ⅳ" && ye > ssxj)
                //    {
                //        string par = ",,,," + ssxj.ToString("0.00") + "元";
                //        call.Call(ts_call.DmType.实收, par);//ssje.ToString("0.00")
                //    }
                //    else
                //        call.Call(ssje.ToString("0.00"), zlje.ToString("0.00")); 

                //}
            }
            catch (System.Exception err)
            {
                MessageBox.Show("收银对话框异常!"+err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region 财务记账刷卡验证
            if (cwjz > 0 && cfg1092.Config.Trim() == "1")
            {
                string strMsg = "读卡失败，医院诊疗卡支付需要进行读卡确认!请将卡放置读卡器上!\r\n可能配置不正确，可能读卡设备有问题...";
                string sbxh = ApiFunction.GetIniString("医院健康卡", "设备型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
                if (sbxh != "")
                {
                    //load 事件 中 将panel_yanzheng 的高度 设为0
                    ts_Read_hospitalCard.Icall ReadCard = ts_Read_hospitalCard.CardFactory.NewCall(sbxh);
                    if (ReadCard != null)
                    {
                        ts_Read_hospitalCard.CardFactory.ReadCard_for_yanzheng(ReadCard, _menuTag.Function_Name, cmbklx, txt_kh_yanzheng);
                        
                        if (string.IsNullOrEmpty(txt_kh_yanzheng.Text) || txt_kh_yanzheng.Text.Trim() != kh.Trim())
                        {
                            string strRetry = "医院诊疗卡支付需要进行读卡确认！\r\n按<重试>将再读一次卡，按<重试>前请放好卡\r\n否则退出，本次挂号无效！";
                            string strRetryMsg = "";
                            if (string.IsNullOrEmpty(txt_kh_yanzheng.Text)) strRetryMsg = "读取卡失败！" + strRetry;
                            else
                            {
                                // (txt_kh_yanzheng.Text.Trim() != kh.Trim())
                                strRetryMsg = "读卡器读取卡信息与挂号窗口填写的卡信息不符！挂号失败！\r\n" + strRetry;
                            }
                            DialogResult dlg = MessageBox.Show(strRetryMsg, "提示", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                            ts_Read_hospitalCard.CardFactory.ReadCard_for_yanzheng(ReadCard, _menuTag.Function_Name, cmbklx, txt_kh_yanzheng);
                            if (string.IsNullOrEmpty(txt_kh_yanzheng.Text))
                            {
                                if (dlg == DialogResult.Retry) MessageBox.Show(string.Format("按<重试>后仍然读卡失败，挂号无效！"), "提示");
                                return;
                            }
                            else if (txt_kh_yanzheng.Text.Trim() != kh.Trim())
                            {
                                string str = txt_kh_yanzheng.Text;
                                if (dlg == DialogResult.Retry) MessageBox.Show(string.Format("按<重试>后仍然读卡卡信息与挂号窗口填写的卡信息不符，挂号无效！", "提示"));
                                return;
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show(strMsg, "提示");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(strMsg, "提示");
                    return;
                }
            }
            #endregion

            #region 获得可用发票号集合
            //Modify By Zj 2013-01-10 动态获取发票类型 不再写死为1 收费发票 为了方便获取收据.ts_mz_class.mz_sf.GetFpLx(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase)
            DataTable tbfp = Fun.GetFph(InstanceForm.BCurrentUser.EmployeeId, dset.Tables[0].Rows.Count, ts_mz_class.mz_sf.GetFpLx(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase), out err_code, out err_text, InstanceForm.BDatabase);
            if (err_code != 0 || tbfp.Rows.Count == 0 || tbfp.Rows.Count != dset.Tables[0].Rows.Count)
            {
                if (cfg1046.Config == "1")//只有打发票时才判断 Modify by zouchihua 2013-4-23)
                {
                    MessageBox.Show(err_text, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            #endregion


            #region 医保正式结算
            try
            {
                if (yblx.issf == true)
                {
                    //医保结算前将处方收费状态置1,防止在医保结算期间医生修改处方
                    int u = InstanceForm.BDatabase.DoCommand("update mz_hjb set bsfbz = 1 where hjid in " + shjid + "  and bsfbz=0");
                    if (u == 0)
                    {
                        MessageBox.Show("该处方可能已收费,请您重新刷新", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    if (new SystemCfg(2).Config.Trim().Contains("萍乡")) //Modify by zp 2013-11-12
                    {
                        brxx.ICD = Convertor.IsNull(cmbtb.SelectedValue, "");//萍乡医院铁路医保需求 Add by 2013-09
                        brxx.KYE = (ybkye - ybzhzf).ToString(); //Modify By zp 2013-09 萍乡医院需求
                    }
                    //brxx.ICDMC = Convertor.IsNull(cmbtb.Text, "");
                    ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                    jsxx.HDGS_MZH = HDGS_MZH;//Add by zp 2013-12-30
                    bool bok = ybjk.Compute(true, yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid, InstanceForm.BCurrentUser.EmployeeId.ToString(), cfmx, brxx, ref jsxx);
                    if (bok == false) throw new Exception("医保正式结算没有成功,操作中断");
                    lbljzh.Text = jsxx.JSDH; //结算单号
                    
                    //不论医保结算是否成功，将收费标志置回
                    InstanceForm.BDatabase.DoCommand("update mz_hjb set bsfbz = 0 where hjid in " + shjid);
                }

            }
            catch (System.Exception err)
            {
                //不论医保结算是否成功，将收费标志置回
                InstanceForm.BDatabase.DoCommand("update mz_hjb set bsfbz = 0 where hjid in " + shjid);

                MessageBox.Show("医保正式结算异常!"+ err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #endregion

            try
            {
                sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();

                InstanceForm.BDatabase.BeginTransaction();
                butsf.Enabled = false;

                #region 保处到处方表
                //decimal cfje = 0;
                for (int i = 0; i <= tbcf.Rows.Count - 1; i++)
                {
                    //插入处方头
                    Guid _NewCfid = Guid.Empty;
                    string _mzh = lblmzh.Text.Trim();
                    _hjid = new Guid(Convertor.IsNull(tbcf.Rows[i]["hjid"], Guid.Empty.ToString()));
                    int _ksdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["科室id"], "0"));
                    int _ysdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["医生id"], "0"));
                    int _zxksdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["执行科室id"], "0"));
                    int _zyksdm = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["住院科室id"], "0"));
                    _xmly = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["项目来源"], "0"));
                    int _js = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["剂数"], "0"));
                    string _cfrq = tbcf.Rows[i]["划价日期"].ToString();
                    int _hjyid = Convert.ToInt32(Convertor.IsNull(tbcf.Rows[i]["hjy"], "0"));
                    string _hjyxm = Fun.SeekEmpName(_hjyid, InstanceForm.BDatabase);
                    string _hjck = tbcf.Rows[i]["划价窗口"].ToString();
                    decimal _cfje = Math.Round(Convert.ToDecimal(Convertor.IsNull(tbcf.Rows[i]["金额"], "0")), 2);


                    DataRow[] rows = tb.Select("HJID='" + _hjid + "' and 项目id>0");

                    long rowcount = Convert.ToInt32(tbcf.Rows[i]["HJMXID"]);
                    if (rowcount != rows.Length)
                        throw new Exception("分组处方时有" + rowcount + "行,插入处方时有" + rows.Length + "行.请检查处方状态或刷新处方再试");

                    if (rows.Length == 0) throw new Exception("没有找到处方明细,请和管理员联系");
                    mz_cf.SaveCf(Guid.Empty, Dqcf.brxxid, Dqcf.ghxxid, _mzh, _hjck, _cfje, _cfrq, _hjyid, _hjyxm, _hjck, _hjid, _ksdm, Fun.SeekDeptName(_ksdm, InstanceForm.BDatabase), _ysdm, Fun.SeekEmpName(_ysdm, InstanceForm.BDatabase), _zyksdm, _zxksdm, Fun.SeekDeptName(_zxksdm, InstanceForm.BDatabase), 0, 0, _xmly, 0, _js, Jgbm, out _NewCfid, out err_code, out err_text, InstanceForm.BDatabase);
                    if (_NewCfid == Guid.Empty || err_code != 0) throw new Exception(err_text);

                    //插处方明细表

                    for (int j = 0; j <= rows.Length - 1; j++)
                    {

                        int _tcid = Convert.ToInt32(Convertor.IsNull(rows[j]["套餐id"], "0"));
                        //如果是套餐则分解保存
                        if (_tcid > 0)
                        {
                            #region 如果是套餐则分解保存

                            DataRow[] tcrow = tb.Select("HJID='" + _hjid + "' and  套餐id=" + _tcid + " ");
                            if (tcrow.Length == 0) throw new Exception("查找套餐次数时出错，没有找到匹配的行");
                            _js = Convert.ToInt32(Convertor.IsNull(tcrow[0]["数量"], "0"));
                            DataTable Tabtc = mz_sf.Select_Wsfcf(0, Guid.Empty, Dqcf.ghxxid, _tcid, _js, _hjid, InstanceForm.BDatabase);
                            long _tcyzid = Convert.ToInt64(Convertor.IsNull(rows[j]["yzid"], "0"));//Add By Zj 2012-08-14 根据yzid判断套餐
                            DataRow[] rows_tc = Tabtc.Select(" yzid=" + _tcyzid + " ");//" "
                            if (rows_tc.Length == 0) throw new Exception("没有找到套餐的明细");
                            for (int xx = 0; xx <= rows_tc.Length - 1; xx++)
                            {
                                Guid _NewCfmxid = Guid.Empty;
                                Guid _hjmxid = new Guid(Convertor.IsNull(rows_tc[xx]["hjmxid"], Guid.Empty.ToString()));
                                string _pym = Convertor.IsNull(rows_tc[xx]["拼音码"], "");
                                string _bm = Convertor.IsNull(rows_tc[xx]["编码"], "");
                                string _pm = Convertor.IsNull(rows_tc[xx]["项目名称"], "");
                                string _spm = Convertor.IsNull(rows_tc[xx]["商品名"], "");
                                string _gg = Convertor.IsNull(rows_tc[xx]["规格"], "");
                                string _cj = Convertor.IsNull(rows_tc[xx]["厂家"], "");
                                decimal _dj = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["单价"], "0"));
                                decimal _sl = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["数量"], "0"));
                                string _dw = Convertor.IsNull(rows_tc[xx]["单位"], "");
                                int _ydwbl = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["ydwbl"], "0"));
                                decimal _je = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["金额"], "0"));
                                string _tjdxmdm = Convertor.IsNull(rows_tc[xx]["统计大项目"], "");
                                long _xmid = Convert.ToInt64(Convertor.IsNull(rows_tc[xx]["项目id"], "0"));
                                //int _bpsyybz = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["皮试用药"], "0"));
                                int _bpsbz = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["皮试标志"], "0"));
                                //int _bmsbz = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["免试标志"], "0"));
                                decimal _yl = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["剂量"], "0"));
                                string _yldw = Convertor.IsNull(rows_tc[xx]["剂量单位"], "");
                                int _yldwid = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["剂量单位id"], "0"));
                                int _dwlx = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["dwlx"], "0"));
                                int _yfid = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["用法id"], "0"));
                                string _yfmc = Convert.ToString(Convertor.IsNull(rows_tc[xx]["用法"], "0"));
                                int _pcid = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["频次id"], "0"));
                                string _pcmc = Convert.ToString(Convertor.IsNull(rows_tc[xx]["频次"], "0"));
                                decimal _ts = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["天数"], "0"));
                                string _zt = Convert.ToString(Convertor.IsNull(rows_tc[xx]["嘱托"], "0"));
                                int _fzxh = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["处方分组序号"], "0"));
                                int _pxxh = Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["排序序号"], "0"));
                                decimal _pfj = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["批发价"], "0"));
                                decimal _pfje = Convert.ToDecimal(Convertor.IsNull(rows_tc[xx]["批发金额"], "0"));
                                if (_js != Convert.ToInt32(Convertor.IsNull(rows_tc[xx]["剂数"], "0"))) throw new Exception("处方可能已修改,请重新刷新");
                                mz_cf.SaveCfmx(Guid.Empty, _NewCfid, _pym, _bm, _pm, _spm, _gg, _cj, _dj, _sl, _dw, _ydwbl, _js, _je, _tjdxmdm, _xmid, _hjmxid, _bm, 0, _bpsbz,
                                    Guid.Empty, _yl, _yldw, _yfmc, _pcid, _ts, _zt, _fzxh, _pxxh, Guid.Empty, _pfj, _pfje, _tcid, out _NewCfmxid, out err_code, out err_text, InstanceForm.BDatabase);
                                if (_NewCfmxid == Guid.Empty || err_code != 0) throw new Exception(err_text);

                                #region 套餐确费
                                if (cfg1063.Config == "1" && Convert.ToInt32(Convertor.IsNull(rows[j]["执行科室ID"], "0")) != 0)
                                {
                                    ParameterEx[] parameters = new ParameterEx[10];
                                    parameters[0].Text = "@CFID";
                                    parameters[0].Value = _NewCfid;
                                    parameters[1].Text = "@CFMXID";
                                    parameters[1].Value = _NewCfmxid;
                                    parameters[2].Text = "@TCID";
                                    parameters[2].Value = _tcid;


                                    parameters[3].Text = "@BQRBZ";
                                    parameters[3].Value = 1;
                                    parameters[4].Text = "@QRKS";
                                    parameters[4].Value = Convert.ToInt32(Convertor.IsNull(rows[j]["执行科室ID"], "0"));
                                    parameters[5].Text = "@QRRQ";
                                    parameters[5].Value = sDate;

                                    parameters[6].Text = "@QRDJY";
                                    parameters[6].Value = InstanceForm.BCurrentUser.EmployeeId;

                                    parameters[7].Text = "@err_code";
                                    parameters[7].ParaDirection = ParameterDirection.Output;
                                    parameters[7].DataType = System.Data.DbType.Int32;
                                    parameters[7].ParaSize = 100;

                                    parameters[8].Text = "@err_text";
                                    parameters[8].ParaDirection = ParameterDirection.Output;
                                    parameters[8].ParaSize = 100;

                                    parameters[9].Text = "@YQRKS";
                                    parameters[9].Value = 0;

                                    InstanceForm.BDatabase.GetDataTable("SP_YJ_SAVE_QRJL_MZ", parameters, 60);
                                    err_code = Convert.ToInt32(parameters[7].Value);
                                    err_text = Convert.ToString(parameters[8].Value);
                                    if (err_code != 0) throw new Exception(err_text);
                                }
                                #endregion
                            }

                            #endregion
                        }
                        else
                        {
                            #region 非套餐
                            Guid _NewCfmxid = Guid.Empty;
                            Guid _hjmxid = new Guid(Convertor.IsNull(rows[j]["hjmxid"], Guid.Empty.ToString()));
                            string _pym = Convertor.IsNull(rows[j]["拼音码"], "");
                            string _bm = Convertor.IsNull(rows[j]["编码"], "");
                            string _pm = Convertor.IsNull(rows[j]["项目名称"], "");
                            string _spm = Convertor.IsNull(rows[j]["商品名"], "");
                            string _gg = Convertor.IsNull(rows[j]["规格"], "");
                            string _cj = Convertor.IsNull(rows[j]["厂家"], "");
                            decimal _dj = Convert.ToDecimal(Convertor.IsNull(rows[j]["单价"], "0"));
                            decimal _sl = Convert.ToDecimal(Convertor.IsNull(rows[j]["数量"], "0"));
                            string _dw = Convertor.IsNull(rows[j]["单位"], "");
                            int _ydwbl = Convert.ToInt32(Convertor.IsNull(rows[j]["ydwbl"], "0"));
                            decimal _je = Convert.ToDecimal(Convertor.IsNull(rows[j]["金额"], "0"));
                            string _tjdxmdm = Convertor.IsNull(rows[j]["统计大项目"], "");
                            long _xmid = Convert.ToInt64(Convertor.IsNull(rows[j]["项目id"], "0"));
                            //int _bpsyybz = Convert.ToInt32(Convertor.IsNull(rows[j]["皮试用药"], "0"));
                            int _bpsbz = Convert.ToInt32(Convertor.IsNull(rows[j]["皮试标志"], "0"));
                            //int _bmsbz = Convert.ToInt32(Convertor.IsNull(rows[j]["免试标志"], "0"));
                            decimal _yl = Convert.ToDecimal(Convertor.IsNull(rows[j]["剂量"], "0"));
                            string _yldw = Convertor.IsNull(rows[j]["剂量单位"], "");
                            int _yldwid = Convert.ToInt32(Convertor.IsNull(rows[j]["剂量单位id"], "0"));
                            int _dwlx = Convert.ToInt32(Convertor.IsNull(rows[j]["dwlx"], "0"));
                            int _yfid = Convert.ToInt32(Convertor.IsNull(rows[j]["用法id"], "0"));
                            string _yfmc = Convert.ToString(Convertor.IsNull(rows[j]["用法"], "0"));
                            int _pcid = Convert.ToInt32(Convertor.IsNull(rows[j]["频次id"], "0"));
                            string _pcmc = Convert.ToString(Convertor.IsNull(rows[j]["频次"], "0"));
                            decimal _ts = Convert.ToDecimal(Convertor.IsNull(rows[j]["天数"], "0"));
                            string _zt = Convert.ToString(Convertor.IsNull(rows[j]["嘱托"], ""));
                            int _fzxh = Convert.ToInt32(Convertor.IsNull(rows[j]["处方分组序号"], "0"));
                            int _pxxh = Convert.ToInt32(Convertor.IsNull(rows[j]["排序序号"], "0"));
                            decimal _pfj = Convert.ToDecimal(Convertor.IsNull(rows[j]["批发价"], "0"));
                            decimal _pfje = Convert.ToDecimal(Convertor.IsNull(rows[j]["批发金额"], "0"));
                            Guid _pshjmxid = new Guid(Convertor.IsNull(rows[j]["pshjmxid"], Guid.Empty.ToString()));
                            mz_cf.SaveCfmx(Guid.Empty, _NewCfid, _pym, _bm, _pm, _spm, _gg, _cj, _dj, _sl, _dw, _ydwbl, _js, _je, _tjdxmdm, _xmid, _hjmxid, _bm, 0, _bpsbz,
                                _pshjmxid, _yl, _yldw, _yfmc, _pcid, _ts, _zt, _fzxh, _pxxh, Guid.Empty, _pfj, _pfje, 0, out _NewCfmxid, out err_code, out err_text, InstanceForm.BDatabase);
                            if (_NewCfmxid == Guid.Empty || err_code != 0) throw new Exception(err_text);
                            //Add By Zj 2012-07-10
                            string updatejsdsql = "update mz_cfb_mx set jsd='" + Convert.ToString(Convertor.IsNull(rows[j]["JSD"], "0")) + "' where cfmxid='" + _NewCfmxid.ToString() + "' ";
                            InstanceForm.BDatabase.DoCommand(updatejsdsql);
                            #region 非套餐确费
                            if (cfg1063.Config == "1" && Convert.ToInt32(Convertor.IsNull(rows[j]["执行科室ID"], "0")) != 0)
                            {
                                ParameterEx[] parameters = new ParameterEx[10];
                                parameters[0].Text = "@CFID";
                                parameters[0].Value = _NewCfid;
                                parameters[1].Text = "@CFMXID";
                                parameters[1].Value = _NewCfmxid;
                                parameters[2].Text = "@TCID";
                                parameters[2].Value = 0;


                                parameters[3].Text = "@BQRBZ";
                                parameters[3].Value = 1;
                                parameters[4].Text = "@QRKS";
                                parameters[4].Value = Convert.ToInt32(Convertor.IsNull(rows[j]["执行科室ID"], "0"));
                                parameters[5].Text = "@QRRQ";
                                parameters[5].Value = sDate;

                                parameters[6].Text = "@QRDJY";
                                parameters[6].Value = InstanceForm.BCurrentUser.EmployeeId;

                                parameters[7].Text = "@err_code";
                                parameters[7].ParaDirection = ParameterDirection.Output;
                                parameters[7].DataType = System.Data.DbType.Int32;
                                parameters[7].ParaSize = 100;

                                parameters[8].Text = "@err_text";
                                parameters[8].ParaDirection = ParameterDirection.Output;
                                parameters[8].ParaSize = 100;

                                parameters[9].Text = "@YQRKS";
                                parameters[9].Value = 0;

                                InstanceForm.BDatabase.GetDataTable("SP_YJ_SAVE_QRJL_MZ", parameters, 60);
                                err_code = Convert.ToInt32(parameters[7].Value);
                                err_text = Convert.ToString(parameters[8].Value);
                                if (err_code != 0) throw new Exception(err_text);
                            }
                            #endregion
                            #endregion 非套餐
                        }

                    }


                }
                #endregion

                #region  保存收银信息
                Guid NewJsid = Guid.Empty;
                mz_sf.SaveJs(Guid.Empty, Dqcf.brxxid, Dqcf.ghxxid, sDate, InstanceForm.BCurrentUser.EmployeeId, zje, ybzhzf, ybjjzf, ybbzzf, ylkzf, yhje, cwjz, qfgz, xjzf, zpzf, srje, ssxj, zlje, fpzs, 0, Jgbm, out NewJsid, out err_code, out err_text, InstanceForm.BDatabase);
                if (NewJsid == Guid.Empty || err_code != 0) throw new Exception(err_text);
                #endregion


                int UpdateCfs = 0;//更新处方头张数
                int UpdateHjs = 0;//更新划价头张数

                #region 保存发票信息 并更新处方状态
                for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)
                {
                    Guid NewFpid = Guid.Empty;
                    string fph = "";
                    if (cfg1046.Config == "1") //如果要打印发票
                        fph = Convertor.IsNull(tbfp.Rows[X]["QZ"], "") + tbfp.Rows[X]["fph"].ToString().Trim();

                    int ksdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ksdm"]);
                    int ysdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ysdm"]);
                    int zyksdm = Convert.ToInt32(dset.Tables[0].Rows[X]["zyksdm"]);
                    int zxks = Convert.ToInt32(dset.Tables[0].Rows[X]["zxks"]);
                    Guid yhlxid = new Guid(dset.Tables[0].Rows[X]["yhlxid"].ToString());
                    string yhlxmc = Fun.SeekYhlxMc(yhlxid, InstanceForm.BDatabase);
                    long dnlsh = Convert.ToInt64(dset.Tables[0].Rows[X]["dnlsh"]);

                    if (fpzs == 1)
                        mz_sf.SaveFp(Guid.Empty, Dqcf.brxxid, Dqcf.ghxxid, lblmzh.Text.Trim(), txtxm.Text.Trim(), sDate, InstanceForm.BCurrentUser.EmployeeId, sfck, dnlsh, fph, zje, ybzhzf, ybjjzf, ybbzzf, ylkzf, yhje, cwjz, qfgz, xjzf, zpzf, srje, Guid.Empty, "", NewJsid, 0, ksdm, ysdm, zyksdm, zxks, yblx.yblx, Convertor.IsNull(jsxx.JSDH, ""), 0, readcard.kdjid, Jgbm, yhlxid, yhlxmc, out NewFpid, out err_code, out err_text, InstanceForm.BDatabase);
                    else
                    {
                        //decimal fp_zfje = Convert.ToDecimal(dset.Tables[0].Rows[X]["zfje"]);
                        //decimal fp_zje = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]);
                        //decimal fp_yhje = Convert.ToDecimal(dset.Tables[0].Rows[X]["yhje"]);
                        //decimal fp_srje = Convert.ToDecimal(dset.Tables[0].Rows[X]["srje"]);
                        //decimal fp_ylkzf = ylkzf > 0 ? fp_zfje : 0;
                        //decimal fp_cwjz = cwjz > 0 ? fp_zfje : 0;
                        //decimal fp_qfgz = qfgz > 0 ? fp_zfje : 0;
                        //decimal fp_xjzf = xjzf > 0 ? fp_zfje : 0;
                        //decimal fp_zpzf = zpzf > 0 ? fp_zfje : 0;
                        //mz_sf.SaveFp(Guid.Empty, Dqcf.brxxid, Dqcf.ghxxid, lblmzh.Text.Trim(), txtxm.Text.Trim(), sDate, InstanceForm.BCurrentUser.EmployeeId, sfck, dnlsh, fph, fp_zje, ybzhzf, ybjjzf, ybbzzf, fp_ylkzf, fp_yhje, fp_cwjz, fp_qfgz, fp_xjzf, fp_zpzf, fp_srje, Guid.Empty, "", NewJsid, 0, ksdm, ysdm, zyksdm, zxks, yblx.yblx, jsxx.JSDH, 0, readcard.kdjid, Jgbm, yhlxid, yhlxmc, out NewFpid, out err_code, out err_text, InstanceForm.BDatabase);

                        decimal fp_zfje = Convert.ToDecimal(dset.Tables[0].Rows[X]["zfjeex"]);
                        decimal fp_zje = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]);
                        decimal fp_yhje = Convert.ToDecimal(dset.Tables[0].Rows[X]["yhje"]);
                        decimal fp_srje = Convert.ToDecimal(dset.Tables[0].Rows[X]["srje"]);
                        decimal fp_ybzhzf = ybzhzf > 0 && ybjjzf == 0 && ybbzzf == 0 ? fp_zfje : 0;
                        decimal fp_ybjjzf = ybjjzf > 0 && ybzhzf == 0 && ybbzzf == 0 ? fp_zfje : 0;
                        decimal fp_ybbzzf = ybbzzf > 0 && ybzhzf == 0 && ybjjzf == 0 ? fp_zfje : 0;
                        decimal fp_ylkzf = ylkzf > 0 ? fp_zfje : 0;
                        decimal fp_cwjz = cwjz > 0 ? fp_zfje : 0;
                        decimal fp_qfgz = qfgz > 0 ? fp_zfje : 0;
                        decimal fp_xjzf = xjzf > 0 ? fp_zfje : 0;
                        decimal fp_zpzf = zpzf > 0 ? fp_zfje : 0;
                        mz_sf.SaveFp(Guid.Empty, Dqcf.brxxid, Dqcf.ghxxid, lblmzh.Text.Trim(), txtxm.Text.Trim(), sDate, InstanceForm.BCurrentUser.EmployeeId, sfck, dnlsh, fph, fp_zje, fp_ybzhzf, fp_ybjjzf, fp_ybbzzf, fp_ylkzf, fp_yhje, fp_cwjz, fp_qfgz, fp_xjzf, fp_zpzf, fp_srje, Guid.Empty, "", NewJsid, 0, ksdm, ysdm, zyksdm, zxks, yblx.yblx, Convertor.IsNull(jsxx.JSDH, ""), 0, readcard.kdjid, Jgbm, yhlxid, yhlxmc, out NewFpid, out err_code, out err_text, InstanceForm.BDatabase);
                    }

                    //更新本地医保结算表的收费信息
                    if (yblx.issf == true)
                    {
                        ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                        bool bok = ybjk.UpdateJsmx(Dqcf.brxxid, Dqcf.ghxxid, 0, jsxx.HisJsdid, NewFpid, fph, sDate, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase);
                        if (bok == false) throw new Exception("更新本地医保结算表的收费信息,操作中断");
                    }

                    dset.Tables[0].Rows[X]["fph"] = fph.ToString();
                    dset.Tables[0].Rows[X]["fpid"] = NewFpid;

                    if (err_code != 0 || NewFpid == Guid.Empty) throw new Exception(err_text);

                    string _sHjid = dset.Tables[0].Rows[X]["hjid"].ToString().Trim();
                    _sHjid = _sHjid.Replace("'", "''");

                    //发票明细
                    decimal fpje = 0;
                    DataRow[] rows = dset.Tables[1].Select(@"hjid = '" + _sHjid + "'");
                    for (int i = 0; i <= rows.Length - 1; i++)
                    {
                        mz_sf.SaveFpmx(NewFpid, Convertor.IsNull(rows[i]["code"], "0"), Convertor.IsNull(rows[i]["item_name"], "0"), Convert.ToDecimal(rows[i]["je"]), 0, out err_code, out err_text, InstanceForm.BDatabase);
                        if (err_code != 0) throw new Exception(err_text);
                        fpje = fpje + Convert.ToDecimal(rows[i]["je"]);
                    }
                    if (fpje != Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]) - (Convert.ToDecimal(dset.Tables[0].Rows[X]["srje"]))) throw new Exception("插入发票明细时出错,金额不等于发票总额");

                    //发票统计大项目明细
                    decimal tjxmje = 0;
                    DataRow[] rows1 = dset.Tables[3].Select("hjid = '" + _sHjid + "'");
                    for (int i = 0; i <= rows1.Length - 1; i++)
                    {
                        mz_sf.SaveFpdxmmx(NewFpid, Convertor.IsNull(rows1[i]["code"], "0"), Convertor.IsNull(rows1[i]["item_name"], "0"), Convert.ToDecimal(rows1[i]["je"]), 0, out err_code, out err_text, InstanceForm.BDatabase);
                        if (err_code != 0) throw new Exception(err_text);
                        tjxmje = tjxmje + Convert.ToDecimal(rows1[i]["je"]);
                    }
                    if (tjxmje != Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]) - (Convert.ToDecimal(dset.Tables[0].Rows[X]["srje"]))) throw new Exception("插入发票明细时出错,金额不等于发票总额");

                    //更新划方表状态  条件 hjid 和 收费标志
                    int Nrows = 0;
                    mz_cf.UpdateCfsfzt_E(dset.Tables[0].Rows[X]["hjid"].ToString().Trim(), InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BCurrentUser.Name, sDate, sfck, dnlsh, fph, NewFpid, out Nrows, out err_code, out err_text, InstanceForm.BDatabase);
                    UpdateCfs = UpdateCfs + Nrows;
                    //更新价划处方表
                    mz_hj.UpdateCfsfzt(dset.Tables[0].Rows[X]["hjid"].ToString().Trim(), 1, 0, out Nrows, out err_code, out err_text, InstanceForm.BDatabase);
                    UpdateHjs = UpdateHjs + Nrows;
                    //更新医技申请的收费状态
                    int iiii = mzys_yjsq.UpdateSfbz(dset.Tables[0].Rows[X]["hjid"].ToString().Trim(), sDate, InstanceForm.BDatabase);

                    #region 门诊收费是否发药
                    if (sss.Config == "1")
                    {
                        Guid _Fyid = Guid.Empty;
                        Guid _Fymxid = Guid.Empty;
                        ssql = @"select *,(select top 1 TJDXMDM from MZ_CFB_MX where CFID=a.CFID) cflx from mz_cfb a 
                        where fpid= '" + NewFpid + "' and bscbz=0 and xmly=1";
                        DataTable tbfy = InstanceForm.BDatabase.GetDataTable(ssql);
                        for (int i = 0; i <= tbfy.Rows.Count - 1; i++)
                        {
                            //Modify by zouchihua 2013-5-27 增加处方类型
                            //Modify By zp 2013-09-01 参数jssjh从0改为传输门诊处方表的电脑流水号
                            YpClass.MZYF.SaveFy(Convert.ToString(tbfy.Rows[i]["cflx"]),Convert.ToDecimal(Convertor.IsNull( tbfy.Rows[i]["DNLSH"],"0")), Convert.ToInt64(fph), Convert.ToDecimal(tbfy.Rows[i]["zje"]), 0, 0, 0, Convert.ToInt32(tbfy.Rows[i]["cfjs"]), new Guid(tbfy.Rows[i]["cfid"].ToString()), new Guid(tbfy.Rows[i]["brxxid"].ToString()),
                                Convert.ToString(tbfy.Rows[i]["blh"]), txtxm.Text.Trim(), ysdm, ksdm, sDate, InstanceForm.BCurrentUser.EmployeeId, sDate, InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BCurrentUser.EmployeeId, "", sfck, zxks, 0, "017", 0, "sp_yf_fy",
                                out _Fyid, out err_code, out err_text, TrasenFrame.Forms.FrmMdiMain.Jgbm, InstanceForm.BDatabase);
                            if (_Fyid == Guid.Empty || err_code != 0) throw new Exception(err_text);

                            ssql = "select *,coalesce(dbo.Fun_getFreqName(cast(coalesce(PCID,0) as smallint)),'''') pc from mz_cfb_mx a,yp_ypcjd b where a.xmid=b.cjid and cfid='" + new Guid(tbfy.Rows[i]["cfid"].ToString()) + "' and  bscbz=0";
                            DataTable tbfymx = InstanceForm.BDatabase.GetDataTable(ssql);
                            for (int j = 0; j <= tbfymx.Rows.Count - 1; j++)
                            {
                                string bpsbz = "";
                                switch (Convert.ToInt32(tbfymx.Rows[j]["bpsbz"]))
                                {
                                    case 0:
                                        bpsbz = "【皮试】";
                                        break;
                                    case 1:
                                        bpsbz = "【-】";
                                        break;
                                    case 2:
                                        bpsbz = "【+】";
                                        break;
                                    case 3:
                                        bpsbz = "【免试】";
                                        break;
                                    default:
                                        break;
                                }
                                MZYF.SaveFymx(_Fyid, Convert.ToInt64(fph), new Guid(tbfymx.Rows[j]["cfid"].ToString()), Convert.ToInt32(tbfymx.Rows[j]["xmid"]), Convertor.IsNull(tbfymx.Rows[j]["shh"], ""), Convertor.IsNull(tbfymx.Rows[j]["pm"], ""),
                                    Convertor.IsNull(tbfymx.Rows[j]["spm"], ""), Convertor.IsNull(tbfymx.Rows[j]["gg"], ""), Convertor.IsNull(tbfymx.Rows[j]["cj"], ""), Convertor.IsNull(tbfymx.Rows[j]["dw"], ""),
                                    Convert.ToUInt32(tbfymx.Rows[j]["ydwbl"]), Convert.ToDecimal(tbfymx.Rows[j]["sl"]), Convert.ToInt32(tbfymx.Rows[j]["js"]), Convert.ToDecimal(tbfymx.Rows[j]["pfj"]),
                                    Convert.ToDecimal(tbfymx.Rows[j]["pfje"]), Convert.ToDecimal(tbfymx.Rows[j]["dj"]), Convert.ToDecimal(tbfymx.Rows[j]["je"]), 0, 0, zxks, Guid.Empty, "", Guid.Empty, new Guid(tbfymx.Rows[j]["cfmxid"].ToString()), bpsbz, //Modify  By Zj 2012-09-22 将bpsbz的传值为空 改为获取处方明细表的皮试状态
                                    "", "", Convertor.IsNull(tbfymx.Rows[j]["yfmc"], ""), Convertor.IsNull(tbfymx.Rows[j]["pc"], ""), Convertor.IsNull(tbfymx.Rows[j]["yl"], ""),
                                    Convertor.IsNull(tbfymx.Rows[j]["yldw"], ""), Convert.ToDecimal(tbfymx.Rows[j]["ts"]), Convert.ToInt32(tbfymx.Rows[j]["fzxh"]),
                                    Convert.ToInt32(tbfymx.Rows[j]["pxxh"]), "sp_YF_FYMX", out _Fymxid, out err_code, out err_text, InstanceForm.BDatabase);
                                if (_Fymxid == Guid.Empty || err_code != 0) throw new Exception(err_text);
                            }
                        }
                    }
                    #endregion

                }

                //更新卡余额和累计消息金额
                if (cwjz > 0)
                    readcard.UpdateKye(cwjz, InstanceForm.BDatabase);
                #endregion

                #region  更新发票领用表的当前发票号码
                if (cfg1046.Config == "1") //如果不打印发票，则不更新发票领用表
                    mz_sf.UpdateDqfph(new Guid(tbfp.Rows[0]["fpid"].ToString()), tbfp.Rows[0]["fph"].ToString().Trim(), tbfp.Rows[tbfp.Rows.Count - 1]["fph"].ToString().Trim(), out Msg, InstanceForm.BDatabase);
                #endregion
                #region 判断处方更新张数和实际分组张数是否一样
                if (UpdateCfs != tbcf.Rows.Count)
                    throw new Exception("更新处方表张数" + UpdateCfs + "张,分组处方张数" + tbcf.Rows.Count + "张.请检查处方状态或刷新处方再试");
                if (UpdateHjs != tbcf.Rows.Count)
                    throw new Exception("更新处方表张数" + UpdateHjs + "张,分组处方张数" + tbcf.Rows.Count + "张.请检查处方状态或刷新处方再试");
                #endregion

                InstanceForm.BDatabase.CommitTransaction();
                #region//报价器 实收 Modify By tck 2013-11-21
                if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
                {
                    ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);
                    
                    decimal ssje = ssxj + ylkzf + zpzf + cwjz;
                    //call.Call(ssje.ToString("0.00"), zlje.ToString("0.00"));
                    if (bjqxh.Trim() == "上海通导语音报价器型号Ⅳ" && ye > ssxj)
                    {
                        string par = ",,,," + ssxj.ToString("0.00") + "元";
                        call.Call(ts_call.DmType.实收, par);//ssje.ToString("0.00")
                    }
                    //ADD BY TCK 2013-11-21
                    else if (bjqxh.Trim() == "上海通导语音报价器邵阳第一人民医院" && cwjz > 0 && ssxj == 0)
                    {
                        ////卡支付金额+卡支付后余额 带“元”
                        string par = cwjz.ToString("0.00") + "元" + "," + (ye - cwjz) + "元";
                        call.Call(ts_call.DmType.实收, par);//ssje.ToString("0.00")
                    }
                    else
                        call.Call(ssxj.ToString("0.00"), zlje.ToString("0.00"));
                }
                //if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
                //{
                //    ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);
                //    decimal ssje = ssxj + ylkzf + zpzf + cwjz;
                //    //call.Call(ssje.ToString("0.00"), zlje.ToString("0.00"));
                //    if (bjqxh.Trim() == "上海通导语音报价器型号Ⅳ" && ye > ssxj)
                //    {
                //        string par = ",,,," + ssxj.ToString("0.00") + "元";
                //        call.Call(ts_call.DmType.实收, par);//ssje.ToString("0.00")
                //    }
                //    else
                //        call.Call(ssje.ToString("0.00"), zlje.ToString("0.00"));
                //}
                Tab = null;
                #endregion
            }
            catch (System.Exception err)
            {
                butsf.Enabled = true;
                InstanceForm.BDatabase.RollbackTransaction();
                MessageBox.Show("保存数据出错!"+err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cfg1046.Config == "1")
            {
                //MessageBox.Show(dset.Tables.Count.ToString());
                PrintBill(dset, klx, card, sDate, ybkye, ye, ybbzzf, ybzje, ybzhzf, yblx, ybjjzf, xjzf, sfck);
                #region 打印发票
                //try
                //{
                //    string szxks = "";//Add By Zj 2013-01-16
                //    for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)
                //    {
                //        //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                //        string fyck = "";
                //        mz_sf.UpdateFyck(Dqcf.brxxid, new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()), out fyck, InstanceForm.BDatabase);

                //        int ksdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ksdm"]);
                //        int ysdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ysdm"]);
                //        int zxks = Convert.ToInt32(dset.Tables[0].Rows[X]["zxks"]);
                //        if (zxks != 0 && cfg1068.Config == "1")//Add By Zj 2013-01-16
                //        {
                //            ssql = "select NAME,DEPTADDR from  JC_DEPT_PROPERTY where dept_id=" + zxks.ToString();
                //            DataRow deptdr = InstanceForm.BDatabase.GetDataRow(ssql);
                //            szxks += deptdr["NAME"] + " 位置:" + Convertor.IsNull(deptdr["DEPTADDR"], "") + "\r\n";
                //        }

                //        ssql = "select * from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                //        DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                //        PrintClass.OPDInvoice invoice = new PrintClass.OPDInvoice();
                //        invoice.OtherInfo = "";
                //        invoice.HisName = Constant.HospitalName;  //医院名称
                //        invoice.PatientName = txtxm.Text.Trim();  //病人姓名
                //        invoice.OutPatientNo = lblmzh.Text.Trim(); //门诊号
                //        invoice.DepartmentName = Fun.SeekDeptName(ksdm, InstanceForm.BDatabase);//科室名称
                //        invoice.DoctorName = Fun.SeekEmpName(ysdm, InstanceForm.BDatabase); //医生名称
                //        invoice.InvoiceNo = "电脑票号：" + Convert.ToString(dset.Tables[0].Rows[X]["fph"]);//电脑发票号

                //        invoice.TotalMoneyCN = Money.NumToChn(dset.Tables[0].Rows[X]["zje"].ToString());//总金额（大写）
                //        invoice.TotalMoneyNum = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]);//总金额（小写）
                //        if (cfgsfy.Config == "1") //显示收款人姓名还是代码
                //            invoice.Payee = InstanceForm.BCurrentUser.Name;  //收款人
                //        else
                //            invoice.Payee = InstanceForm.BCurrentUser.LoginCode;

                //        DateTime time = Convert.ToDateTime(sDate);
                //        invoice.Year = time.Year; 
                //        invoice.Month = time.Month;
                //        invoice.Day = time.Day;

                //        bool bqedy = mz_sf.Bqedy(new Guid(Convertor.IsNull(tbFp.Rows[0]["yhlxid"], Guid.Empty.ToString())), InstanceForm.BDatabase);

                //        if (bqedy == true && Convert.ToDecimal(tbFp.Rows[0]["yhje"]) != 0)
                //        {
                //            invoice.Yhje = 0;
                //            invoice.Qfgz = 0;
                //            invoice.Ybzhzf = 0;
                //            invoice.Ybjjzf = 0;
                //            invoice.Ybbzzf = 0;
                //            invoice.Cwjz = 0;
                //            invoice.Ylkje = 0;
                //            invoice.Srje = 0;
                //            invoice.Xjzf = 0;
                //            invoice.Zpzf = 0;
                //        }
                //        else
                //        {
                //            invoice.Yhje = Convert.ToDecimal(tbFp.Rows[0]["yhje"]);
                //            invoice.Qfgz = Convert.ToDecimal(tbFp.Rows[0]["qfgz"]);
                //            invoice.Ybzhzf = ybzhzf;
                //            invoice.Ybjjzf = ybjjzf;
                //            invoice.Ybbzzf = ybbzzf;
                //            invoice.Cwjz = Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);
                //            invoice.Ylkje = Convert.ToDecimal(tbFp.Rows[0]["ylkzf"]);
                //            invoice.Srje = Convert.ToDecimal(tbFp.Rows[0]["srje"]);
                //            invoice.Xjzf = Convert.ToDecimal(tbFp.Rows[0]["xjzf"]);
                //            invoice.Zpzf = Convert.ToDecimal(tbFp.Rows[0]["Zpzf"]);
                //        }

                //        invoice.Zxks = Fun.SeekDeptName(zxks, InstanceForm.BDatabase);

                //        ye = ye - invoice.Cwjz;
                //        invoice.Kye = ye;

                //        invoice.Ybkye = ybkye - ybzhzf;
                //        if (invoice.Ybkye < 0)
                //            invoice.Ybkye = 0;

                //        invoice.Ybkh = lblybkh.Text.Trim();

                //        invoice.Yblx = cmbyblx.Text.Trim();
                //        invoice.Ybjydjh = jsxx.JSDH;
                //        invoice.Klx = lblkh.Text.Trim() == "" ? "" : cmbklx.Text.Trim();
                //        invoice.Klx_Bje = card.bjebz;

                //        invoice.sfck = sfck;
                //        invoice.fyck = fyck;
                //        invoice.htdwlx = lblhtdwlx.Text.Trim();
                //        invoice.htdwmc = lblhtdw.Text.Trim();
                //        invoice.kswz = "";

                //        invoice.Klx = cmbklx.Text.Trim();
                //        invoice.kh = txtkh.Text.Trim();
                //        invoice.sfsj = Convert.ToDateTime(sDate).ToLongTimeString();
                //        invoice.ghjb = Convertor.IsNull(label5.Tag.ToString(), "");//Add By Zj 2012-03-06 
                //        invoice.Mzhtm = lblmzh.Text.Trim();//Add By Zj 2012-12-11 门诊号条码 方便LIS使用
                //        PrintClass.InvoiceItem[] item = null;
                //        PrintClass.InvoiceItemDetail[] itemdetail = null; //Modify By Tany 2008-12-20 增加发票明细项目

                //        string _sHjid = dset.Tables[0].Rows[X]["hjid"].ToString().Trim();
                //        _sHjid = _sHjid.Replace("'", "''");



                //        DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                //        item = new PrintClass.InvoiceItem[rows.Length];
                //        for (int m = 0; m <= rows.Length - 1; m++)
                //        {
                //            item[m].ItemName = rows[m]["ITEM_NAME"].ToString().Trim();
                //            item[m].ItemMoney = Convert.ToDecimal(rows[m]["je"]);//发票项目金额
                //        }
                //        invoice.Items = item;

                //        string _fpyhmc1 = "";
                //        string _fpyhmc2 = "";
                //        _fpyhmc1 = ApiFunction.GetIniString("划价收费", "发票优惠项目名称1", Constant.ApplicationDirectory + "//ClientWindow.ini");
                //        _fpyhmc2 = ApiFunction.GetIniString("划价收费", "发票优惠项目名称2", Constant.ApplicationDirectory + "//ClientWindow.ini");
                //        if (_fpyhmc1 == "")
                //            _fpyhmc1 = "慈善支付";
                //        if (_fpyhmc2 == "")
                //            _fpyhmc2 = "个人支付";
                //        //增加发票明细项目
                //        DataRow[] rowsdetail = dset.Tables[4].Select("hjid='" + _sHjid + "' and sl<>0");//Modify Bj Zj 2012-09-11
                //        itemdetail = new PrintClass.InvoiceItemDetail[rowsdetail.Length];
                //        for (int m = 0; m <= rowsdetail.Length - 1; m++)
                //        {
                //            itemdetail[m].ItemDetailName = rowsdetail[m]["PM"].ToString().Trim();
                //            itemdetail[m].ItemDW = rowsdetail[m]["DW"].ToString().Trim();
                //            itemdetail[m].ItemGG = rowsdetail[m]["GG"].ToString().Trim();
                //            itemdetail[m].ItemJS = Convert.ToDecimal(rowsdetail[m]["JS"]);
                //            itemdetail[m].ItemNum = Convert.ToDecimal(rowsdetail[m]["SL"]);
                //            itemdetail[m].ItemPrice = Convert.ToDecimal(rowsdetail[m]["DJ"]);
                //            itemdetail[m].ItemJE = Convert.ToDecimal(rowsdetail[m]["JE"]);
                //            if (Convert.ToDecimal(rowsdetail[m]["yhje"]) != 0)
                //                itemdetail[m].ItemYhXmMx = rowsdetail[m]["PM"].ToString().Trim() + _fpyhmc1 + ":" + rowsdetail[m]["yhje"].ToString().Trim() + "" + _fpyhmc2 + ":" + (Convert.ToDecimal(rowsdetail[m]["je"]) - Convert.ToDecimal(rowsdetail[m]["yhje"])).ToString().Trim() + "";
                //        }
                //        invoice.ItemDetail = itemdetail;
                //        //invoice.YhXmMx = "统筹支付:" + yhje.ToString().Trim() + "\n 个人自付:" + (zje - yhje).ToString().Trim();

                //        if (Bview != "true")
                //            invoice.Print();
                //        else
                //            invoice.Preview();
                //    }
                //    if (cfg1068.Config == "1")//Add By Zj 2013-01-16 导引单
                //    {

                //        ParameterEx[] paramters = new ParameterEx[7];
                //        paramters[0].Text = "医院名称";
                //        paramters[0].Value = Constant.HospitalName;

                //        paramters[1].Text = "姓名";
                //        paramters[1].Value = txtxm.Text.Trim();

                //        paramters[2].Text = "卡号";
                //        paramters[2].Value = txtkh.Text.Trim();

                //        paramters[3].Text = "门诊号";
                //        paramters[3].Value = txtmzh.Text.Trim();

                //        paramters[4].Text = "科室";
                //        paramters[4].Value = txtks.Text.Trim();

                //        paramters[5].Text = "就诊日期";
                //        paramters[5].Value = sDate;

                //        paramters[6].Text = "科室信息";
                //        paramters[6].Value = szxks;


                //        TrasenFrame.Forms.FrmReportView f;
                //        f = new TrasenFrame.Forms.FrmReportView(null, Constant.ApplicationDirectory + "\\Report\\MZ_导引单.rpt", paramters, true);
                //    }

                //}
                //catch (System.Exception err)
                //{

                //    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                #endregion
            }
            else if (cfg1046.Config == "2")
            {
                //MessageBox.Show(dset.Tables.Count.ToString());
                //MessageBox.Show("医保账户支付(ybzhzf):" + ybzhzf + "" + "医保统筹支付(ybjjzf):" + ybjjzf + "" + "医保其他支付(ybbzzf):" + ybbzzf + "");
                PrintSmallReport(dset, sDate, klx, xjzf, ye, ybzje, ybzhzf, yblx, ybjjzf, ybbzzf, ylkzf, cwjz);
            }

            #region 刷新未收费处方、并产生当前可用发票号
            string Now_mzh = "";
            try
            {
                ybkye = ybkye - ybzhzf;
                lblybkye.Text = ybkye.ToString();
                if (Convert.ToDecimal(Convertor.IsNull(lblybkye.Text, "0")) < 0)
                    lblybkye.Text = "0";

                //刷新未收费的处方
                butref_Click(sender, e);
                butsf.Enabled = true;
                plSyxx.Visible = true;

                //提示发票段已经用完
                if (Msg.Trim() != "")
                {
                    MessageBox.Show(Msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtfph.Text = "无可用发票";
                    return;
                }

                //获得可用发票号
                txtfph.Text = "";
                //Modify By Zj 2013-01-10 动态获取发票类型 不再写死为1 收费发票 为了方便获取收据.
                DataTable tab = Fun.GetFph(InstanceForm.BCurrentUser.EmployeeId, 1, ts_mz_class.mz_sf.GetFpLx(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase), out err_code, out err_text, InstanceForm.BDatabase);
                if (tab.Rows.Count == 0 || err_code != 0)
                {
                    if (cfg1046.Config == "1")//只有打发票时才判断 Modify by zouchihua 2013-4-23)
                    {
                        MessageBox.Show(err_text, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (tab.Rows.Count > 0)//Modify by zouchihua 2013-4-28
                  txtfph.Text = Convertor.IsNull(tab.Rows[0]["QZ"], "") + tab.Rows[0]["fph"].ToString().Trim();

                if (card.bjebz == true)
                {
                    readcard = new ReadCard(readcard.kdjid, InstanceForm.BDatabase);
                    lblkye.Text = readcard.kye.ToString();
                }

                if (Bkh == "true")
                    txtkh.Focus();
                else
                    txtmzh.Focus();


                tb = (DataTable)dataGridView1.DataSource;
                Now_mzh = this.txtmzh.Text.Trim();
                if (tb.Rows.Count == 0)
                    ClearForm();


                plSyxx.Visible = true;
            }
            catch (System.Exception err)
            {
                MessageBox.Show("收费已成功,但发生下列错误" + err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            //刷新药品处方数 
            //Getypcfs(readcard.kdjid);
            //Add By Zj 2012-08-16 22:03
            string bqypjq = ApiFunction.GetIniString("评价器", "启用评价器", Constant.ApplicationDirectory + "//ClientWindow.ini");
            string pjqxh = ApiFunction.GetIniString("评价器", "评价器型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
            #region  //Modify By zp 2013-07-25 用全局变量获取当前门诊号 针对评价器判断
            if (DqMzh != Now_mzh)//txtmzh.Text.Trim())//Add By Zj 2013-03-04 用于判断是否一个病人多张处方 重复调用 评价器。
            {
                DqMzh = Now_mzh;//txtmzh.Text.Trim();

                if (bqypjq == "true")
                {
                    ts_pjq.ipjq ipjq = ts_pjq.PjqFactory.Newpjq( pjqxh );
                    string perr_text = "";
                    int perr_code = 0;
                    ipjq.Pj( InstanceForm.BCurrentUser.LoginCode.ToString(), InstanceForm.BCurrentUser.Name, InstanceForm.BCurrentDept.DeptId.ToString(), InstanceForm.BCurrentDept.DeptName, out perr_code, out perr_text );
                    if (perr_code != 0)
                        throw new Exception( "评价器调用出错!" + perr_text );
                }
            }
            #endregion

        }
        /// <summary>
        /// 打印收费小票 Add By zp 2013-08-30 重构
        /// </summary>
        /// <param name="dset"></param>
        /// <param name="sDate">收费时间</param>
        /// <param name="klx">卡类型</param>
        /// <param name="xjzf">现金支付</param>
        /// <param name="ye">余额</param>
        /// <param name="yblx">医保类型</param>
        /// <param name="ybzje">医保总金额</param>
        /// <param name="ylkzf">银联支付</param>
        /// <param name="ylkzf">卡支付－财务记帐</param>
        private void PrintSmallReport(DataSet dset, string sDate, int klx, decimal xjzf, decimal ye, decimal ybzje, decimal ybzhzf, Yblx yblx, decimal ybjjzf, decimal ybbzzf, decimal ylkzf, decimal cwjz)
        {
            #region 打印收费小票
            try
            {
                 decimal ybye = decimal.Parse(Convertor.IsNull(lblybkye.Text.Trim(), "0"));
                //MessageBox.Show("ybzje:" + ybzje + "ybzhzf:" + ybzhzf + "ybye:" + ybye);
                string ssql = "";

                decimal zfqkye = ye;
               
                /*add by zch 2013-03-26 门诊小票打印是否打在一张上（只切纸一次）0=否 ，1=是 */
                if (new SystemCfg(1078).Config.Trim() == "1" && dset.Tables[0].Rows.Count > 0)
                {
                    DataTable dtFpxm = dset.Tables[1].Clone();
                    dtFpxm.TableName = "收费明细";
                    DataTable dtFpwjxm = dset.Tables[4].Clone();
                    dtFpwjxm.TableName = "收费物价明细";
                    //复制一个表数据
                    DataTable
                        tableXpmx = dset.Tables[5].Copy();
                    tableXpmx.TableName = "小票明细";
                    //Modify by zouchihua 2013-3-26 门诊小票打印
                    #region 只打一张小票

                    decimal cwjzhj = 0;
                    decimal _xhje = 0;//消费金额
                    decimal _yhje = 0;//优惠金额

                    // decimal _zje=0;//总金额
                    string zhdnlsh = "";//考虑到电脑流水号有多个

                    for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)
                    {

                        _xhje += decimal.Parse(dset.Tables[0].Rows[X]["zje"].ToString());//消费金额
                        _yhje += decimal.Parse(dset.Tables[0].Rows[X]["yhje"].ToString());//优惠金额

                        //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                        string fyck = "";

                        mz_sf.UpdateFyck(Dqcf.brxxid, new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()), out fyck, InstanceForm.BDatabase);

                        ssql = "select * from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                        DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                        string _sHjid = dset.Tables[0].Rows[X]["hjid"].ToString().Trim();
                        _sHjid = _sHjid.Replace("'", "''");

                        DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                        cwjzhj = cwjzhj + Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);
                        foreach (DataRow dr in rows)
                            dtFpxm.Rows.Add(dr.ItemArray);

                        DataRow[] rowsdetail = dset.Tables[4].Select("hjid='" + _sHjid + "'");
                        if (dset.Tables[0].Rows.Count - 1 == X)
                            zhdnlsh = dset.Tables[0].Rows[X]["dnlsh"].ToString();

                        foreach (DataRow dr in rowsdetail)
                            dtFpwjxm.Rows.Add(dr.ItemArray);

                        zhdnlsh += dset.Tables[0].Rows[X]["dnlsh"].ToString() + "\r\n";//多个进行累加

                    }
                    ye = ye - cwjzhj;//放到这里计算
                    
                    #region
                    //读取病人卡余额 重新获得卡 并且获余额 Modify By zp 2013-08-22  医保支付显示医保卡余额
                    if (!yblx.issf) //如果是自付则显示医院诊疗卡余下金额
                    {
                        ReadCard readcard1 = new ReadCard(klx, lblkh.Text.Trim(), InstanceForm.BDatabase);
                        ye = readcard1.kye;
                    }
                    else  //如果是医保支付则显示 医保卡余额 
                    {
                        if (ybzhzf > 0 && lblybkye.Text.Trim() != "" )
                        {
                            ye = 0;
                            ybye = Convert.ToDecimal(lblybkye.Text.Trim()) - ybzhzf;
                        }
                    }

                    ParameterEx[] paramters = new ParameterEx[16];
                    paramters[0].Text = "V_医院名称";
                    paramters[0].Value = Constant.HospitalName;

                    paramters[1].Text = "V_收费日期";
                    paramters[1].Value = sDate;

                    paramters[2].Text = "V_收费员";
                    paramters[2].Value = InstanceForm.BCurrentUser.Name;

                    paramters[3].Text = "V_病人姓名";
                    paramters[3].Value = plSyxx.上一病人;// lbl_xm.Text;

                    paramters[4].Text = "V_门诊号";
                    paramters[4].Value = dset.Tables[0].Rows[0]["blh"].ToString();

                    paramters[5].Text = "V_卡号";
                    paramters[5].Value = txtkh.Text;

                    paramters[6].Text = "V_电脑流水号";
                    paramters[6].Value = zhdnlsh;// +" -" + zhdnlsh;

                    paramters[7].Text = "V_消费金额";
                    paramters[7].Value = _xhje;
    
                    paramters[8].Text = "V_卡余额";
                    paramters[8].Value = ye;
                    paramters[9].Text = "V_医生";
                    paramters[9].Value = txtys.Text;
                    paramters[10].Text = "V_科室";
                    paramters[10].Value = txtks.Text;

                    paramters[11].Text = "V_优惠金额";
                    paramters[11].Value = _yhje.ToString();
                    //add by zouchihua 2013-3-26
                    paramters[12].Text = "V_现金支付";
                    paramters[12].Value = xjzf.ToString();//直接获取收银窗口的值
                    //add by zouchihua 2013-3-26
                    paramters[13].Text = "V_医保支付";
                    paramters[13].Value = ybzje.ToString();//直接获取收银窗口的值
                    //add by zouchihua 2013-3-26
                    paramters[14].Text = "V_其它支付";
                    paramters[14].Value = Convert.ToString(_xhje - xjzf - ybzje);//直接获取收银窗口的值
                    //Add by zp 2013-08-30
                    paramters[15].Text = "V_医保余额";
                    paramters[15].Value = ybye.ToString();

                    #endregion


                    DataSet _dset = new DataSet();
                    _dset.Tables.Add(dtFpxm);
                    _dset.Tables.Add(dtFpwjxm);
                    _dset.Tables.Add(tableXpmx);


                    string reportFile = Constant.ApplicationDirectory + "\\Report\\MZSF_小票(只打一张).rpt";
                    TrasenFrame.Forms.FrmReportView fView = new FrmReportView(_dset, reportFile, paramters, true);
                    TrasenFrame.Forms.FrmReportView fView1 = new FrmReportView(_dset, reportFile, paramters, true);//add by zouchihua 2013-3-27 打两份
                    #endregion
                }
                else
                {
                    for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)
                    {
                        //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                        string fyck = "";
                        mz_sf.UpdateFyck(Dqcf.brxxid, new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()), out fyck, InstanceForm.BDatabase);

                        ssql = "select * from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                        DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                        //引导地址
                        int zxks = Convert.ToInt32(dset.Tables[0].Rows[X]["zxks"]);

                        DataRow deptdr = null;
                        ssql = "select NAME,DEPTADDR from  JC_DEPT_PROPERTY where dept_id=" + zxks.ToString();
                        if (zxks != 0)
                            deptdr = InstanceForm.BDatabase.GetDataRow(ssql);

                        string kswz = "";

                        if (deptdr != null && Convertor.IsNull(deptdr["DEPTADDR"], "") != "")
                            kswz = Convertor.IsNull(deptdr["DEPTADDR"], "");


                        ParameterEx[] paramters = new ParameterEx[19];
                        paramters[0].Text = "V_医院名称";
                        paramters[0].Value = Constant.HospitalName;

                        paramters[1].Text = "V_收费日期";
                        paramters[1].Value = sDate;

                        paramters[2].Text = "V_收费员";
                        paramters[2].Value = InstanceForm.BCurrentUser.Name;

                        paramters[3].Text = "V_病人姓名";
                        paramters[3].Value = plSyxx.上一病人;//lbl_xm.Text;

                        paramters[4].Text = "V_门诊号";
                        paramters[4].Value = dset.Tables[0].Rows[X]["blh"].ToString();

                        paramters[5].Text = "V_卡号";
                        paramters[5].Value = txtkh.Text;

                        paramters[6].Text = "V_电脑流水号";
                        paramters[6].Value = dset.Tables[0].Rows[X]["dnlsh"].ToString();

                        paramters[7].Text = "V_消费金额";
                        paramters[7].Value = dset.Tables[0].Rows[X]["zje"].ToString();


                        ye = ye - Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);


                        //医保支付显示医保卡余额
                        if (yblx.issf)
                        {
                            if (ybzhzf > 0 && lblybkye.Text.Trim() != "")
                            {
                                ybye = Convert.ToDecimal(lblybkye.Text.Trim()) - ybzhzf;
                            }
                        }


                        paramters[8].Text = "V_卡余额";
                        paramters[8].Value = ye;

                        paramters[9].Text = "V_医生";
                        paramters[9].Value = txtys.Text;

                        paramters[10].Text = "V_科室";
                        paramters[10].Value = txtks.Text;

                        paramters[11].Text = "V_优惠金额";
                        paramters[11].Value = dset.Tables[0].Rows[X]["yhje"].ToString();

                        //ADD BY JIANGZF 2014-03-14 GRBH
                        paramters[12].Text = "V_医保余额";
                        paramters[12].Value = ybye.ToString() == "0" ? "" : ybye.ToString();

                        paramters[13].Text = "V_个人帐户";
                        paramters[13].Value = ybzje.ToString();//直接获取收银窗口的值 医保支付

                        paramters[14].Text = "V_现金支付";
                        paramters[14].Value = xjzf.ToString();//直接获取收银窗口的值

                        paramters[15].Text = "V_银联支付";
                        paramters[15].Value = ylkzf.ToString();

                        paramters[16].Text = "V_卡支付";
                        paramters[16].Value = cwjz.ToString();

                        paramters[17].Text = "V_位置";
                        paramters[17].Value = kswz;

                        paramters[18].Text = "V_支付前卡余额";
                        paramters[18].Value = zfqkye;

                        

                        string _sHjid = dset.Tables[0].Rows[X]["hjid"].ToString().Trim();
                        _sHjid = _sHjid.Replace("'", "''");
 
                        DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                        DataTable dtFpxm = dset.Tables[1].Clone();
                        dtFpxm.TableName = "收费明细";
                        foreach (DataRow dr in rows)
                            dtFpxm.Rows.Add(dr.ItemArray);

                        DataRow[] rowsdetail = dset.Tables[4].Select("hjid='" + _sHjid + "'");
                        DataTable dtFpwjxm = dset.Tables[4].Clone();
                        dtFpwjxm.TableName = "收费物价明细";
                        foreach (DataRow dr in rowsdetail)
                            dtFpwjxm.Rows.Add(dr.ItemArray);
                        DataSet _dset = new DataSet();
                        _dset.Tables.Add(dtFpxm);
                        _dset.Tables.Add(dtFpwjxm);

                        string reportFile = Constant.ApplicationDirectory + "\\Report\\MZSF_小票.rpt";
                        TrasenFrame.Forms.FrmReportView fView = new FrmReportView(_dset, reportFile, paramters, true);
                        //fView.Show();
                    }
                }
            }
            catch (System.Exception err)
            {

                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #endregion
        }


        private void PrintBill(DataSet dset, int klx, mz_card card, string sDate,
    decimal ybkye, decimal ye, decimal ybbzzf, decimal ybzje, decimal ybzhzf,
    Yblx yblx, decimal ybjjzf, decimal xjzf, string sfck)
        {
            #region 打印发票
            try
            {
                string ssql = "";
                string szxks = "";//Add By Zj 2013-01-16
                if (cfg1096.Config.Trim() == "0")
                {
                    #region 收费打印发票不用水晶报表
                    for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)//循环发票张数
                    {
                        //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                        string fyck = "";
                        mz_sf.UpdateFyck(Dqcf.brxxid, new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()), out fyck, InstanceForm.BDatabase);

                        int ksdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ksdm"]);
                        int ysdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ysdm"]);
                        int zxks = Convert.ToInt32(dset.Tables[0].Rows[X]["zxks"]);

                        DataRow deptdr = null;
                        ssql = "select NAME,DEPTADDR from  JC_DEPT_PROPERTY where dept_id=" + zxks.ToString();

                        if(zxks != 0)
                            deptdr = InstanceForm.BDatabase.GetDataRow(ssql);

                        if (zxks != 0 && cfg1068.Config == "1")//Add By Zj 2013-01-16
                        {
                            szxks += deptdr["NAME"] + " 位置:" + Convertor.IsNull(deptdr["DEPTADDR"], "") + "\r\n";
                        }

                        ssql = "select * from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                        DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                        PrintClass.OPDInvoice invoice = new PrintClass.OPDInvoice();
                        invoice.OtherInfo = "";
                        invoice.HisName = Constant.HospitalName;  //医院名称
                        invoice.PatientName = txtxm.Text.Trim();  //病人姓名
                        invoice.OutPatientNo = lblmzh.Text.Trim(); //门诊号
                        invoice.DepartmentName = Fun.SeekDeptName(ksdm, InstanceForm.BDatabase);//科室名称
                        invoice.DoctorName = Fun.SeekEmpName(ysdm, InstanceForm.BDatabase); //医生名称

                        invoice.InvoiceNo = "电脑票号：" + Convert.ToString(dset.Tables[0].Rows[X]["fph"]);//电脑发票号
                        decimal dxxje = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]);
                        //通过参数设置大小写金额是否取自付金额 Add By zp 2014-02-24 
                        if (cfg1109.Config.Trim() == "1")
                            dxxje = dxxje - ybzje; //dset.Tables[0].Rows[X]["zfje"].ToString();
                        invoice.TotalMoneyCN = Money.NumToChn(dxxje.ToString());//总金额（大写）
                        invoice.TotalMoneyNum = Convert.ToDecimal(dxxje);//总金额（小写）
                        if (cfgsfy.Config == "1") //显示收款人姓名还是代码
                            invoice.Payee = InstanceForm.BCurrentUser.Name;  //收款人
                        else
                            invoice.Payee = InstanceForm.BCurrentUser.LoginCode;

                        DateTime time = Convert.ToDateTime(sDate);
                        invoice.Year = time.Year;
                        invoice.Month = time.Month;
                        invoice.Day = time.Day;

                        bool bqedy = mz_sf.Bqedy(new Guid(Convertor.IsNull(tbFp.Rows[0]["yhlxid"], Guid.Empty.ToString())), InstanceForm.BDatabase);

                        if (bqedy == true && Convert.ToDecimal(tbFp.Rows[0]["yhje"]) != 0)
                        {
                            invoice.Yhje = 0;
                            invoice.Qfgz = 0;
                            invoice.Ybzhzf = 0;
                            invoice.Ybjjzf = 0;
                            invoice.Ybbzzf = 0;
                            invoice.Cwjz = 0;
                            invoice.Ylkje = 0;
                            invoice.Srje = 0;
                            invoice.Xjzf = 0;
                            invoice.Zpzf = 0;
                        }
                        else
                        {
                            invoice.Yhje = Convert.ToDecimal(tbFp.Rows[0]["yhje"]);
                            invoice.Qfgz = Convert.ToDecimal(tbFp.Rows[0]["qfgz"]);
                            invoice.Ybzhzf = ybzhzf;
                            invoice.Ybjjzf = ybjjzf;
                            invoice.Ybbzzf = ybbzzf;
                            invoice.Cwjz = Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);
                            invoice.Ylkje = Convert.ToDecimal(tbFp.Rows[0]["ylkzf"]);
                            invoice.Srje = Convert.ToDecimal(tbFp.Rows[0]["srje"]);
                            invoice.Xjzf = Convert.ToDecimal(tbFp.Rows[0]["xjzf"]);
                            invoice.Zpzf = Convert.ToDecimal(tbFp.Rows[0]["Zpzf"]);
                        }

                        invoice.Zxks = Fun.SeekDeptName(zxks, InstanceForm.BDatabase);

                        ye = ye - invoice.Cwjz;
                        invoice.Kye = ye;

                        invoice.Ybkye = ybkye - ybzhzf;
                        if (invoice.Ybkye < 0)
                            invoice.Ybkye = 0;

                        invoice.Ybkh = lblybkh.Text.Trim();

                        invoice.Yblx = cmbyblx.Text.Trim();
                        invoice.Ybjydjh = jsxx.JSDH;
                        invoice.Klx = lblkh.Text.Trim() == "" ? "" : cmbklx.Text.Trim();
                        invoice.Klx_Bje = card.bjebz;

                        invoice.sfck = sfck;
                        invoice.fyck = fyck;
                        invoice.htdwlx = lblhtdwlx.Text.Trim();
                        invoice.htdwmc = lblhtdw.Text.Trim();
                        if (deptdr != null && Convertor.IsNull(deptdr["DEPTADDR"], "") !="")
                            invoice.kswz =  Convertor.IsNull(deptdr["DEPTADDR"], "");
                        else
                            invoice.kswz = "";

                        invoice.Klx = cmbklx.Text.Trim();
                        invoice.kh = txtkh.Text.Trim();
                        invoice.sfsj = Convert.ToDateTime(sDate).ToLongTimeString();
                        invoice.ghjb = Convertor.IsNull(label5.Tag.ToString(), "");//Add By Zj 2012-03-06 
                        invoice.Mzhtm = lblmzh.Text.Trim();//Add By Zj 2012-12-11 门诊号条码 方便LIS使用
                        PrintClass.InvoiceItem[] item = null;
                        PrintClass.InvoiceItemDetail[] itemdetail = null; //Modify By Tany 2008-12-20 增加发票明细项目

                        string _sHjid = dset.Tables[0].Rows[X]["hjid"].ToString().Trim();
                        _sHjid = _sHjid.Replace("'", "''");



                        DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                        item = new PrintClass.InvoiceItem[rows.Length];
                        for (int m = 0; m <= rows.Length - 1; m++)
                        {
                            item[m].ItemName = rows[m]["ITEM_NAME"].ToString().Trim();
                            item[m].ItemMoney = Convert.ToDecimal(rows[m]["je"]);//发票项目金额
                        }
                        invoice.Items = item;

                        string _fpyhmc1 = "";
                        string _fpyhmc2 = "";
                        _fpyhmc1 = ApiFunction.GetIniString("划价收费", "发票优惠项目名称1", Constant.ApplicationDirectory + "//ClientWindow.ini");
                        _fpyhmc2 = ApiFunction.GetIniString("划价收费", "发票优惠项目名称2", Constant.ApplicationDirectory + "//ClientWindow.ini");
                        if (_fpyhmc1 == "")
                            _fpyhmc1 = "慈善支付";
                        if (_fpyhmc2 == "")
                            _fpyhmc2 = "个人支付";
                        //增加发票明细项目
                        DataRow[] rowsdetail = dset.Tables[4].Select("hjid='" + _sHjid + "' and sl<>0");//Modify Bj Zj 2012-09-11
                        itemdetail = new PrintClass.InvoiceItemDetail[rowsdetail.Length];
                        for (int m = 0; m <= rowsdetail.Length - 1; m++)
                        {
                            itemdetail[m].ItemDetailName = rowsdetail[m]["PM"].ToString().Trim();
                            itemdetail[m].ItemDW = rowsdetail[m]["DW"].ToString().Trim();
                            itemdetail[m].ItemGG = rowsdetail[m]["GG"].ToString().Trim();
                            itemdetail[m].ItemJS = Convert.ToDecimal(rowsdetail[m]["JS"]);
                            itemdetail[m].ItemNum = Convert.ToDecimal(rowsdetail[m]["SL"]);
                            itemdetail[m].ItemPrice = Convert.ToDecimal(rowsdetail[m]["DJ"]);
                            itemdetail[m].ItemJE = Convert.ToDecimal(rowsdetail[m]["JE"]);
                            if (Convert.ToDecimal(rowsdetail[m]["yhje"]) != 0)
                                itemdetail[m].ItemYhXmMx = rowsdetail[m]["PM"].ToString().Trim() + _fpyhmc1 + ":" + rowsdetail[m]["yhje"].ToString().Trim() + "" + _fpyhmc2 + ":" + (Convert.ToDecimal(rowsdetail[m]["je"]) - Convert.ToDecimal(rowsdetail[m]["yhje"])).ToString().Trim() + "";
                        }
                        invoice.ItemDetail = itemdetail;
                        //invoice.YhXmMx = "统筹支付:" + yhje.ToString().Trim() + "\n 个人自付:" + (zje - yhje).ToString().Trim();

                        if (Bview != "true")
                            invoice.Print();
                        else
                            invoice.Preview();
                    }
                    #endregion
                }
                else  //打印水晶报表
                {

                    #region 收费打印发票用水晶报表
                    for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)//循环发票张数
                    {
                        //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                        string fyck = "";
                        mz_sf.UpdateFyck(Dqcf.brxxid, new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()), out fyck, InstanceForm.BDatabase);

                        int ksdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ksdm"]);
                        int ysdm = Convert.ToInt32(dset.Tables[0].Rows[X]["ysdm"]);
                        int zxks = Convert.ToInt32(dset.Tables[0].Rows[X]["zxks"]);
                        if (zxks != 0 && cfg1068.Config == "1")//Add By Zj 2013-01-16
                        {
                            ssql = "select NAME,DEPTADDR from  JC_DEPT_PROPERTY where dept_id=" + zxks.ToString();
                            DataRow deptdr = InstanceForm.BDatabase.GetDataRow(ssql);
                            szxks += deptdr["NAME"] + " 位置:" + Convertor.IsNull(deptdr["DEPTADDR"], "") + "\r\n";
                        }

                        ssql = "select * from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                        DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                        ParameterEx[] paramters = new ParameterEx[34];


                        paramters[0].Text = "V_医院名称";
                        paramters[0].Value = Constant.HospitalName;

                        paramters[1].Text = "V_病人姓名";
                        paramters[1].Value = txtxm.Text.Trim();

                        paramters[2].Text = "V_门诊号";
                        paramters[2].Value = lblmzh.Text.Trim();

                        paramters[3].Text = "V_科室名称";
                        paramters[3].Value = Fun.SeekDeptName(ksdm, InstanceForm.BDatabase);

                        paramters[4].Text = "V_医生名称";
                        paramters[4].Value = Fun.SeekEmpName(ysdm, InstanceForm.BDatabase);

                        paramters[5].Text = "V_电脑票号";
                        paramters[5].Value = "电脑票号：" + Convert.ToString(dset.Tables[0].Rows[X]["fph"]);

                        paramters[6].Text = "V_大写总金额";
                        paramters[6].Value = Money.NumToChn(dset.Tables[0].Rows[X]["zje"].ToString());

                        paramters[7].Text = "V_小写总金额";
                        paramters[7].Value = Convert.ToDecimal(dset.Tables[0].Rows[X]["zje"]);

                        paramters[8].Text = "V_收款人";
                        if (cfgsfy.Config == "1") //显示收款人姓名还是代码
                            paramters[8].Value = InstanceForm.BCurrentUser.Name;  //收款人
                        else
                            paramters[8].Value = InstanceForm.BCurrentUser.LoginCode;


                        bool bqedy = mz_sf.Bqedy(new Guid(Convertor.IsNull(tbFp.Rows[0]["yhlxid"], Guid.Empty.ToString())), InstanceForm.BDatabase);

                        if (bqedy == true && Convert.ToDecimal(tbFp.Rows[0]["yhje"]) != 0)
                        {

                            paramters[9].Text = "V_优惠金额";
                            paramters[9].Value = 0;

                            paramters[10].Text = "V_欠费挂账";
                            paramters[10].Value = 0;

                            paramters[11].Text = "V_医保账户支付";
                            paramters[11].Value = 0;

                            paramters[12].Text = "V_医保基金支付";
                            paramters[12].Value = 0;

                            paramters[13].Text = "V_医保补助支付";
                            paramters[13].Value = 0;

                            paramters[14].Text = "V_财务记账";
                            paramters[14].Value = 0;

                            paramters[15].Text = "V_银联卡金额";
                            paramters[15].Value = 0;

                            paramters[16].Text = "V_舍入金额";
                            paramters[16].Value = 0;

                            paramters[17].Text = "V_现金支付";
                            paramters[17].Value = 0;

                            paramters[18].Text = "V_支票支付";
                            paramters[18].Value = 0;
                        }
                        else
                        {

                            paramters[9].Text = "V_优惠金额";
                            paramters[9].Value = Convert.ToDecimal(tbFp.Rows[0]["yhje"]);

                            paramters[10].Text = "V_欠费挂账";
                            paramters[10].Value = Convert.ToDecimal(tbFp.Rows[0]["qfgz"]);

                            paramters[11].Text = "V_医保账户支付";
                            paramters[11].Value = ybzhzf;

                            paramters[12].Text = "V_医保基金支付";
                            paramters[12].Value = ybjjzf;

                            paramters[13].Text = "V_医保补助支付";
                            paramters[13].Value = ybbzzf;

                            paramters[14].Text = "V_财务记账";
                            paramters[14].Value = Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);

                            paramters[15].Text = "V_银联卡金额";
                            paramters[15].Value = Convert.ToDecimal(tbFp.Rows[0]["ylkzf"]);

                            paramters[16].Text = "V_舍入金额";
                            paramters[16].Value = Convert.ToDecimal(tbFp.Rows[0]["srje"]);

                            paramters[17].Text = "V_现金支付";
                            paramters[17].Value = Convert.ToDecimal(tbFp.Rows[0]["xjzf"]);

                            paramters[18].Text = "V_支票支付";
                            paramters[18].Value = Convert.ToDecimal(tbFp.Rows[0]["Zpzf"]);
                        }

                        paramters[19].Text = "V_执行科室";
                        paramters[19].Value = Fun.SeekDeptName(zxks, InstanceForm.BDatabase);
                        ye = ye - Convert.ToDecimal(tbFp.Rows[0]["cwjz"]); //invoice.Cwjz;

                        paramters[20].Text = "V_卡余额";
                        paramters[20].Value = ye;

                        decimal _ybkye = ybkye - ybzhzf;
                        paramters[21].Text = "V_医保卡余额";
                        paramters[21].Value = _ybkye;

                        if (_ybkye < 0)
                            paramters[21].Value = 0;

                        paramters[22].Text = "V_医保卡号";
                        paramters[22].Value = lblybkh.Text.Trim();

                        paramters[23].Text = "V_医保类型";
                        paramters[23].Value = cmbyblx.Text.Trim();

                        paramters[24].Text = "V_结算单号";
                        paramters[24].Value = jsxx.JSDH;

                        paramters[25].Text = "V_卡类型";
                        paramters[25].Value = lblkh.Text.Trim() == "" ? "" : cmbklx.Text.Trim();

                        paramters[26].Text = "V_收费窗口";
                        paramters[26].Value = sfck;

                        paramters[27].Text = "V_发药窗口";
                        paramters[27].Value = fyck;

                        paramters[28].Text = "V_合同单位类型";
                        paramters[28].Value = lblhtdwlx.Text.Trim();

                        paramters[29].Text = "V_合同单位名称";
                        paramters[29].Value = lblhtdw.Text.Trim();

                        paramters[30].Text = "V_卡号";
                        paramters[30].Value = txtkh.Text.Trim();

                        paramters[31].Text = "V_收费时间";
                        paramters[31].Value = Convert.ToDateTime(sDate);

                        paramters[32].Text = "V_挂号级别";
                        paramters[32].Value = Convertor.IsNull(label5.Tag.ToString(), "");

                        paramters[33].Text = "V_备注";
                        paramters[33].Value = "";

                        

                        DataTable dt_fpmx = dset.Tables[1].Copy();//得到发票项目明细 
                        dt_fpmx.TableName = "发票明细";

                        //增加发票收费明细项目
                        DataTable dt_sfmx = dset.Tables[4].Copy();
                        dt_sfmx.TableName = "项目明细";

                        DataSet _dset = new DataSet();
                        _dset.Tables.Add(dt_fpmx);
                        //_dset.Tables.Add(dt_sfmx);

                        //modify by jiangzf 根据参数1028限制打印发票明细条数
                        int dt_sfmx_num = Convert.ToInt32(Convertor.IsNull(cfg1028.Config, "20"));
                        if (dt_sfmx.Rows.Count > dt_sfmx_num)
                        {
                            _dset.Tables.Add(DtSelectTop(dt_sfmx_num, dt_sfmx));
                            //paramters[34].Value = "清单未打完，共" + dt_sfmx.Rows.Count.ToString() + "条记录";
                            
                        }
                        else
                        {
                            _dset.Tables.Add(dt_sfmx);
                        }

                        string reportFile = Constant.ApplicationDirectory + "\\Report\\MZSF_正式发票.rpt";
                        bool isbview = true; //是否直接打印
                        if (Bview == "true")
                            isbview = false;
                        FrmReportView fView = new FrmReportView(_dset, reportFile, paramters, isbview);
                        if (!isbview)
                            fView.Show();
                    }
                    #endregion
                    //decimal ybye = decimal.Parse(Convertor.IsNull(lblybkye.Text.Trim(), "0"));
                    #region 只打一张小票

                    // decimal cwjzhj = 0;
                    //decimal _xhje = 0;//消费金额
                    //decimal _yhje = 0;//优惠金额

                    // decimal _zje=0;//总金额
                    //string zhdnlsh = "";//考虑到电脑流水号有多个

                    //for (int X = 0; X <= dset.Tables[0].Rows.Count - 1; X++)
                    //{
                    //    DataTable dtFpxm = dset.Tables[1].Clone();
                    //    dtFpxm.TableName = "发票明细";
                    //    DataTable dtFpwjxm = dset.Tables[4].Clone();
                    //    dtFpwjxm.TableName = "项目明细";

                    //    decimal _xhje = decimal.Parse(dset.Tables[0].Rows[X]["zje"].ToString());//消费金额
                    //    decimal _yhje = decimal.Parse(dset.Tables[0].Rows[X]["yhje"].ToString());//优惠金额

                    //    //更新发药窗口 由于存在多执行科室不分票的情况 具体发药窗口输出请修改 sp_yf_MZSF_FYCK
                    //    string fyck = "";

                    //    mz_sf.UpdateFyck(Dqcf.brxxid, new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()), out fyck, InstanceForm.BDatabase);

                    //    ssql = "select * from mz_fpb  (nolock)  where fpid='" + new Guid(dset.Tables[0].Rows[X]["fpid"].ToString()) + "'";
                    //    DataTable tbFp = InstanceForm.BDatabase.GetDataTable(ssql);

                    //    string _sHjid = dset.Tables[0].Rows[X]["hjid"].ToString().Trim();
                    //    _sHjid = _sHjid.Replace("'", "''");

                    //    DataRow[] rows = dset.Tables[1].Select("hjid='" + _sHjid + "'");
                    //    decimal cwjzhj = Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);//cwjzhj +
                    //    foreach (DataRow dr in rows)
                    //        dtFpxm.Rows.Add(dr.ItemArray);

                    //    DataRow[] rowsdetail = dset.Tables[4].Select("hjid='" + _sHjid + "'");
                    //    if (dset.Tables[0].Rows.Count - 1 == X)
                    //        zhdnlsh = dset.Tables[0].Rows[X]["dnlsh"].ToString();

                    //    foreach (DataRow dr in rowsdetail)
                    //        dtFpwjxm.Rows.Add(dr.ItemArray);

                    //    zhdnlsh += dset.Tables[0].Rows[X]["dnlsh"].ToString() + "\r\n";//多个进行累加
                    //    ye = ye - cwjzhj;//放到这里计算

                    //    #region
                    //    //读取病人卡余额 重新获得卡 并且获余额 Modify By zp 2013-08-22  医保支付显示医保卡余额
                    //    if (!yblx.issf) //如果是自付则显示医院诊疗卡余下金额
                    //    {
                    //        ReadCard readcard1 = new ReadCard(klx, lblkh.Text.Trim(), InstanceForm.BDatabase);
                    //        ye = readcard1.kye;
                    //    }
                    //    else  //如果是医保支付则显示 医保卡余额 
                    //    {
                    //        if (ybzhzf > 0 && lblybkye.Text.Trim() != "")
                    //        {
                    //            ye = 0;
                    //            ybye = Convert.ToDecimal(lblybkye.Text.Trim()) - ybzhzf;
                    //        }
                    //    }

                    //    ParameterEx[] paramters = new ParameterEx[17];
                    //    paramters[0].Text = "V_医院名称";
                    //    paramters[0].Value = Constant.HospitalName;

                    //    paramters[1].Text = "V_收费日期";
                    //    paramters[1].Value = sDate;

                    //    paramters[2].Text = "V_收费员";
                    //    paramters[2].Value = InstanceForm.BCurrentUser.Name;

                    //    paramters[3].Text = "V_病人姓名";
                    //    paramters[3].Value = plSyxx.上一病人;// lbl_xm.Text;

                    //    paramters[4].Text = "V_门诊号";
                    //    paramters[4].Value = dset.Tables[0].Rows[0]["blh"].ToString();

                    //    paramters[5].Text = "V_卡号";
                    //    paramters[5].Value = txtkh.Text;

                    //    paramters[6].Text = "V_电脑流水号";
                    //    paramters[6].Value = zhdnlsh;// +" -" + zhdnlsh;

                    //    paramters[7].Text = "V_消费金额";
                    //    paramters[7].Value = _xhje;

                    //    paramters[8].Text = "V_卡余额";
                    //    paramters[8].Value = ye;
                    //    paramters[9].Text = "V_医生";
                    //    paramters[9].Value = txtys.Text;
                    //    paramters[10].Text = "V_科室";
                    //    paramters[10].Value = txtks.Text;

                    //    paramters[11].Text = "V_优惠金额";
                    //    paramters[11].Value = _yhje.ToString();
                    //    //add by zouchihua 2013-3-26
                    //    paramters[12].Text = "V_现金支付";
                    //    paramters[12].Value = Convert.ToDecimal(tbFp.Rows[0]["xjzf"]);//xjzf.ToString();//直接获取收银窗口的值
                    //    //add by zouchihua 2013-3-26
                    //    paramters[13].Text = "V_医保支付";
                    //    paramters[13].Value = ybzje.ToString();//直接获取收银窗口的值
                    //    //add by zouchihua 2013-3-26
                    //    //paramters[14].Text = "V_其它支付";
                    //    //paramters[14].Value = Convert.ToString(_xhje - xjzf - ybzje);//直接获取收银窗口的值
                    //    //Add by zp 2013-08-30
                    //    paramters[14].Text = "V_医保余额";
                    //    paramters[14].Value = ybye.ToString();

                    //    paramters[15].Text = "V_大写金额";
                    //    paramters[15].Value = Money.NumToChn(_xhje.ToString()); //ybye.ToString();
                    //    #endregion


                    //    DataSet _dset = new DataSet();
                    //    _dset.Tables.Add(dtFpxm);
                    //    _dset.Tables.Add(dtFpwjxm);
                    //    string reportFile = Constant.ApplicationDirectory + "\\Report\\MZSF_正式发票.rpt";
                    //    bool isbview = true; //是否直接打印
                    //    if (Bview != "true")
                    //        isbview = false;
                    //    FrmReportView fView = new FrmReportView(_dset, reportFile, paramters, isbview);

                    //}

                    #endregion

                }

                if (cfg1068.Config == "1")//Add By Zj 2013-01-16 导引单
                {

                    ParameterEx[] paramters = new ParameterEx[7];
                    paramters[0].Text = "医院名称";
                    paramters[0].Value = Constant.HospitalName;

                    paramters[1].Text = "姓名";
                    paramters[1].Value = txtxm.Text.Trim();

                    paramters[2].Text = "卡号";
                    paramters[2].Value = txtkh.Text.Trim();

                    paramters[3].Text = "门诊号";
                    paramters[3].Value = txtmzh.Text.Trim();

                    paramters[4].Text = "科室";
                    paramters[4].Value = txtks.Text.Trim();

                    paramters[5].Text = "就诊日期";
                    paramters[5].Value = sDate;

                    paramters[6].Text = "科室信息";
                    paramters[6].Value = szxks;


                    TrasenFrame.Forms.FrmReportView f;
                    f = new TrasenFrame.Forms.FrmReportView(null, Constant.ApplicationDirectory + "\\Report\\MZ_导引单.rpt", paramters, true);
                }

            }
            catch (System.Exception err)
            {

                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion
        }



        #region 获取DataTable前几条数据  .
        /// <summary>  
        /// 获取DataTable前几条数据  
        /// </summary>  
        /// <param name="TopItem">前N条数据</param>  
        /// <param name="oDT">源DataTable</param>  
        /// <returns></returns>  
        public static DataTable DtSelectTop(int TopItem, DataTable oDT)  
        {  
            if (oDT.Rows.Count < TopItem) return oDT;  
          
            DataTable NewTable = oDT.Clone();  
            DataRow[] rows = oDT.Select("1=1");  
            for (int i = 0; i < TopItem; i++)  
            {  
                NewTable.ImportRow((DataRow)rows[i]);  
            }
            DataRow row = NewTable.NewRow();
            row["PM"] = "清单未打完，共" + oDT.Rows.Count.ToString() + "条记录";
            NewTable.Rows.Add(row);
            return NewTable;  
        }  
        #endregion



        private void txtkh_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == 22)
                {
                    txtkh.Text = "";
                    e.Handled = true;
                    return;
                }

                if ((int)e.KeyChar == 13)
                {
                    #region 敲入回车
                    txtkh.Text = ToDBC(txtkh.Text);
                    string kh = txtkh.Text.Trim();
                    int klx = Convert.ToInt32(Convertor.IsNull(cmbklx.SelectedValue, "0"));

                    ClearForm();
                    cmbklx.SelectedValue = klx.ToString();
                    GetBrxx("", klx, kh, "", "");
                    if (Dqcf.ghxxid == Guid.Empty) return;
                    if (Convertor.IsNull(txtys.Tag, "0") != "0" && txtys.Text.Trim() != "")
                        butnew_Click(sender, e);
                    else
                    {
                        if (txtys.Enabled == true)
                            txtys.Focus();
                        else
                            butnew_Click(sender, e);
                    }
                    #endregion
                }
             
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void txtxm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar != 13) return;

            if ((txtys.Text.Trim() != "" && Convert.ToInt32(Convertor.IsNull(txtys.Tag, "0")) > 0) || _menuTag.Function_Name == "Fun_ts_mz_hj_Lg")
                butnew_Click(sender, e);
            else
                txtys.Focus();
        }

        private void txtys_KeyPress(object sender, KeyPressEventArgs e)
        {
            Control control = (Control)sender;

            if ((int)e.KeyChar == 8)
            {
                txtys.Tag = "0";
                txtys.Text = "";
                lblzyks.Text = "";
                lblzyks.Tag = "0";
                txtks.Text = "";
                txtks.Tag = "0";
                e.Handled = true;
                return;
            }

            if ((int)e.KeyChar == 13 && Convertor.IsNumeric(txtys.Text.Trim()) == true)
            {
                DataRow[] rows = Tbys.Select("code='" + txtys.Text.Trim() + "'", "");
                if (rows.Length == 1)
                {
                    txtys.Tag = rows[0]["employee_id"].ToString();
                    txtys.Text = rows[0]["name"].ToString().Trim();

                    int zyksid = Fun.GetDocZyks(Convert.ToInt32(rows[0]["employee_id"]), _menuTag.Jgbm, InstanceForm.BDatabase);
                    lblzyks.Text = Fun.SeekDeptName(zyksid, InstanceForm.BDatabase);
                    lblzyks.Tag = zyksid.ToString();

                    int mzksid = Fun.GetDocMzks(Convert.ToInt32(rows[0]["employee_id"]), _menuTag.Jgbm, InstanceForm.BDatabase);
                    txtks.Text = Fun.SeekDeptName(mzksid, InstanceForm.BDatabase);
                    txtks.Tag = mzksid.ToString();
                    if (mzksid == 0)
                    {
                        txtks.Focus();
                        return;
                    }
                    txtks.Focus();
                    txtks.SelectAll();
                     
                    //if (Convert.ToInt32(rows[0]["employee_id"]) != 0)
                     //   butnew_Click(sender, e);
                    e.Handled = true;

                    return;
                }
                else
                {
                    txtys.Tag = "0";
                    txtys.Text = "";
                    lblzyks.Text = "";
                    lblzyks.Tag = "0";
                    txtks.Text = "";
                    txtks.Tag = "0";
                    e.Handled = true;
                    return;
                }
            }


            if ((int)e.KeyChar != 13 && Convertor.IsNumeric(e.KeyChar.ToString()) == false)
            {

                string[] headtext = new string[] { "医生姓名", "代码", "工号", "拼音码", "五笔码", "employee_id" };
                string[] mappingname = new string[] { "name", "ys_code", "code", "py_code", "wb_code", "employee_id" };
                string[] searchfields = new string[] { "ys_code", "py_code", "wb_code", "code" };//, "code" Modify By Tany 2008-12-19 不一定有工号
                int[] colwidth = new int[] { 100, 75, 75, 75, 75, 0 };
                TrasenFrame.Forms.FrmSelectCard f = new FrmSelectCard(searchfields, headtext, mappingname, colwidth);
                f.sourceDataTable = Tbys;
                f.WorkForm = this;
                f.srcControl = txtys;
                f.Font = txtks.Font;
                f.Width = 400;
                f.ReciveString = e.KeyChar.ToString();
                e.Handled = true;
                if (f.ShowDialog() == DialogResult.Cancel)
                {
                    txtys.Focus();
                }
                else
                {
                    txtys.Tag = Convert.ToInt32(f.SelectDataRow["employee_id"]);
                    txtys.Text = f.SelectDataRow["name"].ToString().Trim();

                    int zyksid = Fun.GetDocZyks(Convert.ToInt32(f.SelectDataRow["employee_id"]), _menuTag.Jgbm, InstanceForm.BDatabase);
                    lblzyks.Text = Fun.SeekDeptName(zyksid, InstanceForm.BDatabase);
                    lblzyks.Tag = zyksid.ToString();

                    int mzksid = Fun.GetDocMzks(Convert.ToInt32(f.SelectDataRow["employee_id"]), _menuTag.Jgbm, InstanceForm.BDatabase);
                    txtks.Text = Fun.SeekDeptName(mzksid, InstanceForm.BDatabase);
                    txtks.Tag = mzksid.ToString();
                    if (mzksid == 0)
                    {
                        txtks.Focus();
                        return;
                    }
                    txtks.Focus();
                    txtks.SelectAll();
                    //if (Convert.ToInt32(f.SelectDataRow["employee_id"]) != 0)
                     //   butnew_Click(sender, e);
                    e.Handled = true;
                }
            }

            if ((int)e.KeyChar == 13)
            {
                txtks.Focus();
                txtks.SelectAll();
               // if (Convert.ToInt32(txtys.Tag) == 0 || txtys.Text.Trim() == "") return;
                // butnew_Click(sender, e);
            }
        }
        private SystemCfg cfg1087 = new SystemCfg(1087);
        private void txtks_KeyPress(object sender, KeyPressEventArgs e)
        {
            Control control = (Control)sender;
            if ((int)e.KeyChar != 13)
            {
                string[] headtext = new string[] { "科室名称", "数字码", "拼音码", "dept_id" };
                string[] mappingname = new string[] { "name", "d_code", "py_code", "dept_id" };
                string[] searchfields = new string[] { "d_code", "py_code", "wb_code" };
                int[] colwidth = new int[] { 150, 100, 100, 0 };
                TrasenFrame.Forms.FrmSelectCard f = new FrmSelectCard(searchfields, headtext, mappingname, colwidth);
                //Modify by zouchihua 根据医生所在科室显示科室 2013-5-22
                 DataTable temp = Tbks.Copy();
                 if (cfg1087.Config.Trim() == "1")
                 {
                     if (!(txtys.Tag == null || txtys.Tag.ToString() == "0"))
                     {
                         DataTable Ys_dept = FrmMdiMain.Database.GetDataTable("select EMPLOYEE_ID,DEPT_ID from JC_EMP_DEPT_ROLE where EMPLOYEE_ID=" + txtys.Tag + " ");
                         string tj = " DEPT_ID in (0,";
                         for (int i = 0; i < Ys_dept.Rows.Count; i++)
                         {
                             tj += Ys_dept.Rows[i]["DEPT_ID"].ToString() + ",";
                         }
                         tj = tj.Substring(0, tj.Length - 1) + ")";
                         temp.DefaultView.RowFilter = tj;
                         temp = temp.DefaultView.ToTable();
                     }
                 }
                f.sourceDataTable = temp;
                f.WorkForm = this;
                f.srcControl = txtks;
                f.Font = txtks.Font;
                f.Width = 400;
                f.ReciveString = e.KeyChar.ToString();
                e.Handled = true;
                if (f.ShowDialog() == DialogResult.Cancel)
                {
                    txtks.Focus();
                    return;
                }
                else
                {
                    txtks.Tag = Convert.ToInt32(f.SelectDataRow["dept_id"]);
                    txtks.Text = f.SelectDataRow["name"].ToString().Trim();
                    butnew_Click(sender, e);

                    e.Handled = true;
                }
            }
            else
            {
                butnew_Click(sender, e);
            }
        }

        private void butreadcard_Click(object sender, EventArgs e)
        {
            ////读卡的话默认第一个医保类型 
            //if (Convert.ToInt32(cmbyblx.SelectedValue) <= 0)
            //{
            //    if (cmbyblx.Items.Count > 0) cmbyblx.SelectedIndex = 0;
            //}

            //if (cmbyblx.SelectedIndex == -1)
            //{
            //    MessageBox.Show("没有选择医保类型！");
            //    return;
            //}

            //try
            //{
            //    Cursor = PubStaticFun.WaitCursor();
            //    butreadcard.Enabled = false;
            //    int _yblx = Convert.ToInt32(cmbyblx.SelectedValue);
            //    Yblx yblx = new Yblx(_yblx, InstanceForm.BDatabase);

            //    if (yblx.issf == false) return;

            //    brxx = new ts_yb_mzgl.BRXX();
            //    jsxx = new ts_yb_mzgl.JSXX();

            //    brxx.BRXXID = Dqcf.brxxid;
            //    brxx.GHXXID = Dqcf.ghxxid;
            //    brxx.BLH = lblmzh.Text.Trim();
            //    brxx.SFZH = lblsfzh.Text;
            //    brxx.BRXM = txtxm.Text;

            //    ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
            //    bool bok = ybjk.GetPatientInfo("",yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid, InstanceForm.BCurrentUser.EmployeeId.ToString(), "", "", ref brxx, cmbtb);
            //    if ( brxx.KLX != 0 && brxx.KH!=null && brxx.KH!="")
            //    {
            //        cmbklx.SelectedValue = brxx.KLX;
            //        txtkh.Text = brxx.KH;
            //        GetBrxx("", brxx.KLX, brxx.KH, "", "");
            //    }
            //    brxx.BRXXID = Dqcf.brxxid;
            //    brxx.GHXXID = Dqcf.ghxxid;
            //    brxx.BLH = lblmzh.Text.Trim();
            //    //brxx.SFZH = lblsfzh.Text;


            //    lblbrxm_yb.Text = brxx.BRXM;
            //    jsxx.JSDH = brxx.JSSJH;
            //    lbljzh.Text = brxx.JSSJH;
            //    lblsfzh.Text = brxx.SFZH;
            //    lblybkh.Text = brxx.YLBZKKH;
            //    lblybkye.Text = brxx.KYE;
            //    lblgzdw.Text = brxx.GZDW;
            //    lblybrylx.Text = brxx.RYLB;
            //    butreadcard.Enabled = true;

            //}
            //catch (Exception err)
            //{
            //    MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    butreadcard.Enabled = true;
            //    Cursor = Cursors.Default;
            //}
            //finally
            //{
            //    butreadcard.Enabled = true;
            //    Cursor = Cursors.Default;
            //}



            //读卡的话默认第一个医保类型 
            if (Convert.ToInt32(cmbyblx.SelectedValue) <= 0)
            {
                if (cmbyblx.Items.Count > 0) cmbyblx.SelectedIndex = 0;
            }

            if (cmbyblx.SelectedIndex == -1)
            {
                MessageBox.Show("没有选择医保类型！");
                return;
            }

            try
            {
                Cursor = PubStaticFun.WaitCursor();
                butreadcard.Enabled = false;
                int _yblx = Convert.ToInt32(cmbyblx.SelectedValue);
                Yblx yblx = new Yblx(_yblx);

                brxx = new ts_yb_mzgl.BRXX();
                jsxx = new ts_yb_mzgl.JSXX();

                brxx.BRXXID = Dqcf.brxxid;
                brxx.GHXXID = Dqcf.ghxxid;
                brxx.BLH = lblmzh.Text.Trim();
                brxx.SFZH = lblsfzh.Text;
                brxx.BRXM = txtxm.Text;

                ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                bool bok = ybjk.GetPatientInfo("", yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid, InstanceForm.BCurrentUser.EmployeeId.ToString(), "", "", ref brxx, cmbtb);
                if (brxx.KLX != 0 && brxx.KH != null && brxx.KH != "")
                {
                    //modify by wangzhi 2011-01-18
                    //1、在读取IC卡完成后，如果挂号信息不为空，则说明先检索了病人挂号信息（此时界面上的门诊号，卡号，卡类型已赋值）,
                    //   不需要再重新对卡号和卡类型赋值和检索病人信息
                    //2、如果为空，则利用刚读出来的医保卡号去检索病人的挂号信息（需要一开始就是用医保卡挂号）
                    if (brxx.GHXXID == Guid.Empty)
                    {
                        cmbklx.SelectedValue = brxx.KLX;
                        txtkh.Text = brxx.KH;
                        GetBrxx("", brxx.KLX, brxx.KH, "", "");
                    }
                    //end modify

                    //comment by wangzhi 2011-01-18
                    //cmbklx.SelectedValue = brxx.KLX;
                    //txtkh.Text = brxx.KH;
                    //GetBrxx("", brxx.KLX, brxx.KH, "", "");
                    //end comment
                }

                brxx.BRXXID = Dqcf.brxxid;
                brxx.GHXXID = Dqcf.ghxxid;
                brxx.BLH = lblmzh.Text.Trim();
                //brxx.SFZH = lblsfzh.Text;

                lblbrxm_yb.Text = brxx.BRXM;
                jsxx.JSDH = brxx.JSSJH;
                lbljzh.Text = brxx.JSSJH;
                lblsfzh.Text = brxx.SFZH;
                lblybkh.Text = brxx.YLBZKKH;
                lblybkye.Text = brxx.KYE; //获取医保余额
                lblgzdw.Text = brxx.GZDW;
                lblybrylx.Text = brxx.RYLB;
                if (Dqcf.ghxxid == Guid.Empty)
                {
                    txtxm.Text = brxx.BRXM;
                }
                butreadcard.Enabled = true;


            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                butreadcard.Enabled = true;
                Cursor = Cursors.Default;
            }
            finally
            {
                butreadcard.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void butxtk_Click(object sender, EventArgs e)
        {
            try
            {
                ////默认第一个医保类型
                //if (Convert.ToInt32(cmbyblx.SelectedValue) <= 0)
                //{
                //    DataTable yblxTb = (DataTable)cmbyblx.DataSource;
                //    DataRow[] yblxDr = yblxTb.Select("ID>0");

                //    if (yblxDr.Length > 0)
                //    {
                //        cmbyblx.SelectedValue = Convert.ToInt32(Convertor.IsNull(yblxDr[0]["id"], "-1"));
                //    }
                //}
                //读卡的话默认第一个医保类型 
                if (Convert.ToInt32(cmbyblx.SelectedValue) <= 0)
                {
                    if (cmbyblx.Items.Count > 0) cmbyblx.SelectedIndex = 0;
                }

                if (cmbyblx.SelectedIndex == -1)
                {
                    MessageBox.Show("没有选择医保类型！");
                    return;
                }

                Cursor = PubStaticFun.WaitCursor();
                int _yblx = Convert.ToInt32(cmbyblx.SelectedValue);
                Yblx yblx = new Yblx(_yblx, InstanceForm.BDatabase);

                ts_yb_mzgl.IMZYB ybjk = ts_yb_mzgl.ClassFactory.InewInterface(yblx.ybjklx);
                bool bok = ybjk.Xtk(yblx.yblx.ToString(), yblx.insureCentral, yblx.hospid);

                butreadcard.Focus();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                butnew_Click(sender, e);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        //无号
        private void butwh_Click(object sender, EventArgs e)
        {
            ClearForm();
            txtmzh.Text = Fun.GetNewMzh(InstanceForm.BDatabase);
            lblmzh.Text = txtmzh.Text;
            txtxm.Text = "未写名";
            label5.Tag = "";//Add By Zj 2012-04-10
            txtxm.Focus();
            txtxm.SelectAll();

            if (_menuTag.Function_Name == "Fun_ts_mz_hj_Lg")
            {
                txtks.Tag = InstanceForm.BCurrentDept.DeptId.ToString();
                txtks.Text = InstanceForm.BCurrentDept.DeptName;
            }

            //添加优惠方案
            FunAddComboBox.AddYhfa(Convert.ToInt32(Convertor.IsNull(lblklx.Tag, "0")), new Guid(Convertor.IsNull(lblkh.Tag, Guid.Empty.ToString())), Convert.ToInt32(Convertor.IsNull(lblbrlx.Tag, "0")), Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0")), Convert.ToInt32(Convertor.IsNull(lblhtdwlx.Tag, "0")), _menuTag.Function_Name, cmbyhlx, InstanceForm.BDatabase);


            DataTable tb = (DataTable)this.dataGridView1.DataSource;
            tb.Rows.Clear();

            //报价器 姓名
            string bqybjq = ApiFunction.GetIniString("报价器文件路径", "启用报价器", Constant.ApplicationDirectory + "//ClientWindow.ini");
            if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
            {
                string bjqxh = ApiFunction.GetIniString("报价器文件路径", "报价器型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
                ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);

                call.Call(ts_call.DmType.清除, "");
            }
        }



        private void butref_Click(object sender, EventArgs e)
        {
            try
            {
                if (_menuTag.Function_Name.Trim() == "Fun_ts_mz_hj")
                    Tab = mz_sf.Select_Wsfcf(InstanceForm.BCurrentDept.DeptId, Guid.Empty, Dqcf.ghxxid, 0, 0, Guid.Empty, InstanceForm.BDatabase);
                else
                    Tab = mz_sf.Select_Wsfcf(0, Guid.Empty, Dqcf.ghxxid, 0, 0, Guid.Empty, InstanceForm.BDatabase);
                Fun.AddDataTableColumn(Tab, "分方状态", "");
                Fun.AddDataTableColumn(Tab, "自备药", "0");
                AddPresc(Tab);

                chkbfp.Checked = false;
                IsRowAdd = false;
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentCell == null) return;
            DataTable tb = (DataTable)this.dataGridView1.DataSource;
            int nrow = this.dataGridView1.CurrentCell.RowIndex;
            int ncol = this.dataGridView1.CurrentCell.ColumnIndex;
            if (dataGridView1.Columns[ncol].Name == "选择")
            {
                DataRow[] rows1 = tb.Select("hjid='" + tb.Rows[nrow]["hjid"] + "'");
                int b = Convert.ToInt16(Convertor.IsNull(tb.Rows[nrow]["选择"], "0"));
                if (b == 1)
                {
                    for (int i = 0; i <= rows1.Length - 1; i++)
                    {
                        if (rows1[i]["序号"].ToString().Trim() != "小计") rows1[i]["选择"] = false;
                    }
                }
                else
                    for (int i = 0; i <= rows1.Length - 1; i++)
                        if (rows1[i]["序号"].ToString().Trim() != "小计") rows1[i]["选择"] = true;
            }
        }



        private void txtxm_Leave(object sender, EventArgs e)
        {
            //#region  如果使用卡 且持卡人病人信息不等于当前姓名时，就新增病人信息，或找相应的病人信息
            //if (tbk != null)
            //{
            //    if (tbk.Rows.Count != 0)
            //    {
            //        if (Convertor.IsNull(tbk.Rows[0]["ckrxm"], "").Trim() != txtxm.Text.Trim())
            //        {
            //            Dqcf.brxxid = Guid.Empty;
            //            Dqcf.ghxxid = Guid.Empty;
            //            string ss = "select * from YY_BRXX where brxxid in(select b.brxxid from YY_KDJB  a inner join mz_ghxx b on a.kdjid=b.kdjid and a.brxxid=" + Convert.ToInt64(tbk.Rows[0]["brxxid"]) + ")";
            //            DataTable tbghxx = InstanceForm.BDatabase.GetDataTable(ss);
            //            this.Tag = "0";
            //            for (int i = 0; i <= tbghxx.Rows.Count - 1; i++)
            //            {
            //                if (tbghxx.Rows[i]["brxm"].ToString().Trim() == txtxm.Text.Trim())
            //                {
            //                    txtxm.Text = tbghxx.Rows[i]["brxm"].ToString();

            //                    lblgzdw.Text = tbghxx.Rows[i]["gzdw"].ToString();
            //                    if (lblgzdw.Text.Trim() == "") lblgzdw.Text = tbghxx.Rows[i]["jtdz"].ToString();

            //                    lbllxdh.Text = tbghxx.Rows[i]["gzdwdh"].ToString();
            //                    if (lbllxdh.Text.Trim() == "") lbllxdh.Text = tbghxx.Rows[i]["jtdh"].ToString();
            //                    if (lbllxdh.Text.Trim() == "") lbllxdh.Text = tbghxx.Rows[i]["grlxfs"].ToString();

            //                    lblbrlx.Tag =tbghxx.Rows[i]["brlx"].ToString();

            //                    Dqcf.brxxid = Convert.ToInt64(tbghxx.Rows[i]["brxxid"]);
            //                }
            //            }
            //        }
            //    }
            //}
            //#endregion

            //报价器 姓名
            string bqybjq = ApiFunction.GetIniString("报价器文件路径", "启用报价器", Constant.ApplicationDirectory + "//ClientWindow.ini");
            if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
            {
                string bjqxh = ApiFunction.GetIniString("报价器文件路径", "报价器型号", Constant.ApplicationDirectory + "//ClientWindow.ini");
                ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);
                call.Call(ts_call.DmType.姓名, txtxm.Text.Trim());
            }
        }

        private void buttzfph_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentCell == null) return;
                DataTable tb = (DataTable)dataGridView1.DataSource;
                int nrow = dataGridView1.CurrentCell.RowIndex;
                int ncol = dataGridView1.CurrentCell.ColumnIndex;

                if (e.KeyValue == 46)
                {
                    mnuDelrow_Click(sender, e);
                }

                if ((e.KeyValue >= 0 && e.KeyValue <= 9) || (e.KeyValue >= 48 && e.KeyValue <= 57) || (e.KeyValue >= 65 && e.KeyValue <= 90) ||
                    e.KeyValue == 46 || e.KeyValue == 8 || e.KeyValue == 190 || e.KeyValue == 13 || (e.KeyValue >= 96 && e.KeyValue <= 105) || e.KeyValue == 110 || e.KeyValue == 190)
                {
                }
                else
                {
                    return;
                }

                #region 编码
                if (dataGridView1.Columns[ncol].Name == "编码")
                {
                    //if ((int)e.KeyChar == 13 || (int)e.KeyChar == 8 || (int)e.KeyChar == 46 || (int)e.KeyChar == 32) return;
                    if (e.KeyValue >= 112 && e.KeyValue <= 123) return;

                    if (_menuTag.Function_Name != "Fun_ts_mz_sf")
                    {
                        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Space) return;
                    }
                    else
                    {
                        if (e.KeyCode == Keys.Enter && Config1056.Config == "1") { butsf_Click(sender, e); return; }
                        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Space) return;
                    }


                    if (tb.Rows[nrow]["序号"].ToString().Trim() == "小计") return;

                    //dataGridView1.Rows[nrow].Cells["编码"].Value = dataGridView1.Rows[nrow].Cells["编码"].Value + e.KeyChar.ToString();


                    //FrmCard f = new FrmCard(_menuTag, "", _mdiParent);
                    int ysdm = Convert.ToInt32(Convertor.IsNull(txtys.Tag, "0"));
                    int ksdm = Convert.ToInt32(Convertor.IsNull(txtks.Tag, "0"));
                    if (ysdm == 0 && _menuTag.Function_Name != "Fun_ts_mz_hj_Lg")
                    {
                        MessageBox.Show("请选择医生", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (ksdm == 0)
                    {
                        MessageBox.Show("请选择科室", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    //if (Dqcf.tcid != 0 && Dqcf.tcid != Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["套餐id"], "0")))
                    //{
                    //    MessageBox.Show("一张处方只能有一个套餐", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}



                    //对于新处方初始化结构
                    DataRow[] rows = tb.Select("hjid='" + Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()) + "' ");
                    //if (Convertor.IsNull(Dqcf.cfh, "0") == "0")
                    if (rows.Length == 0)
                    {

                        Dqcf.ysdm = Convert.ToInt32(Convertor.IsNull(txtys.Tag, "0"));
                        Dqcf.ksdm = Convert.ToInt32(Convertor.IsNull(txtks.Tag, "0"));
                        Dqcf.zyksid = 0;//隶属于住院科室收入

                        if (_menuTag.Function_Name.Trim() != "Fun_ts_mz_hj")
                        {
                            Dqcf.xmly = Xmly;//项目来源
                            Dqcf.zxksid = 0;//执行科室
                        }
                        else
                        {
                            Dqcf.xmly = 1;//项目来源
                            Dqcf.zxksid = 0;// InstanceForm.BCurrentDept.DeptId; //执行科室
                        }

                        Dqcf.tcid = 0;
                        Dqcf.fpcode = "";
                        Dqcf.tjdxmdm = "";
                        Dqcf.js = 1;

                    }

                    // FrmCard f = new FrmCard(_menuTag, _chineseName, _mdiParent);
                    f._all = 0;
                    f._xmly = Dqcf.xmly;
                    f._zyyf = rdozyyf.Checked == true ? 1 : 0; //是否查询住院药房
                    f._execdept = Dqcf.zxksid;
                    f._deptid = InstanceForm.BCurrentDept.DeptId;
                    f.txtinput.Text = Convert.ToString((char)e.KeyCode);
                    f.txtinput.Select(1, 0);
                    f.ShowDialog();

                    if (f.ReturnRow == null) return;
                    #region /*如果选择的项目为检验或检查项目则弹出相应的申请界面选择框 Add By zp 2013-10-09*/
                    if (Convertor.IsNull(f.ReturnRow["项目来源"], "1") == "2" && cfg1095.Config.Trim()=="1")
                    {
                        unyzid = Convertor.IsNull(tb.Rows[nrow]["yzid"], "0");
                        unhjxmid = Convertor.IsNull(tb.Rows[nrow]["hjmxid"], "");
                        unyznr = Convertor.IsNull(tb.Rows[nrow]["医嘱内容"], "").Trim();
                        unbbmc = Convertor.IsNull(tb.Rows[nrow]["规格"], "");
                        unxmid = Convertor.IsNull(tb.Rows[nrow]["项目id"], "0");
                        untcid = Convertor.IsNull(tb.Rows[nrow]["套餐id"], "0");


                        string tcid = Convertor.IsNull(f.ReturnRow["套餐"], "-1");
                        string xmid = f.ReturnRow["项目id"].ToString().Trim();
                        bool is_jc = false; //当前项目是否为检查项目
                        string _OrderId="";
                        //通过套餐id和项目id获取 医嘱信息
                        DataTable dt_order = mz_sf.GetOrderYjItemInfo(xmid, tcid,"", InstanceForm.BDatabase);
                        if (dt_order.Rows.Count <= 0) //如果不为检验项目 则为检查项目
                        {
                            is_jc = true;
                            dt_order = mz_sf.GetOrderJcItemInfo(xmid, tcid,"", InstanceForm.BDatabase);
                        }
                        if (dt_order.Rows.Count > 0)
                        {
                           
                            _OrderId = dt_order.Rows[0]["HOITEM_ID"].ToString();
                            DataRow[] rowyj = PubDset.Tables["ITEM_YJ"].Select("order_id=" + _OrderId + "");
                            if (rowyj.Length > 0) //需要向医技申请表插入记录 则show出医技申请框
                            {
                                Guid _yjsqid = Guid.Empty;
                                Guid _yjhjid = new Guid(Convertor.IsNull(tb.Rows[nrow]["hjid"], Guid.Empty.ToString()));
                                Guid _yjhjmxid = Guid.Empty;//Convertor.IsNull(this.dataGridView1.CurrentRow.Cells["hjmxid"].Value, "") == "" ? Guid.Empty : new Guid(this.dataGridView1.CurrentRow.Cells["hjmxid"].Value.ToString());
                                if (Convertor.IsNull(tb.Rows[nrow]["hjmxid"], "") != "" && Convertor.IsNull(tb.Rows[nrow]["hjmxid"], "").Trim() != "99999999")
                                    _yjhjmxid = new Guid(this.dataGridView1.CurrentRow.Cells["hjmxid"].Value.ToString());
                                string bbmc = Convertor.IsNull(this.dataGridView1.CurrentRow.Cells["规格"].Value, "");
                                if (_yjhjid != Guid.Empty && (!IsRowAdd)) //获取到医技申请id 将医技申请
                                {
                                    string sql = @"SELECT * FROM YJ_MZSQ WHERE YZID='" + _yjhjid + "' ";
                                    if (_yjhjmxid != Guid.Empty)
                                        sql += " and HJMXID='" + _yjhjmxid + "'";
                                    if (!string.IsNullOrEmpty(bbmc))
                                        sql += " AND BBMC='" + bbmc + "'";
                                    if (!string.IsNullOrEmpty(unyzid)) 
                                        sql += " AND YZXMID='" + unyzid + "'";
                                    DataTable dt = InstanceForm.BDatabase.GetDataTable(sql);
                                    if(dt.Rows.Count>0)
                                        _yjsqid = new Guid(dt.Rows[0]["YJSQID"].ToString());
                                }

                                DataTable dt_jzinfo = mz_sf.GetJzInfo(Dqcf.ghxxid, InstanceForm.BDatabase);
                                object[] comValue = new object[13];

                                if (Dqcf.brxxid == null || Dqcf.brxxid==Guid.Empty)
                                     SaveBrxx();
                                comValue[0] = Dqcf.brxxid;
                                if (Dqcf.ghxxid==null || Dqcf.ghxxid == Guid.Empty) //如果当前未产生挂号信息,则先构造一个虚拟的挂号信息id,保存后再修改为实际的挂号信息id Modify By zp 2013-10-11
                                {
                                   string _sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
                                   ReadCard readcard = new ReadCard(Convert.ToInt32(Convertor.IsNull(lblklx.Tag, "0")), txtkh.Text.Trim(), InstanceForm.BDatabase);
                                   UnYjJzid = Guid.Empty;
                                   InstanceForm.BDatabase.BeginTransaction();
                                       try
                                       {
                                           //Modify by zp 2013-12-09 保存挂号信息
                                           SaveGhInfo(ref UnYjJzid, _sDate, readcard, "", 0, "");
                                           InstanceForm.BDatabase.CommitTransaction();
                                       }
                                       catch
                                       {
                                           InstanceForm.BDatabase.RollbackTransaction();
                                       }
                                   
                                   comValue[1] = Dqcf.ghxxid;
                                }
                                else
                                    comValue[1] = Dqcf.ghxxid;
                                if (UnYjJzid == Guid.Empty && dt_jzinfo.Rows.Count <= 0)
                                {
                                    int _err_code = 0;
                                    string _err_text = "";
                                    mzys_jzjl.jz(Jgbm, Dqcf.ghxxid, Dqcf.ysdm, Dqcf.ksdm, DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString("yyyy-MM-dd HH:mm:ss"), "非医生工作站产生", out UnYjJzid, out _err_code, out _err_text, 1, InstanceForm.BDatabase);
                                    if (_err_code == -1 || UnYjJzid == Guid.Empty)
                                        throw new Exception(_err_text);
                                }
                                comValue[2] = dt_jzinfo.Rows.Count <= 0 ? UnYjJzid : new Guid(dt_jzinfo.Rows[dt_jzinfo.Rows.Count - 1]["JZID"].ToString());
                                comValue[3] = txtxm.Text;
                                comValue[4] = dt_jzinfo.Rows.Count <= 0 ? "" : dt_jzinfo.Rows[dt_jzinfo.Rows.Count - 1]["XB"].ToString();
                                if (dt_jzinfo.Rows.Count <= 0)
                                    comValue[5] = "";
                                else if (dt_jzinfo.Rows[dt_jzinfo.Rows.Count - 1]["AGE"].ToString().Trim() == "0岁")
                                    comValue[5] = "";
                                else
                                    comValue[5] = dt_jzinfo.Rows[dt_jzinfo.Rows.Count - 1]["AGE"].ToString();
                                comValue[7] = lblgzdw.Text;//工作单位
                                comValue[6] = "";//体征
                                comValue[8] = dt_jzinfo.Rows.Count <= 0 ? "" : dt_jzinfo.Rows[dt_jzinfo.Rows.Count - 1]["BRLXFS"].ToString();//个人联系方式
                                comValue[9] = lblmzh.Text;
                                comValue[10] = _yjsqid; //医技申请id
                                if (unyzid != "0" || IsRowAdd)
                                    comValue[11] = _yjhjid;
                                else
                                    comValue[11] = Guid.Empty;
                                comValue[12] = "";
                                if (!is_jc)
                                {
                                    Frmhysqd frm = (Frmhysqd)ShowDllForm("ts_mzys_yjsqd", "Fun_ts_mzys_yjsqd_hysq", "医技化验申请单", ref comValue, false);
                                    frm.dt_mzsf_order = dt_order;
                                    frm.Issfy = true;
                                    if (_yjsqid != Guid.Empty)
                                        frm.IsXg = true;
                                    frm.ShowDialog();
                                }
                                else //Ts_zyys_jcsq.FrmJCSQ 
                                {
                                   Ts_zyys_jcsq.FrmJCSQ frm = (Ts_zyys_jcsq.FrmJCSQ)ShowDllForm("ts_mzys_yjsqd", "Fun_ts_mzys_yjsqd_jcsq", "医技检查申请单", ref comValue, false);
                                   frm.Xg = true;
                                   frm.issfy = true;
                                   frm.tbxg = dt_order;
                                   frm.ShowDialog();
                                }
                                /*如果UnYjJzid不为空则表示需要查询下当前挂号信息id是否产生了处方信息 如果未产生则作废挂号记录,把UnYjGhid设置为空*/
                                if (UnYjJzid != Guid.Empty)
                                {
                                    if (!mz_hj.GetHj(Dqcf.ghxxid, InstanceForm.BDatabase))//未产生处方记录
                                    {
                                        string _sDate = DateManager.ServerDateTimeByDBType(InstanceForm.BDatabase).ToString();
                                        int row=0;
                                        int err_code=0;
                                        string err_text="";
                                        mz_ghxx.CancelGh(Dqcf.ghxxid, _sDate, InstanceForm.BCurrentUser.EmployeeId, out row, out err_code, out err_text, InstanceForm.BDatabase);  //CancelGh
                                        mzys_jzjl.Ujz(UnYjJzid, InstanceForm.BDatabase);
                                        UnYjJzid = Guid.Empty;
                                        Dqcf.ghxxid = Guid.Empty;
                                        this.txtmzh.Text= Fun.GetNewMzh(InstanceForm.BDatabase);//获取新门诊号
                                        this.lblmzh.Text = this.txtmzh.Text;
                                    }
                                }
                                butref_Click(null, null);
                                //Dqcf.ghxxid = Guid.Empty;
                                return;
                            }
                        }
                    }
                    #endregion
                    //dataGridView1.CurrentCell = dataGridView1.Rows[nrow].Cells["编码"];
                    Addrow(f.ReturnRow);

                    //如果单价为零则可以输入单价
                    if (Convert.ToDecimal(f.ReturnRow["单价"]) == 0)
                    {
                        tb.Rows[nrow]["单价"] = "";
                        dataGridView1.CurrentCell = dataGridView1["单价", nrow];
                        return;
                    } 

                    dataGridView1.CurrentCell = dataGridView1["数量", nrow];

                    ModifCfje(tb, tb.Rows[nrow]["hjid"].ToString());
                    ComputerJE(tb, tb.Rows[nrow]["hjid"].ToString());//Modify By Tany 2009-01-05
                    return;
                }
                #endregion

                string KeyValue = "";
                if (e.KeyValue >= 96 && e.KeyValue <= 105)
                {
                    KeyValue = Convert.ToString(e.KeyValue - 96);
                }
                else if (e.KeyValue == 110 || e.KeyValue == 190)
                    KeyValue = ".";
                else
                    KeyValue = Convert.ToString((char)e.KeyCode);

                #region  单价
                if (dataGridView1.Columns[ncol].Name == "单价")
                {
                    if (tb.Rows[nrow]["项目id"].ToString().Trim() == "") return;
                    if (Convert.ToBoolean(tb.Rows[nrow]["单价可改"]) == false) return;

                    if (Convertor.IsNumeric(KeyValue) == true || KeyValue == ".")
                    {
                        //tb.Rows[nrow]["单价"] = tb.Rows[nrow]["单价"].ToString() + e.KeyChar.ToString();
                        sNum = sNum + KeyValue;
                        tb.Rows[nrow]["单价"] = sNum;
                    }
                    if (KeyValue == "\b" && tb.Rows[nrow]["单价"].ToString().Length > 0)
                    {
                        //tb.Rows[nrow]["单价"] = tb.Rows[nrow]["单价"].ToString().Substring(0, tb.Rows[nrow]["单价"].ToString().Length - 1);
                        sNum = tb.Rows[nrow]["单价"].ToString();
                        sNum = sNum.ToString().Substring(0, sNum.ToString().Length - 1);
                        tb.Rows[nrow]["单价"] = sNum;
                    }
                    decimal _sl = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["数量"], "0"));
                    decimal _dj = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["单价"], "0"));
                    int _js = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["剂数"], "0"));
                    decimal _je = _dj * _js * _sl;
                    tb.Rows[nrow]["金额"] = _je.ToString("0.000");

                    tb.Rows[nrow]["修改"] = true;
                    ModifCfje(tb, tb.Rows[nrow]["hjid"].ToString());
                    ComputerJE(tb, tb.Rows[nrow]["hjid"].ToString());//Modify By Tany 2009-01-05
                    if (KeyValue == "\r" && _dj > 0)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[nrow].Cells["数量"];
                    }
                }
                #endregion


                string tjdxm = tb.Rows[nrow]["统计大项目"].ToString().Trim();
                int byscf = Convert.ToInt16(Convertor.IsNull(tb.Rows[nrow]["byscf"], "0"));

                #region 数量
                if (dataGridView1.Columns[ncol].Name == "数量")
                {
                    //如果1095参数为1 针对医技项目不能更新数量 
                    //if (Convertor.IsNull(f.ReturnRow["项目来源"], "1") == "2" && e.KeyValue!=13 && cfg1095.Config.Trim() == "1")
                    //    return;
                    if (byscf == 1) return;
                    string ss = KeyValue;

                    if (Convertor.IsNumeric(KeyValue) == true || KeyValue == ".")
                    {
                        if (tb.Rows[nrow]["项目id"].ToString().Trim() == "") return;
                        sNum = sNum + KeyValue;
                        tb.Rows[nrow]["数量"] = sNum;
                    }
                    if (KeyValue == "\b" && tb.Rows[nrow]["数量"].ToString().Length > 0)
                    {
                        sNum = tb.Rows[nrow]["数量"].ToString();
                        sNum = sNum.ToString().Substring(0, sNum.ToString().Length - 1);
                        tb.Rows[nrow]["数量"] = sNum;
                    }

                    decimal _pfj = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["批发价"], "0"));
                    decimal _dj = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["单价"], "0"));
                    int _js = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["剂数"], "0"));
                    decimal _sl = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["数量"], "0"));
                    if (sNum.Trim() != "" || KeyValue == "\b")
                    {
                        decimal _je = _dj * _js * _sl;
                        tb.Rows[nrow]["金额"] = _je.ToString("0.0000");
                        tb.Rows[nrow]["修改"] = true;
                        ModifCfje(tb, tb.Rows[nrow]["hjid"].ToString());
                        ComputerJE(tb, tb.Rows[nrow]["hjid"].ToString());//Modify By Tany 2009-01-05

                        decimal _pfje = _pfj * _js * _sl;
                        tb.Rows[nrow]["批发金额"] = _pfje.ToString("0.000");

                        
                        // unyzid = tb.Rows[nrow][""] //"";
                        //private string unhjxmid = ""; //Add by zp 2013-10-13
                        //private string unyznr = "";
                        //private string unbbmc = "";
                        //private string unxmid = "";
                        //private string untcid = "";
                        if (Convertor.IsNull(tb.Rows[nrow]["项目来源"], "1") == "2" && cfg1095.Config.Trim() == "1")
                        {
                            unyzid = Convertor.IsNull(tb.Rows[nrow]["yzid"], "0");
                            unhjxmid = Convertor.IsNull(tb.Rows[nrow]["hjmxid"], "");
                            unyznr = Convertor.IsNull(tb.Rows[nrow]["医嘱内容"], "").Trim();
                            unbbmc = Convertor.IsNull(tb.Rows[nrow]["规格"], "");
                            unxmid = Convertor.IsNull(tb.Rows[nrow]["项目id"], "0");
                            untcid = Convertor.IsNull(tb.Rows[nrow]["套餐id"], "0");

                            DataTable dt = mz_sf.GetOrderJcItemInfo(unxmid, untcid, "", InstanceForm.BDatabase);
                            if (dt.Rows.Count > 0)
                                unyzid = dt.Rows[0]["医嘱项目id"].ToString();
                            else
                                dt = mz_sf.GetOrderYjItemInfo(unxmid, untcid, "", InstanceForm.BDatabase);
                            if(dt.Rows.Count>0)
                                unyzid = dt.Rows[0]["医嘱项目id"].ToString();
                        }
                    }

                    if (_sl > 0)
                    {
                        bool Bdelete = false;
                        string dw = tb.Rows[nrow]["单位"].ToString();
                        decimal kcl = mz_hj.ReturnKcl(Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0")), Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["执行科室ID"], "0")), out Bdelete, InstanceForm.BDatabase);
                        if (Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0")) == 1 && (_sl * _js) > kcl)
                        {
                            MessageBox.Show("当前药品库存量只有" + Convert.ToDouble(kcl).ToString() + dw.Trim() + "，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            tb.Rows[nrow]["数量"] = "";
                            tb.Rows[nrow]["金额"] = "0";
                            sNum = "";
                            ModifCfje(tb, tb.Rows[nrow]["hjid"].ToString());
                            ComputerJE(tb, tb.Rows[nrow]["hjid"].ToString());//Modify By Tany 2009-01-05
                            return;
                        }
                    }

                    if (KeyValue == "\r" && _sl > 0)
                    {
                        //

                        if (tjdxm == "03")
                        {
                            if (Dqcf.js == 1)
                            {
                                dataGridView1.CurrentCell = dataGridView1["剂数", nrow];
                                return;
                            }
                            else
                            {
                                if (nrow == tb.Rows.Count - 1)
                                {
                                    DataRow row = tb.NewRow();
                                    tb.Rows.Add(row);
                                    dataGridView1.DataSource = tb;
                                    dataGridView1.CurrentCell = dataGridView1.Rows[nrow + 1].Cells["编码"];
                                    return;
                                }
                                if (nrow < tb.Rows.Count - 1)
                                {
                                    dataGridView1.CurrentCell = dataGridView1.Rows[nrow + 1].Cells["编码"];
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (nrow == tb.Rows.Count - 1)
                            {
                                DataRow row = tb.NewRow();
                                tb.Rows.Add(row);
                                dataGridView1.DataSource = tb;
                                dataGridView1.CurrentCell = dataGridView1.Rows[nrow + 1].Cells["编码"];
                                return;
                            }
                            if (nrow < tb.Rows.Count - 1)
                            {
                                dataGridView1.CurrentCell = dataGridView1.Rows[nrow + 1].Cells["编码"];
                                return;
                            }
                        }
                    }
                    if (nrow == tb.Rows.Count - 1 && KeyValue == "\r") dataGridView1.CurrentCell = dataGridView1.Rows[tb.Rows.Count - 1].Cells["编码"];
                    return;
                }
                #endregion



                #region  剂数
                if (dataGridView1.Columns[ncol].Name == "剂数")
                {
                    if (byscf == 1) return;
                    //if (tb.Rows[nrow]["项目id"].ToString().Trim() == "") return;

                    if (Convertor.IsNumeric(KeyValue) == true || KeyValue == ".")
                    {
                        if (tb.Rows[nrow]["项目id"].ToString().Trim() == "") return;
                        if (tjdxm != "03") return;
                        //tb.Rows[nrow]["剂数"] = tb.Rows[nrow]["剂数"].ToString() + e.KeyChar.ToString();
                        sNum = sNum + KeyValue;
                        tb.Rows[nrow]["剂数"] = sNum;
                    }
                    if (KeyValue == "\b" && tb.Rows[nrow]["剂数"].ToString().Length > 0)
                    {
                        if (tb.Rows[nrow]["项目id"].ToString().Trim() == "") return;
                        if (tjdxm != "03") return;
                        //tb.Rows[nrow]["剂数"] = tb.Rows[nrow]["剂数"].ToString().Substring(0, tb.Rows[nrow]["剂数"].ToString().Length - 1);
                        sNum = tb.Rows[nrow]["剂数"].ToString();
                        sNum = sNum.ToString().Substring(0, sNum.ToString().Length - 1);
                        tb.Rows[nrow]["剂数"] = sNum;
                    }
                    int _js = Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["剂数"], "0"));

                    //修改小计金额
                    if (KeyValue != "\r")
                    {
                        DataRow[] rows = tb.Select("hjid='" + Convertor.IsNull(tb.Rows[nrow]["hjid"], "0") + "' ");
                        for (int i = 0; i <= rows.Length - 1; i++)
                        {
                            if (rows[i]["序号"].ToString().Trim() != "小计")
                            {
                                decimal _sl = Convert.ToDecimal(Convertor.IsNull(rows[i]["数量"], "0"));
                                decimal _dj = Convert.ToDecimal(Convertor.IsNull(rows[i]["单价"], "0"));
                                decimal _je = _dj * _js * _sl;
                                rows[i]["金额"] = _je.ToString("0.000");
                                rows[i]["剂数"] = _js.ToString();

                                decimal _pfj = Convert.ToDecimal(Convertor.IsNull(rows[i]["批发价"], "0"));
                                decimal _pfje = _pfj * _js * _sl;
                                rows[i]["批发金额"] = _pfje.ToString("0.000");
                            }
                        }

                        ModifCfje(tb, tb.Rows[nrow]["hjid"].ToString());
                        ComputerJE(tb, tb.Rows[nrow]["hjid"].ToString());//Modify By Tany 2009-01-05
                    }


                    tb.Rows[nrow]["修改"] = true;
                    if (KeyValue == "\r" && _js > 0)
                    {
                        bool Bdelete = false;
                        decimal _sl = Convert.ToDecimal(Convertor.IsNull(tb.Rows[nrow]["数量"], "0"));
                        string dw = tb.Rows[nrow]["单位"].ToString();
                        decimal kcl = mz_hj.ReturnKcl(Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目id"], "0")), Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["执行科室ID"], "0")), out Bdelete, InstanceForm.BDatabase);
                        if (Convert.ToInt32(Convertor.IsNull(tb.Rows[nrow]["项目来源"], "0")) == 1 && (_sl * _js) > kcl)
                        {
                            MessageBox.Show(this, "当前药品库存量只有" + Convert.ToDouble(kcl).ToString() + dw.Trim() + "，请修改剂数", "确认", MessageBoxButtons.OK);
                            return;
                        }

                        if (nrow == tb.Rows.Count - 1)
                        {
                            DataRow row = tb.NewRow();
                            tb.Rows.Add(row);
                            dataGridView1.DataSource = tb;
                            dataGridView1.CurrentCell = dataGridView1.Rows[nrow + 1].Cells["编码"];
                        }
                        if (nrow < tb.Rows.Count - 1)
                        {
                            dataGridView1.CurrentCell = dataGridView1.Rows[nrow + 1].Cells["编码"];
                        }
                    }
                    if (nrow == tb.Rows.Count - 1 && KeyValue == "\r") dataGridView1.CurrentCell = dataGridView1.Rows[tb.Rows.Count - 1].Cells["编码"];

                    Dqcf.js = _js;

                }
                #endregion

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        //Add By zp 2013-10-09
        private Form ShowDllForm(string dllName, string functionName, string chineseName, ref object[] communicateValue, bool showModule)
        {
            try
            {
                long menuId;
                menuId = _menuTag.ModuleId;
                //获得DLL中窗体
                Form dllForm = null;
                if (showModule)
                    dllForm = (Form)WorkStaticFun.InstanceForm(dllName, functionName, chineseName, InstanceForm.BCurrentUser, InstanceForm.BCurrentDept,
                        _menuTag, menuId, this.MdiParent, InstanceForm.BDatabase, ref communicateValue);
                else
                    dllForm = (Form)WorkStaticFun.InstanceForm(dllName, functionName, chineseName, InstanceForm.BCurrentUser, InstanceForm.BCurrentDept,
                        _menuTag, menuId, null, InstanceForm.BDatabase, ref communicateValue);
                return dllForm;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        private void butybyc_Click(object sender, EventArgs e)
        {
            //#region 医保退费

            //if (MessageBox.Show(this, "说明:\n 1、该操作专门用于处理医保收费成功,但医院系统收费没有成功的情况.\n  2、此操作只负责对医保系统的退费操作\n\n您确定要对就医号为 [" + lbljzh.Text + "] 进行异常处理吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            //int _yblx = Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0"));
            //ts_yb_mzgl.IMZYB mzyb;
            //Yblx yblx = new Yblx(_yblx);

            //string ss = "";
            //string ssql = "";
            //DataTable tbcx = null;
            //bool ret = false;
            //int x = 0;

            //switch (yblx.ybjklx)
            //{
            //    case 1:
            //        if (yblx.issf == false) break;
            //        ss = "select  ybjzh,fpH,ybjsid,fpid from MZ_YBJSB_CXYB where  ybjzh = '" + lbljzh.Text.Trim() + "' and JGBM=" + TrasenFrame.Forms.FrmMdiMain.Jgbm + " and bscbz=0 ";
            //        tbcx = InstanceForm.BDatabase.GetDataTable(ss);
            //        if (tbcx.Rows.Count == 0)
            //        {
            //            MessageBox.Show("在医保结算表中未找到相关结算信息");
            //            return;
            //        }
            //        if (tbcx.Rows.Count > 1)
            //        {
            //            MessageBox.Show("在医保结算表中找到多条就医登记号为" + lbljzh.Text.Trim() + "的记录,请和管理员联系");
            //            return;
            //        }
            //        if (tbcx.Rows[0]["fpid"].ToString().Trim() != "0")
            //        {
            //            MessageBox.Show("就医登记号为 [" + lbljzh.Text.Trim() + "] 的医保结算，与医院系统中的发票号 [" + Convertor.IsNull(tbcx.Rows[0]["fph"], "") + "] 对应，您不能取消这匹配结算");
            //            return;
            //        }

            //        mzyb = new ts_yb_mzgl.CX_Mzyb();
            //        string err_text = "";
            //        ((ts_yb_mzgl.CX_Mzyb)mzyb).Insur_No = tbcx.Rows[0][0].ToString().Trim();

            //        ret = ((ts_yb_mzgl.CX_Mzyb)mzyb).UnCompute(out err_text);
            //        if (!ret)
            //        {
            //            MessageBox.Show(err_text, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return;
            //        }

            //        ssql = "update MZ_YBJSB_CXYB set bscbz=1 where ybjzh='" + lbljzh.Text.Trim() + "' and bscbz=0 and ybjsid=" + Convert.ToInt64(tbcx.Rows[0]["ybjsid"]) + "";
            //        x = InstanceForm.BDatabase.DoCommand(ssql);
            //        if (x != 1)
            //        {
            //            MessageBox.Show("医保退费成功，但产生本地医保结算红冲记录时出错,就医登记号为" + lbljzh.Text.Trim() + ",请和管理员联系");
            //            return;
            //        }
            //        MessageBox.Show("异常处理成功");
            //        break;
            //    case 2:
            //        if (yblx.issf == false) break;

            //        DlgInputBox Inputbox = new DlgInputBox("", "请输入您要取消医保收费的医保结算号！（请与系统管理员联系获得该号码）", "取消医保收费");
            //        Inputbox.NumCtrl = false;
            //        Inputbox.ShowDialog();
            //        if (!DlgInputBox.DlgResult) return;

            //        string ybjssjh = DlgInputBox.DlgValue;

            //        ss = "select ybjzh,fpH,ybjsid,fpid from MZ_YBJSB_SDYB where  ybjsh = '" + ybjssjh + "' and JGBM=" + TrasenFrame.Forms.FrmMdiMain.Jgbm + " and bscbz=0 ";
            //        tbcx = InstanceForm.BDatabase.GetDataTable(ss);
            //        if (tbcx.Rows.Count == 0)
            //        {
            //            MessageBox.Show("在医保结算表中未找到相关结算信息");
            //            return;
            //        }
            //        if (tbcx.Rows.Count > 1)
            //        {
            //            MessageBox.Show("在医保结算表中找到多条就医登记号为" + ybjssjh + "的记录,请和管理员联系");
            //            return;
            //        }
            //        if (tbcx.Rows[0]["fpid"].ToString().Trim() != "0")
            //        {
            //            MessageBox.Show("就医登记号为 [" + ybjssjh + "] 的医保结算，与医院系统中的发票号 [" + Convertor.IsNull(tbcx.Rows[0]["fph"], "") + "] 对应，您不能取消这匹配结算");
            //            return;
            //        }

            //        mzyb = new ts_yb_mzgl.SD_Mzyb();
            //        ((ts_yb_mzgl.SD_Mzyb)mzyb).BillNo = ybjssjh;

            //        ret = ((ts_yb_mzgl.SD_Mzyb)mzyb).CancelCharge();
            //        if (!ret)
            //        {
            //            MessageBox.Show("医保退费出错！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return;
            //        }

            //        ssql = "update MZ_YBJSB_SDYB set bscbz=1 where ybjsh='" + ybjssjh + "' and bscbz=0 and ybjsid=" + Convert.ToInt64(tbcx.Rows[0]["ybjsid"]) + "";
            //        x = InstanceForm.BDatabase.DoCommand(ssql);
            //        if (x != 1)
            //        {
            //            MessageBox.Show("医保退费成功，但产生本地医保结算红冲记录时出错,医保结算号为" + ybjssjh + ",请和管理员联系");
            //            return;
            //        }
            //        MessageBox.Show("异常处理成功");
            //        break;
            //    default:
            //        break;

            //}

            //if (yblx.ybjklx == 0 && lbljzh.Text.Trim() != "")
            //{
            //    MessageBox.Show("没有获取到医保类型,但就医号不为空,就医登记号为" + lbljzh.Text.Trim() + ",请和管理员联系");
            //    return;
            //}
            //#endregion
        }

        private void Language_Off(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;

            control.ImeMode = ImeMode.Close;
            Fun.SetInputLanguageOff();
        }

        private void Language_On(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            control.ImeMode = ImeMode.On;
            Fun.SetInputLanguageOn();
        }

        private string GetStringbyId(string IdFieldsName, string NameFieldName, string tableName, string Searchvalue)
        {
            try
            {
                string sql = "select " + NameFieldName + " from " + tableName + " where " + IdFieldsName + " = " + Searchvalue;
                return InstanceForm.BDatabase.GetDataResult(sql).ToString();
            }
            catch
            {
                return "";
            }
        }

        private void Frmhjsf_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //Modify by Tany 2009-01-04 关闭的时候释放医保资源
                DataTable yblxTb = (DataTable)cmbyblx.DataSource;
                DataRow[] yblxDr = yblxTb.Select("ID>0");
                if (TrasenFrame.Forms.FrmMdiMain.Database == null) return;
                for (int i = 0; i < yblxDr.Length; i++)
                {
                    Yblx yblx = new Yblx(Convert.ToInt64(Convertor.IsNull(yblxDr[i]["id"], "-1")), InstanceForm.BDatabase);

                    switch (yblx.ybjklx)
                    {
                        case 1://长信
                            break;
                        case 2://桑达
                            string msg = "";
                            ushort iAuth = ts_yb_interface.SED_Interface.Sed_UnAuthHis(ref msg);
                            if (iAuth == 0)
                                MessageBox.Show("取消认证失败！" + msg);
                            break;
                        default:
                            break;
                    }
                }

                //((FrmMdiMain)_mdiParent).sttbpDescription.Text = "";
                this.ToolState_Txt.Text = "";
            }
            catch (System.Exception err)
            {
            }
        }

        private void btnfpcx_Click(object sender, EventArgs e)
        {
            ts_mz_cx.Frmfpcx frmfpcx = new ts_mz_cx.Frmfpcx(_menuTag, "门诊发票查询", _mdiParent);
            ts_mz_cx.InstanceForm.BDatabase = InstanceForm.BDatabase;
            ts_mz_cx.InstanceForm.BCurrentDept = InstanceForm.BCurrentDept;
            ts_mz_cx.InstanceForm.BCurrentUser = InstanceForm.BCurrentUser;
            frmfpcx._mzh = txtmzh.Text.Trim();
            frmfpcx._kh = txtkh.Text.Trim();
            frmfpcx._xm = txtxm.Text.Trim();
            frmfpcx.Show();
        }

        private void ComputerJE(DataTable tb, string hjid)
        {
            decimal sumje = 0;

            if (hjid == "")
                hjid = Convertor.IsNull(hjid, "0");

            sumje = Convert.ToDecimal(Convertor.IsNull(tb.Compute("sum(金额)", "序号<>'小计'  and hjid='" + hjid + "' "), "0"));

            //((FrmMdiMain)_mdiParent).sttbpDescription.Text = sumje == 0 ? "" : "当前处方金额:" + sumje.ToString();
            this.ToolState_Txt.Text = sumje == 0 ? "" : "当前处方金额:" + sumje.ToString();
        }

        private void Frmhjsf_Leave(object sender, EventArgs e)
        {
            //((FrmMdiMain)_mdiParent).sttbpDescription.Text = "";
            this.ToolState_Txt.Text = "";
        }

        private void Getypcfs(Guid kdjid)
        {
            string bdate = DateTime.Now.ToShortDateString() + " 00:00:00";
            string edate = DateTime.Now.ToShortDateString() + " 23:59:59";
            string sql = "select count(1) from YY_KDJB a inner join mz_fpb b on a.kdjid=b.kdjid inner join mz_cfb c on b.fpid=c.fpid where b.jlzt=0 and c.xmly=1 and a.kdjid='" + kdjid + "' and b.sfrq between '" + bdate + "' and '" + edate + "'";
            DataTable tb = FrmMdiMain.Database.GetDataTable(sql);

            if (tb.Rows.Count > 0)
            {
                lblypcfs.Text = Convert.ToString(tb.Rows[0][0]);
            }
            else
            {
                lblypcfs.Text = "0";
            }

            lblypcfs.Visible = true;
            label24.Visible = true;
        }

        private void cmbyblx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbyblx.SelectedIndex == -1) return;
                //添加优惠方案
                FunAddComboBox.AddYhfa(Convert.ToInt32(Convertor.IsNull(lblklx.Tag, "0")), new Guid(Convertor.IsNull(lblkh.Tag, Guid.Empty.ToString())), Convert.ToInt32(Convertor.IsNull(lblbrlx.Tag, "0")), Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0")), Convert.ToInt32(Convertor.IsNull(lblhtdwlx.Tag, "0")), _menuTag.Function_Name,cmbyhlx, InstanceForm.BDatabase);
            }
            catch (System.Exception err)
            {
            }
        }

        /// <summary>
        /// 验证医保是否匹配 Modify by zp 2013-11-12
        /// </summary>
        /// <param name="yblx"></param>
        /// <param name="hiscode"></param>
        /// <param name="xmid"></param>
        /// <param name="xmly"></param>
        private void YBPP(Yblx yblx, string hiscode, long xmid, int xmly)
        {
            try
            {
                
                if (yblx.yblx > 0 && yblx.issf == true && Convertor.IsNull(hiscode, "") != "")
                {
                    if (xmly == 1)
                        xmid = Convert.ToInt64(hiscode);
                    string ssql = "select * from JC_YB_BL where yblx=" + yblx.yblx + " and xmid=" + xmid + " and xmly=" + xmly + "";
                    DataTable tb = InstanceForm.BDatabase.GetDataTable(ssql);
                    if (tb.Rows.Count == 0 )
                    {
                        //if (xmly==1) hiscode="YP"+xmid.ToString();
                        //if (xmly == 2) hiscode = "XM" + xmid.ToString();
                        if (cfg1097.Config.Trim().Length > 0)
                        {
                            string[] par = cfg1097.Config.Split(',');
                            for (int y = 0; y < par.Length; y++)
                            {
                                if (par[y].Trim() == yblx.ybjklx.ToString())
                                {
                                    throw new Exception(hiscode + "这个项目没有进行医保匹配");
                                    //MessageBox.Show("以下项目没有匹配:\r\n" + pp
                                    //           + "\r\n请刷新匹配关系或者重新匹配！");
                                 
                                }
                            }
                        }
                        
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Frmhjsf_Activated(object sender, EventArgs e)
        {
            if (_menuTag.Function_Name == "Fun_ts_mz_hj_Lg") return;
             
                //获得可用发票号
                int err_code; string err_text;
                //Modify By Zj 2013-01-10 动态获取到发票类型
                DataTable tb = Fun.GetFph(InstanceForm.BCurrentUser.EmployeeId, 1, ts_mz_class.mz_sf.GetFpLx(InstanceForm.BCurrentUser.EmployeeId, InstanceForm.BDatabase), out err_code, out err_text, InstanceForm.BDatabase);
                if (tb.Rows.Count != 0)
                    txtfph.Text = Convertor.IsNull(tb.Rows[0]["QZ"], "") + tb.Rows[0]["fph"].ToString().Trim();
                else
                    txtfph.Text = "无可用票据";


                //报价器 欢迎
                string bqybjq = ApiFunction.GetIniString("报价器文件路径", "启用报价器", Constant.ApplicationDirectory + "//ClientWindow.ini");
                if (bqybjq == "true" && _menuTag.Function_Name == "Fun_ts_mz_sf")
                {
                  
                    ts_call.Icall call = ts_call.CallFactory.NewCall(bjqxh);
                    call.Call(ts_call.DmType.欢迎, "您好,欢迎光临");
                }

                if (Bkh == "true")
                    txtkh.Focus();
                else
                    txtmzh.Focus();             
        }

        private void buthelp_Click(object sender, EventArgs e)
        {
            try
            {


                MenuTag tag = new MenuTag();
                tag = _menuTag;
                //tag.Function_Name = "Fun_ts_mz_kgl_kdj";
                //tag.DllName = "ts_mz_gh";
                ts_mz_kgl.Frmbrxxcx f = new ts_mz_kgl.Frmbrxxcx(tag, "病人查询", null);
                f.txtbrxm.Text = txtxm.Text;
                if (txtxm.Text.Trim() == "")
                    f.chkdjsj.Checked = true;
                f.txtbrxm.Focus();
                f.StartPosition = FormStartPosition.CenterScreen;
                f.ShowDialog();

                ReadCard card = new ReadCard(f.return_kdjid, InstanceForm.BDatabase);
                if (card.kdjid != Guid.Empty)
                {
                    cmbklx.SelectedValue = card.klx;
                    txtkh.Text = card.kh;
                    txtkh.Focus();
                    txtkh_KeyPress(sender, new KeyPressEventArgs((char)Keys.Enter));
                }
                else
                {
                    if (f.bok == true)
                    {
                        MessageBox.Show("只能检索有卡的病人", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void chkyb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cmbyblx.Enabled = chkyb.Checked == true ? true : false;
                if (chkyb.Checked == false) cmbyblx.SelectedIndex = -1;
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbklx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int klx = Convert.ToInt32(Convertor.IsNull(cmbklx.SelectedValue, "0"));
                mz_card card = new mz_card(klx, InstanceForm.BDatabase);
                txtmzh.Enabled = true;
                buthelp.Enabled = true;
                if (card.binput == false)
                {

                    txtmzh.Enabled = false;
                    buthelp.Enabled = false;

                }
                if (txtkh.Enabled == true) txtkh.Focus();
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }



        public static String ToDBC(String input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
             
            //获得可用发票号
            int err_code; string err_text;
            DataSet dset = new DataSet();
            #region 第一次使用报表处理
            
            try
            {
                //int err_code = 0;
               // string err_text = "";

                //不是打小票就返回
                if (_cfg1046.Config.Trim() != "2")
                    return;
                try
                {

                    ParameterEx[] parameters = new ParameterEx[10];
                    parameters[0].Text = "@hjid";
                    parameters[0].Value ="('"+ Guid.Empty.ToString()+"')";

                    parameters[1].Text = "@yblx";
                    parameters[1].Value = 0;

                    parameters[2].Text = "@ybzf";
                    parameters[2].Value = 0;

                    parameters[3].Text = "@tfbz";
                    parameters[3].Value = 0;

                    parameters[4].Text = "@fpid";
                    parameters[4].Value = Guid.Empty;

                    parameters[5].Text = "@yhlxid";
                    parameters[5].Value = Guid.Empty;

                    parameters[6].Text = "@jgbm";
                    parameters[6].Value = FrmMdiMain.Jgbm;

                    parameters[7].Text = "@err_code";
                    parameters[7].ParaDirection = ParameterDirection.Output;
                    parameters[7].DataType = System.Data.DbType.Int32;
                    parameters[7].ParaSize = 100;

                    parameters[8].Text = "@err_text";
                    parameters[8].ParaDirection = ParameterDirection.Output;
                    parameters[8].ParaSize = 100;

                    parameters[9].Text = "@tszt";
                    parameters[9].Value = 0;
                    RelationalDatabase Database = new MsSqlServer();
                    Database.Initialize(FrmMdiMain.Database.ConnectionString);
                    //(string hjid, int yblx, decimal ybzf, int tfbz, Guid fpid, Guid yhlxid, long jgbm, out int err_code, out string err_text, int tszt, RelationalDatabase _DataBase)
                    Database.AdapterFillDataSet("SP_MZSF_GetFpResult", parameters, dset, "sfmx", 30);
                    

                    err_code = Convert.ToInt32(parameters[7].Value);
                    err_text = Convert.ToString(parameters[8].Value);
                    Database.Close();
                    Database.Dispose();
                }
                catch (System.Exception err)
                {
                    MessageBox.Show( err.Message);
                }
                #region 只打一张小票

                decimal cwjzhj = 0;
                decimal _xhje = 0;//消费金额
                decimal _yhje = 0;//优惠金额

                // decimal _zje=0;//总金额
                string zhdnlsh = "";//考虑到电脑流水号有多个

                
                
                #region
                 

                ParameterEx[] paramters = new ParameterEx[15];
                paramters[0].Text = "V_医院名称";
                paramters[0].Value = "医院";

                paramters[1].Text = "V_收费日期";
                paramters[1].Value = "";

                paramters[2].Text = "V_收费员";
                paramters[2].Value = "dd";

                paramters[3].Text = "V_病人姓名";
                paramters[3].Value = "";

                paramters[4].Text = "V_门诊号";
                paramters[4].Value = "";

                paramters[5].Text = "V_卡号";
                paramters[5].Value = "";

                paramters[6].Text = "V_电脑流水号";
                paramters[6].Value = zhdnlsh;// +" -" + zhdnlsh;

                paramters[7].Text = "V_消费金额";
                paramters[7].Value = _xhje;
                //  ye = ye - Convert.ToDecimal(tbFp.Rows[0]["cwjz"]);
                paramters[8].Text = "V_卡余额";
                paramters[8].Value = 0;
                paramters[9].Text = "V_医生";
                paramters[9].Value = "";
                paramters[10].Text = "V_科室";
                paramters[10].Value ="";

                paramters[11].Text = "V_优惠金额";
                paramters[11].Value = _yhje.ToString();
                //add by zouchihua 2013-3-26
                paramters[12].Text = "V_现金支付";
                paramters[12].Value ="0";//直接获取收银窗口的值
                //add by zouchihua 2013-3-26
                paramters[13].Text = "V_医保支付";
                paramters[13].Value = "0";//直接获取收银窗口的值
                //add by zouchihua 2013-3-26
                paramters[14].Text = "V_其它支付";
                paramters[14].Value = "0";//直接获取收银窗口的值


                #endregion

                DataTable dtFpxm = dset.Tables[1].Copy();
                dtFpxm.TableName = "收费明细";
                DataTable dtFpwjxm = dset.Tables[4].Copy();
                dtFpwjxm.TableName = "收费物价明细";
                //复制一个表数据
                DataTable
                    tableXpmx = dset.Tables[5].Copy();
                tableXpmx.TableName = "小票明细";
                DataSet _dset = new DataSet();
                _dset.Tables.Add(dtFpxm);
                _dset.Tables.Add(dtFpwjxm);
                _dset.Tables.Add(tableXpmx);
               


                string reportFile = Constant.ApplicationDirectory + "\\Report\\MZSF_小票(只打一张).rpt";
                //TrasenFrame.Forms.FrmReportView fView = new FrmReportView(_dset, reportFile, paramters, false);
               // fView.Show();

                //fView.Close();
                //fView.Dispose();
                #endregion
                CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                CrystalDecisions.CrystalReports.Engine.ReportDocument rd = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

                rd.Load(Constant.CustomDirectory + "\\Report\\MZSF_小票(只打一张).rpt");
                rd.SetDataSource(_dset);
                for (int i = 0; i < paramters.Length; i++)
                {
                    rd.SetParameterValue(paramters[i].Text, paramters[i].Value);
                }
                crystalReportViewer1.ReportSource = rd;
               
                crystalReportViewer1.Show();
                crystalReportViewer1.Dispose();
                //Form f = new Form();
                //f.Controls.Add(crystalReportViewer1);
                //f.ShowDialog();

            }
            catch (Exception ex)
            {
              //  MessageBox.Show(ex.Message);
                 
            }
            #endregion
        }

        //Add by zp 2013-11-18 允许划价使用模板
        private void 存为模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tbs = (DataTable)dataGridView1.DataSource;
                DataTable tb = tbs.Copy();
                MenuTag tag = _menuTag;
                tag.Function_Name = "Fun_ts_mzys_blcflr_grmb";

                DataRow[] rowsX = tb.Select("选择=true");
                if (rowsX.Length == 0)
                {
                    MessageBox.Show("请选择要存为模板的处方", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ts_mzys_blcflr.InstanceForm.BCurrentDept = InstanceForm.BCurrentDept;
                ts_mzys_blcflr.InstanceForm.BCurrentUser = InstanceForm.BCurrentUser;
                ts_mzys_blcflr.InstanceForm.BDatabase = InstanceForm.BDatabase;
                ts_mzys_blcflr.InstanceForm.IsSfy = true;
                Frmblcf f = new Frmblcf(tag, "模板维护", _mdiParent, 0);
                f._MbFunctionName = _menuTag.Function_Name;
                f.butnewmb_Click(sender, e);
                DataTable tbmb = (DataTable)f.dataGridView1.DataSource;
                tbmb.Clear();
               // tbmb.Columns["hjmxid"].DataType = Type.GetType("System.String");
                
                for (int y = 0; y <= rowsX.Length - 1; y++)
                {
                    if (Convertor.IsNull(rowsX[y]["hjmxid"], "") == "99999999")
                        rowsX[y]["hjmxid"] = Guid.NewGuid().ToString();
                }

                for (int x = 0; x <= rowsX.Length - 1; x++)
                {
                    DataRow dr = tbmb.NewRow();
                    dr["选择"] = rowsX[x]["选择"];
                    //dr["组"] = rowsX[x]["组"];
                    //dr["开嘱时间"] = rowsX[x]["开嘱时间"];
                    dr["医嘱内容"] = rowsX[x]["医嘱内容"];
                    dr["剂量"] = rowsX[x]["剂量"];
                    dr["剂量单位"] = rowsX[x]["剂量单位"];
                    dr["频次"] = rowsX[x]["频次"];
                    dr["用法"] = rowsX[x]["用法"];
                    dr["天数"] = rowsX[x]["天数"];
                    dr["嘱托"] = rowsX[x]["嘱托"];
                    dr["单价"] = rowsX[x]["单价"];
                    dr["剂数"] = rowsX[x]["剂数"];
                    dr["数量"] = rowsX[x]["数量"];
                    dr["单位"] = rowsX[x]["单位"];
                    dr["金额"] = rowsX[x]["金额"];
                   // dr["确认锁定"] = rowsX[x]["金额"];
                    //dr["开嘱医生"] =
                    dr["执行科室"] = rowsX[x]["执行科室"];
                    dr["序号"] = rowsX[x]["序号"];
                    dr["hjid"] = rowsX[x]["hjid"];
                    dr["拼音码"] = rowsX[x]["拼音码"];
                    dr["编码"] = rowsX[x]["编码"];
                    dr["项目名称"] = rowsX[x]["项目名称"];
                    dr["商品名"] = rowsX[x]["商品名"];
                    dr["规格"] = rowsX[x]["规格"];
                    dr["厂家"] = rowsX[x]["厂家"];
                    dr["剂量单位id"] = rowsX[x]["剂量单位id"];
                    dr["dwlx"] = rowsX[x]["dwlx"];
                    dr["频次id"] = rowsX[x]["频次id"];
                    dr["用法ID"] = rowsX[x]["用法ID"];
                    dr["ydwbl"] = rowsX[x]["ydwbl"];
                    dr["统计大项目"] = rowsX[x]["统计大项目"];
                    dr["项目id"] = rowsX[x]["项目ID"];
                    dr["hjmxid"] = rowsX[x]["hjmxid"];
                    dr["自备药"] = rowsX[x]["自备药"];
                    dr["皮试标志"] = rowsX[x]["皮试标志"];
                    dr["pshjmxid"] = rowsX[x]["pshjmxid"];
                    dr["处方分组序号"] = rowsX[x]["处方分组序号"];
                    dr["排序序号"] = rowsX[x]["排序序号"];
                    dr["执行科室id"] = rowsX[x]["执行科室id"];
                    dr["科室id"] = rowsX[x]["科室id"];
                    dr["医生id"] = rowsX[x]["医生id"];
                    dr["住院科室id"] = rowsX[x]["住院科室id"];
                    dr["项目来源"] = rowsX[x]["项目来源"];
                    dr["套餐ID"] = rowsX[x]["套餐ID"];
                    //dr["修改"] = rowsX[x]["修改"];
                    //dr["收费"] = rowsX[x]["收费"];
                    dr["单价可改"] = rowsX[x]["单价可改"];
                    dr["划价日期"] = rowsX[x]["划价日期"];
                    dr["划价员"] = rowsX[x]["划价员"];
                    dr["hjy"] = rowsX[x]["hjy"];
                    dr["划价窗口"] = rowsX[x]["划价窗口"];
                    dr["批发价"] = rowsX[x]["批发价"];
                    dr["批发金额"] = rowsX[x]["批发金额"];
                    dr["yzid"] = rowsX[x]["yzid"];
                    dr["yzmc"] = rowsX[x]["医嘱内容"];
                    dr["分方状态"] = rowsX[x]["分方状态"];
                   // dr["警示灯"] = rowsX[x]["警示灯"];
                   // tbmb.Rows.Add(rowsX[x].ItemArray);
                    //tbmb.Rows.Add(rowsX[x].ItemArray);
                    tbmb.Rows.Add(dr);
                }

                for (int x = 0; x <= tbmb.Rows.Count - 1; x++)
                {
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【皮试】", "");
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【-】", "");
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【+】", "");
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【免试】", "");
                    tbmb.Rows[x]["医嘱内容"] = tbmb.Rows[x]["医嘱内容"].ToString().Replace("【皮试液】", "");
                }

                f.butnew_Click(sender, e);
                f.dataGridView1.DataSource = tbmb;
                f.MdiParent = _mdiParent;
                f.Show();
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现错误!原因:" + ea.Message, "提示");
            }
        }
        //选择模板 Add by zp 2013-11-20
        private void ToolStrip_SelectMb_Click(object sender, EventArgs e)
        {
            try
            {
                //if (Dqcf.ghxxid == Guid.Empty)
                //{  MessageBox.Show("未找到挂号信息,无法录入项目!", "提示"); return;}
                if (Dqcf.ghxxid == Guid.Empty)
                {
                    if (lblmzh.Text.Trim() == "" || txtxm.Text.Trim() == "" || txtmzh.Text == "")
                    {
                        MessageBox.Show("没有输入病人信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if ((Convert.ToInt32(txtys.Tag) == 0 || txtys.Text.Trim() == "") && _menuTag.Function_Name != "Fun_ts_mz_hj_Lg")
                {
                    MessageBox.Show("请输入医生");
                    txtys.Focus();
                    return;
                }
                if (Convert.ToInt32(txtks.Tag) == 0 || txtks.Text.Trim() == "")
                {
                    MessageBox.Show("请输入科室");
                    txtks.Focus();
                    return;
                }
                //butnew_Click(null, null);

                DataTable dt_cf = (DataTable)this.dataGridView1.DataSource;
                Frm_SFMb_Select frm = new Frm_SFMb_Select(dt_cf);
                frm.ShowDialog();
                DataTable dt_mbmx = frm.dt_mbmx;
                bool check = frm.isff_check;
                /*得到模板明细后填充到界面*/
                if (dt_mbmx == null || dt_mbmx.Rows.Count <= 0) return;
                butnew_Click(null, null);
                //if (this.dataGridView1.Rows.Count > 0)
                //    this.dataGridView1.CurrentCell = this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells["编码"];
                SetDgvValue(dt_mbmx, check);
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:"+ea.Message,"提示");
            }
        }

        private void SetDgvValue(DataTable tab,bool check)
        {
            try
            {
                DataTable tb = (DataTable)this.dataGridView1.DataSource;
                string[] GroupbyField = { "CFXH" };
                string[] ComputeField = { };
                string[] CField = { };
                TrasenFrame.Classes.TsSet xcset = new TrasenFrame.Classes.TsSet();
                xcset.TsDataTable = tab;
                DataTable tbcf = xcset.GroupTable(GroupbyField, ComputeField, CField, "序号<>'小计'");//处方头表

               int nrow = this.dataGridView1.CurrentCell == null ? 0 : this.dataGridView1.CurrentCell.RowIndex;
               //add by zouchihua 2013-7-11 获得改科室对应的门诊药房
               string mzyf = "select drugstore_id,ksmc from   jc_dept_drugstore a join YP_YJKS b on a.DRUGSTORE_ID=b.DEPTID  where delete_bit=0 and dept_id=" + InstanceForm.BCurrentDept.DeptId + " and  convert(nvarchar,getdate(),108)>=convert(nvarchar,a.kssj,108)  "
                     + "  and convert(nvarchar,getdate(),108)<=convert(nvarchar,a.jssj,108) and  KSLX2='门诊药房'";
               DataTable tbmzyf = InstanceForm.BDatabase.GetDataTable(mzyf);
               //循环处方
               for (int x = 0; x <= tbcf.Rows.Count - 1; x++)
               {
                   DataRow[] rows_cf = tab.Select("CFXH='" + tab.Rows[x]["CFXH"].ToString().Trim() + "'"); //得到选中的处方明细
                   bool Badd = false;

                   #region 添加每个处方明细
                   decimal cfje = 0;
                   for (int i = 0; i <= rows_cf.Length - 1; i++)
                   {
                       if (Convertor.IsNull(rows_cf[i]["序号"], "").Trim() == "小计") //Add by zp 2013-10-22 出现小计行就continue 
                           continue;
                       //nrow = cell.nrow;//获得当前的行下标
                       int xmly = Convert.ToInt32(rows_cf[i]["项目来源"]);
                       long xmid = Convert.ToInt64(rows_cf[i]["项目id"]);
                       int cjid = Convert.ToInt32(rows_cf[i]["cjid"]);
                       string zxksmc = Convertor.IsNull(rows_cf[i]["执行科室"], "");
                       int zxksid = Convert.ToInt32(rows_cf[i]["执行科室id"]);

                       DataRow[] rows = null;
                       if (xmly == 1)
                       {
                           #region 药品
                           int flagzd = 0;//找到
                           string where = "";
                           //add by zouchihua 2013-7-11 优先考虑门诊药房
                           if (rdomzyf.Checked && tbmzyf.Rows.Count > 0)
                           {
                               DataRow[] drmzyf = tbmzyf.Select("drugstore_id=" + zxksid + "");
                               #region//如果没有找到门诊药房，优先门诊
                               if (drmzyf.Length <= 0)
                               {

                                   for (int j = 0; j < tbmzyf.Rows.Count; j++)
                                   {
                                       zxksid = Convert.ToInt32(tbmzyf.Rows[j]["drugstore_id"]);
                                       where = "项目id=" + cjid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                                       rows = PubDset.Tables["ITEM"].Select(where, "zxksid");
                                       if (rows.Length == 0)
                                       {
                                           where = "ggid=" + xmid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                                           rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                                       }
                                       if (rows.Length == 0)
                                       {
                                           where = "项目id=" + cjid + " AND 项目来源=" + xmly + "";
                                           rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                                       }
                                       if (rows.Length == 0)
                                       {
                                           where = "ggid=" + xmid + " AND 项目来源=" + xmly + "";
                                           rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                                       }
                                       if (rows.Length > 0)
                                       {
                                           flagzd = 1;
                                           break;
                                       }
                                   }
                               }
                               #endregion
                           }

                           //如果还是没有找到就找原科室的
                           if (flagzd == 0)
                           {
                               #region 在原执行科室寻找指定的项目
                               zxksid = Convert.ToInt32(rows_cf[i]["执行科室id"]);
                               where = "项目id=" + cjid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                               rows = PubDset.Tables["ITEM"].Select(where, "zxksid");
                               if (rows.Length == 0)
                               {
                                   where = "ggid=" + xmid + " AND 项目来源=" + xmly + " and zxksid=" + zxksid + "";
                                   rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                               }
                               if (rows.Length == 0)
                               {
                                   where = "项目id=" + cjid + " AND 项目来源=" + xmly + "";
                                   rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                               }
                               if (rows.Length == 0)
                               {
                                   where = "ggid=" + xmid + " AND 项目来源=" + xmly + "";
                                   rows = PubDset.Tables["ITEM"].Select(where, "zxksid");

                               }
                               if (rows.Length == 0)
                               {
                                   string ss = "";
                                   Ypgg gg = new Ypgg(Convert.ToInt32(xmid.ToString()), InstanceForm.BDatabase);
                                   ss = "没有找到药品 [" + gg.YPPM + " " + gg.YPGG + " ] 可能没有库存或已停用";
                                   MessageBox.Show(ss, "导入模板", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                   if (tb.Rows.Count > 1 && tb.Rows[nrow]["项目id"].ToString() == "" && tb.Rows[nrow]["序号"].ToString() != "小计" && i == rows_cf.Length - 1 && i != 0)
                                       tb.Rows.Remove(tb.Rows[nrow]);
                                   continue;
                               }
                               #endregion
                           }
                           #endregion
                       }
                       else
                       {
                           #region 非药品
                           string where = "项目id=" + xmid + " AND 项目来源=" + xmly + " ";//and 执行科室id=" + zxksid + "";
                           if (tbcf.Columns.Contains("tc_flag")) //如果有tc_flag标志 Add by zp 2013-12-13 区分套餐和非套餐
                           {
                               int tcflag = Convert.ToInt32(Convertor.IsNull(rows_cf[i]["tc_flag"], "0"));
                               if (tcflag > 0)
                                   where += " and 套餐>0";
                               else
                                   where += " and 套餐<=0";
                           }
                         
                           rows = PubDset.Tables["ITEM"].Select(where);
                           if (rows.Length == 0)
                           {
                               MessageBox.Show("没有找到" + rows_cf[i]["医嘱内容"].ToString() + ",可能已停用", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                               if (tb.Rows.Count > 1 && tb.Rows[nrow]["项目id"].ToString() == "" && tb.Rows[nrow]["序号"].ToString() != "小计" && i == rows_cf.Length - 1 && i != 0)
                                   tb.Rows.Remove(tb.Rows[nrow]);
                               continue;
                           }
                           #endregion
                       }

                       
                       if (rows.Length > 0) //如果查到了模板内的项目
                       {
                           int nrowX = nrow; //得到当前下标
                           DataRow dr = tb.NewRow();
                           tb.Rows.Add(dr);
                           //DataRow dr_add = null;
                         
                           Addrow(rows[0]);// ref nrow
                           butnew_Click(null, null);
                           dataGridView1.CurrentCell = dataGridView1["项目名称", tb.Rows.Count - 1];
                           #region 注释代码
                           // DataRow[] SelRow = tb.Select("项目id=" + rows[0]["项目id"].ToString() + " and 项目来源=" + rows[0]["项目来源"].ToString() + " and hjmxid='" + Guid.Empty.ToString() + "'");
                           //if (SelRow.Length == 0) continue;
                           // Badd = true;
                           //SelRow[SelRow.Length - 1]["剂量"] = rows_cf[i]["剂量"];
                           //SelRow[SelRow.Length - 1]["剂量单位"] = rows_cf[i]["剂量单位"];
                           //SelRow[SelRow.Length - 1]["剂量单位id"] = rows_cf[i]["剂量单位id"];
                           //SelRow[SelRow.Length - 1]["dwlx"] = rows_cf[i]["dwlx"];
                           //SelRow[SelRow.Length - 1]["用法"] = rows_cf[i]["用法"];
                           //SelRow[SelRow.Length - 1]["用法id"] = rows_cf[i]["用法id"];
                           //SelRow[SelRow.Length - 1]["频次"] = rows_cf[i]["频次"];
                           //SelRow[SelRow.Length - 1]["频次id"] = rows_cf[i]["频次id"];
                           //SelRow[SelRow.Length - 1]["天数"] = rows_cf[i]["天数"];
                           //SelRow[SelRow.Length - 1]["嘱托"] = rows_cf[i]["嘱托"];
                           //SelRow[SelRow.Length - 1]["处方分组序号"] = rows_cf[i]["处方分组序号"];
                           //if (check && cfg3039.Config == "1")
                           //{
                           //    string[] GroupbyField1 = { "分方状态", "项目来源", "收费", "修改" };
                           //    string[] ComputeField1 = { };
                           //    string[] CField1 = { };
                           //    TrasenFrame.Classes.TsSet xcset1 = new TrasenFrame.Classes.TsSet();
                           //    xcset1.TsDataTable = tb;
                           //    DataTable wsfcftb1 = xcset1.GroupTable(GroupbyField1, ComputeField1, CField1, "序号<>'小计'  and 项目来源=1 ");
                           //    DataRow[] wsfdr = wsfcftb1.Select("收费=0 and 修改=1");
                           //    //Add By Zj 2012-05-25
                           //    if (rows_cf[i]["统计大项目"] != SelRow[SelRow.Length - 1]["统计大项目"])
                           //    {
                           //        if (rows_cf[i]["统计大项目"].ToString() == "01" || rows_cf[i]["统计大项目"].ToString() == "02")
                           //        {
                           //            if (SelRow[SelRow.Length - 1]["统计大项目"].ToString() != "01" && SelRow[SelRow.Length - 1]["统计大项目"].ToString() != "02")
                           //            {
                           //                SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["CFXH"];
                           //            }
                           //            else
                           //            {
                           //                SelRow[SelRow.Length - 1]["分方状态"] = wsfdr[0]["分方状态"];
                           //            }
                           //        }
                           //        else if (rows_cf[i]["统计大项目"].ToString() == "03" && wsfdr[0]["统计大项目"].ToString() == "03")
                           //        {
                           //            SelRow[SelRow.Length - 1]["分方状态"] = wsfdr[0]["分方状态"];
                           //            if (wsfdr[0]["分方状态"].ToString() != "")
                           //                rows_cf[0]["CFXH"] = Convertor.IsNull(wsfdr[0]["分方状态"], "");

                           //            SelRow[SelRow.Length - 1]["剂数"] = wsfdr[0]["剂数"];//Add By Zj 2012-04-10

                           //        }
                           //        else
                           //        {
                           //            SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["CFXH"];
                           //        }
                           //    }
                           //    else
                           //        SelRow[SelRow.Length - 1]["分方状态"] = wsfdr[0]["分方状态"];
                           //}
                           //else
                           //{
                           //    if (cfg3048.Config == "0")//Add By Zj 2013-01-09 begin 
                           //        SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["cfxh"].ToString();
                           //    else
                           //    {
                           //        string sql = "select HYLXID from JC_ASSAY where YZID=" + SelRow[SelRow.Length - 1]["yzid"].ToString();
                           //        SelRow[SelRow.Length - 1]["分方状态"] = Convertor.IsNull(InstanceForm.BDatabase.GetDataResult(sql), "");
                           //    }
                           //    //Add By Zj 2013-01-09 end 
                           //    //SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["cfxh"].ToString(); Modify By ZJ 2013-01-09
                           //    SelRow[SelRow.Length - 1]["剂数"] = rows_cf[i]["剂数"].ToString();//Add By Zj 2012-04-10
                           //}

                           //SelRow[SelRow.Length - 1]["排序序号"] = rows_cf[i]["排序序号"];
                           //SelRow[SelRow.Length - 1]["自备药"] = rows_cf[i]["自备药"];
                           ////SelRow[SelRow.Length - 1]["分方状态"] = rows_cf[i]["cfxh"].ToString(); By Zj 2012-05-25

                           //if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb"
                           //    || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb"
                           //    || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb")
                           //    SelRow[SelRow.Length - 1]["hjid"] = rows_cf[i]["cfxh"];


                           //if (rows_cf[i]["自备药"].ToString() == "1") SelRow[SelRow.Length - 1]["医嘱内容"] = SelRow[SelRow.Length - 1]["医嘱内容"] + " 【自备】";
                           //if (rows_cf[i]["处方分组序号"].ToString() == "1") { b_ks = true; SelRow[SelRow.Length - 1]["医嘱内容"] = "┌" + SelRow[SelRow.Length - 1]["医嘱内容"].ToString(); }
                           //if (rows_cf[i]["处方分组序号"].ToString() == "2" && b_ks == true) { SelRow[SelRow.Length - 1]["医嘱内容"] = "│" + SelRow[SelRow.Length - 1]["医嘱内容"].ToString(); }
                           //if (rows_cf[i]["处方分组序号"].ToString() == "-1" && b_ks == true) { b_ks = false; SelRow[SelRow.Length - 1]["医嘱内容"] = "└" + SelRow[SelRow.Length - 1]["医嘱内容"].ToString(); }

                           //bool bok = false;
                           //Seek_Price(SelRow[SelRow.Length - 1], out bok);
                           //cfje = cfje + Convert.ToDecimal(SelRow[SelRow.Length - 1]["金额"]);


                           //if (i < rows_cf.Length - 1 && rows_cf[i]["项目id"].ToString() != "")
                           //{
                           //    DataRow row = tb.NewRow();
                           //    tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                           //    row["修改"] = true;
                           //    //row["收费"] = false;

                           //    if (cfg3048.Config == "0")//Add By Zj 2013-01-09
                           //        row["分方状态"] = rows_cf[i]["cfxh"].ToString();
                           //    else
                           //    {
                           //        string sql = "select HYLXID from JC_ASSAY where YZID=" + SelRow[SelRow.Length - 1]["yzid"].ToString();
                           //        row["分方状态"] = Convertor.IsNull(InstanceForm.BDatabase.GetDataResult(sql), "");
                           //    }
                           //    tb.Rows.Add(row);
                           //    dataGridView1.DataSource = tb;
                           //    dataGridView1.CurrentCell = dataGridView1["项目名称", tb.Rows.Count - 1];
                           //}
                           //else
                           //{
                           //    dataGridView1.CurrentCell = dataGridView1["项目名称", tb.Rows.Count - 1];
                           //}
                           #endregion
                       }
                   }
                   #endregion
                   if (tbcf.Rows.Count == 1)
                   {

                       DataRow row = tb.NewRow();
                       tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                       row["修改"] = true;
                       // row["收费"] = false;
                       row["分方状态"] = rows_cf[0]["cfxh"].ToString();
                       tb.Rows.Add(row);
                       dataGridView1.DataSource = tb;
                       dataGridView1.CurrentCell = dataGridView1["项目名称", tb.Rows.Count - 1];
                   }
                   if (rows_cf.Length > 0 && Badd == true && x < tbcf.Rows.Count && tbcf.Rows.Count != 1)
                   {

                       DataRow row = tb.NewRow();
                       row["序号"] = "小计";
                       row["修改"] = true;
                       //row["收费"] = false;
                       row["选择"] = false;
                       row["金额"] = cfje.ToString();

                       if (_menuTag.Function_Name == "Fun_ts_mzys_blcflr_grmb"
                               || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_yjmb"
                               || _menuTag.Function_Name == "Fun_ts_mzys_blcflr_kjmb")
                           row["hjid"] = rows_cf[0]["cfxh"];
                       else
                           row["hjid"] = Guid.Empty.ToString();
                       row["分方状态"] = rows_cf[0]["cfxh"].ToString();
                       cfje = 0;
                       tb.Rows.Add(row);
                       dataGridView1.DataSource = tb;
                       dataGridView1.CurrentCell = dataGridView1["项目名称", tb.Rows.Count - 1];

                       if (x < tbcf.Rows.Count - 1)
                       {
                           DataRow row1 = tb.NewRow();
                           tb.Rows[tb.Rows.Count - 1]["序号"] = "";
                           row1["修改"] = true;
                           //row1["收费"] = false;
                           tb.Rows.Add(row1);
                           dataGridView1.DataSource = tb;
                           dataGridView1.CurrentCell = dataGridView1["项目名称", tb.Rows.Count - 1];
                       }
                   }
               }
               ModifCfje(tb, "");
               butnew_Click(null, null);
                
            }
            catch (Exception ea)
            {
                MessageBox.Show("出现异常!原因:" + ea.Message, "提示");
            }
        }

        //计算用量和价格
        private void Seek_Price(DataRow row, out bool bok)
        {
            bok = true;
            int xmly = Convert.ToInt32(Convertor.IsNull(row["项目来源"], "0"));
            if (xmly == 1)
            {
                int dwlx = Convert.ToInt32(row["dwlx"]);
                decimal jl = Convert.ToDecimal(Convertor.IsNull(row["剂量"], "0"));
                int pcid = Convert.ToInt32(Convertor.IsNull(row["频次id"], "0"));
                pc pc = new pc(pcid, InstanceForm.BDatabase);
                decimal ts = Convert.ToDecimal(Convertor.IsNull(row["天数"], "0"));
                int js = Convert.ToInt32(Convertor.IsNull(row["剂数"], "0"));
                int cjid = Convert.ToInt32(row["项目id"]);
                int yfid = Convert.ToInt32(row["执行科室id"]);

                DataTable tb = null;
                if (Dqcf.tjdxmdm != "03")
                    tb = mzys.Seek_Yp_Price(dwlx, jl, pc.zxcs, pc.jgts, ts, cjid, yfid, 0, InstanceForm.BDatabase);
                else
                    tb = mzys.Seek_Yp_Price(dwlx, jl, 1, 1, 1, cjid, yfid, 0, InstanceForm.BDatabase);


                row["单价"] = tb.Rows[0]["price"];
                row["单价可改"] = false;
                row["修改"] = true;
                //row["收费"] = false;
                row["YDWBL"] = tb.Rows[0]["ydwbl"];
                row["数量"] = "0";
                row["单位"] = tb.Rows[0]["unit"];
                if (Dqcf.tjdxmdm != "03")
                    row["金额"] = "0";
                else
                    row["金额"] = "0";
                //Modify by zouchihua 屏蔽2013-5-3 如果是药品 addrow 方法中已经获得批发价，和批发金额
                //row["批发价"] = "0";
                //row["批发金额"] = "0";

                //库存控制
                //Add By Zj 新增判断 有些很老的医院 没有bdelete和kcl这两个列 值为空 所以加判断
                bool Bdelete = Convert.ToBoolean(Convert.ToInt32(Convertor.IsNull(tb.Rows[0]["bdelete"], "0")));
                decimal sl = Convert.ToDecimal(tb.Rows[0]["yl"]);
                decimal kcl = Convert.ToDecimal(Convertor.IsNull(tb.Rows[0]["kcl"], "0"));
                if (Bdelete == true)
                {
                    bok = false;
                    MessageBox.Show("该药品已被暂停使用!!! 药房库存 " + Convert.ToDouble(kcl).ToString() + tb.Rows[0]["unit"].ToString().Trim() + "，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //如果总数大于库存量的话，判断是否是模板维护。如果是模板维护就不需要判断库存量
                if ((sl * js) > kcl && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_grmb" && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_kjmb" && _menuTag.Function_Name != "Fun_ts_mzys_blcflr_yjmb")
                {
                    if (kckz.Config == "1")
                    {
                        bok = false;
                        MessageBox.Show("当前<" + row["yzmc"].ToString().Trim() + ">药品库存量只有" + Convert.ToDouble(kcl).ToString() + tb.Rows[0]["unit"].ToString().Trim() + "，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (MessageBox.Show(this, "当前<" + row["yzmc"].ToString().Trim() + ">药品库存量只有" + Convert.ToDouble(kcl).ToString() + tb.Rows[0]["unit"].ToString().Trim() + ",您要继续吗?", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            bok = false;
                            return;
                        }
                    }
                }

                row["数量"] = tb.Rows[0]["yl"];
                row["单位"] = tb.Rows[0]["unit"].ToString().Trim();
                //Add By Zj 2012-03-17 解决中药批发价为0
                if (Dqcf.tjdxmdm != "03")
                {
                    row["金额"] = tb.Rows[0]["sdvalue"];
                    row["批发金额"] = tb.Rows[0]["pfje"];
                }
                else
                {
                    decimal lsje = Convert.ToDecimal(row["单价"]) * Convert.ToDecimal(row["数量"]) * js;
                    row["金额"] = Math.Round(lsje, 2);

                    decimal pfje = Convert.ToDecimal(row["批发价"]) * Convert.ToDecimal(row["数量"]) * js;
                    row["批发金额"] = Math.Round(pfje, 2);
                }
                //row["批发价"] = tb.Rows[0]["pfj"];
                //row["批发金额"] = tb.Rows[0]["pfje"];
                if (row["皮试标志"].ToString() == "0" && new SystemCfg(3002).Config == "1" && Dqcf.tjdxmdm != "03")
                {
                    int _sl = Convert.ToInt32(tb.Rows[0]["yl"]);
                    if (_sl >= 1) _sl = _sl - 1;
                    row["数量"] = _sl.ToString();
                    Decimal _je = Convert.ToDecimal(tb.Rows[0]["price"]) * _sl;
                    row["金额"] = _je.ToString();
                }
                //Modify By Zj 2012-06-25
                //if (row["皮试标志"].ToString() == "9" && Convert.ToInt32(Convert.ToDecimal(row["数量"].ToString())) > 1)
                //{
                //    if (MessageBox.Show("您确定皮试液需要开大于1支的用量吗?", "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                //    {
                //        row["数量"] = 1;
                //    }
                //}
            }
            else
            {
                decimal jl = Convert.ToDecimal(Convertor.IsNull(row["剂量"], "0"));
                decimal price = Convert.ToDecimal(Convertor.IsNull(row["单价"], "0"));
                int pcid = Convert.ToInt32(Convertor.IsNull(row["频次id"], "0"));
                pc pc = new pc(pcid, InstanceForm.BDatabase);
                decimal ts = Convert.ToDecimal(Convertor.IsNull(row["天数"], "0"));
                decimal _sl = jl * pc.zxcs * ts / pc.jgts;
                decimal sl = _sl;
                decimal je = sl * price;
                row["单价"] = price.ToString();
                if (price == 0)
                    row["单价可改"] = true;
                row["修改"] = true;
                //row["收费"] = false;
                row["数量"] = sl.ToString();
                //row["单位"] = tb.Rows[0]["unit"];
                row["金额"] = je.ToString();
                row["YDWBL"] = "1";
                row["批发价"] = "0";
                row["批发金额"] = "0";
            }
        }

        private void txtks_Leave(object sender, EventArgs e)
        {
            FunAddComboBox.AddYhfa(Convert.ToInt32(Convertor.IsNull(lblklx.Tag, "0")), new Guid(Convertor.IsNull(lblkh.Tag, Guid.Empty.ToString())), Convert.ToInt32(Convertor.IsNull(lblbrlx.Tag, "0")), Convert.ToInt32(Convertor.IsNull(cmbyblx.SelectedValue, "0")), Convert.ToInt32(Convertor.IsNull(lblhtdwlx.Tag, "0")), _menuTag.Function_Name,cmbyhlx, InstanceForm.BDatabase);
        }

        private void btnkcz_Click(object sender, EventArgs e)
        {
            Frmbrkcz Frmbrkcz = new Frmbrkcz(InstanceForm.BDatabase,InstanceForm.BCurrentUser,InstanceForm.BCurrentDept,Convert.ToInt32(cmbklx.SelectedValue),txtkh.Text.Trim());

            Frmbrkcz.ShowDialog();

            string ssq = "";
            ssq = "select * from YY_KDJB   where klx=" + Convert.ToInt32(cmbklx.SelectedValue) + " and kh='" + txtkh.Text.Trim() + "'  and ZFBZ=0 ";
            tbk = InstanceForm.BDatabase.GetDataTable(ssq);
            if (tbk.Rows.Count != 0)
                lblkye.Text = tbk.Rows[0]["kye"].ToString();

        }

    }
}
