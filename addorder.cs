public string AddOrder(string hid, string cartinfo, string countrycode, string telephone, string verifycode, string special, string sectionid, string tablenumber, string email, string lastname, string firstname, string ordertype, string scheduledtime, string paymethod, string schedulemealminute, string profileid, string nowscheduledtime, string nowlatertime, string couponcode, string paytypeid, string payid, string cuslocation, string cuslocationname, string cuslocationjson, string paymentlist, string isapp, string virtualprofileid, string tokennumber,string kiosknumber,string deliverydistance,string randomnumber,string language,string chooseeatinmethod)
        {
            JObject datajson = JObject.Parse(cartinfo);
            string dbconnstr = ConfigurationManager.AppSettings["connstr"];
            DbCommonContext _db = new DbCommonContext(dbconnstr);
            var centerhotel = _db.Hotels.AsNoTracking().SingleOrDefault(x => x.Hid == hid);

            string grpid = centerhotel.Hid;
            if (centerhotel != null && !string.IsNullOrWhiteSpace(centerhotel.Grpid) && centerhotel.Grpid != hid)
            {
                grpid = centerhotel.Grpid;
            }

            var dbitem = _db.DbLists.AsNoTracking().SingleOrDefault(x => x.Id == centerhotel.Dbid);
            string connstr = ConnStrHelper.GetConnStr(dbitem.DbServer, dbitem.DbName, dbitem.LogId, dbitem.LogPwd, "GemstarBSPMS", dbitem.IntIp, _db.IsConnectViaInternetIp());
            using (DbHotelPmsContext pmsContext = new DbHotelPmsContext(connstr,hid, "admin", new HttpRequestWrapper(HttpContext.Current.Request).RequestContext.HttpContext.Request, ProductType.Member))
            {
                using (var pmsdbtransaction = pmsContext.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                {
                    Random random = new Random(Guid.NewGuid().GetHashCode());
                    string FormId = "";




                    bool isonlinepay = false;


                    bool iskiosk = false;

                    //discountprice 暂时不用

                    //countrycode，mobilephone,tablenumber,customerspecial 需要从SubmitOrder方法传递 ， 其他值都从 ihotelInfoService.GetLocationorderCartInfo 方法获取

                    #region 这里判断是否有该支付方式
                    Cloudpayment paymentmodel = null;
                    if (!string.IsNullOrEmpty(payid))
                    {
                        Guid paygid = Guid.Parse(payid);
                        Cloudlocationpayment locationpayment = pmsContext.Cloudlocationpayment.AsNoTracking().FirstOrDefault(x => x.Hid == hid && x.Id == paygid && x.Isdelete == 0);
                        if (locationpayment != null)
                        {
                            paymentmodel = _db.Cloudpayment.AsNoTracking().FirstOrDefault(x => x.Id == locationpayment.Typeid);
                            if (paymentmodel != null)
                            {
                                paytypeid = paymentmodel.Id.ToString();
                                isonlinepay = paymentmodel.Isonlinepay;

                                //if (paymentmodel.Iskiosk.HasValue && paymentmodel.Iskiosk.Value)
                                //{
                                //    //如果是Kiosk的刷卡支付，不给顺序号先
                                //    if (paymentmodel.Id == Guid.Parse("40202486-4C8C-4AED-ADA7-B7D62DABC01C"))
                                //    {
                                //        isonlinepay = true;
                                //    }
                                //}
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    #endregion


                    Guid? profileguid = null;
                    if (!string.IsNullOrEmpty(profileid))
                    {
                        try
                        {
                            profileguid = Guid.Parse(profileid);
                        }
                        catch (Exception)
                        {
                            profileguid = null;
                        }
                    }

                    Cloudlocationorder cloudlocationordermodel = new Cloudlocationorder();
                    cloudlocationordermodel.Id = Guid.NewGuid();

                    cloudlocationordermodel.Profileid = profileguid;
                    if (!string.IsNullOrWhiteSpace(virtualprofileid))
                    {
                        cloudlocationordermodel.Virtualprofileid = Guid.Parse(virtualprofileid);
                    }
                    cloudlocationordermodel.Countrycode = countrycode;

                    //剔除掉手机号没用的字符串
                    if (!string.IsNullOrWhiteSpace(telephone))
                    {
                        telephone = telephone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("（", "").Replace("）", "");
                    }

                    cloudlocationordermodel.Mobilephone = telephone;
                    cloudlocationordermodel.Cartprice = Convert.ToDecimal(datajson["cartprice"]?.ToString());
                    cloudlocationordermodel.Discountprice = Convert.ToDecimal(datajson["discountprice"] == null ? 0 : datajson["discountprice"]);
                    cloudlocationordermodel.Taxprice = Convert.ToDecimal(datajson["taxprice"]?.ToString());

                    if (datajson["cartpoints"] != null)
                    {
                        cloudlocationordermodel.Cartpoints = Convert.ToDecimal(datajson["cartpoints"]);
                    }

                    cloudlocationordermodel.Tipprice = Convert.ToDecimal(datajson["tipprice"] == null ? 0 : datajson["tipprice"]);
                    cloudlocationordermodel.Drivertipprice = Convert.ToDecimal(datajson["drivertipprice"] == null ? 0 : datajson["drivertipprice"]);
                    cloudlocationordermodel.Deliveryprice = Convert.ToDecimal(datajson["deliveryprice"] == null ? 0 : datajson["deliveryprice"]);

                    cloudlocationordermodel.Totalprice = Convert.ToDecimal(datajson["totalsumprice"]?.ToString());
                    cloudlocationordermodel.Sectionid = !string.IsNullOrEmpty(sectionid) ? int.Parse(sectionid) : 0;
                    cloudlocationordermodel.Tablenumber = (!string.IsNullOrEmpty(tablenumber) ? tablenumber : "");


                    cloudlocationordermodel.Kioskcode = (!string.IsNullOrEmpty(kiosknumber) ? kiosknumber : "");

                    cloudlocationordermodel.Customerspecial = special;
                    cloudlocationordermodel.Notes = "";
                    cloudlocationordermodel.Status = 0;
                    cloudlocationordermodel.Ispay = 0;
                    cloudlocationordermodel.Paytime = null;
                    cloudlocationordermodel.Paymethod = paymethod;
                    cloudlocationordermodel.Tokennumber = tokennumber;
                    if (!string.IsNullOrWhiteSpace(paytypeid))
                    {
                        cloudlocationordermodel.Paytypeid = Guid.Parse(paytypeid);
                    }
                    if (!string.IsNullOrWhiteSpace(payid))
                    {
                        cloudlocationordermodel.Payid = Guid.Parse(payid);
                    }
                    if (!string.IsNullOrWhiteSpace(isapp))
                    {
                        cloudlocationordermodel.Isapp = true;
                    }
                    else
                    {
                        cloudlocationordermodel.Isapp = false;
                    }

                    cloudlocationordermodel.Payorderno = null;
                    cloudlocationordermodel.Completetime = null;
                    cloudlocationordermodel.Hid = hid;
                    cloudlocationordermodel.Createdate = DateTime.UtcNow;
                    cloudlocationordermodel.Email = email;
                    cloudlocationordermodel.Lastname = lastname;
                    cloudlocationordermodel.Firstname = firstname;
                    cloudlocationordermodel.Ordertype = ordertype;

                    if (!string.IsNullOrWhiteSpace(randomnumber))
                    {
                        cloudlocationordermodel.Randomstr = randomnumber;
                    }

                    if (ordertype == "delivery" && !string.IsNullOrWhiteSpace(deliverydistance))
                    {
                        cloudlocationordermodel.Deliverydistance = deliverydistance;
                    }

                    string ackordermode = "0";
                    if (centerhotel.Ackordermode.HasValue)
                    {
                        ackordermode = centerhotel.Ackordermode.Value.ToString();
                    }
                    if (ordertype != "pickup" && ordertype != "delivery")
                    {
                        ackordermode = "0";
                    }

                    if ((ackordermode == "1" || ackordermode == "2") && (cloudlocationordermodel.Ordertype == "pickup" || cloudlocationordermodel.Ordertype == "delivery"))
                    {
                        cloudlocationordermodel.Isackorder = true;

                        if (ackordermode == "2")
                        {
                            cloudlocationordermodel.issimplifyackorder = true;
                        }
                        else
                        {
                            cloudlocationordermodel.issimplifyackorder = false;
                        }
                    }

                    cloudlocationordermodel.Isonlinepay = isonlinepay;

                    //如果订单金额小于0  直接成功
                    if (isonlinepay && cloudlocationordermodel.Totalprice <= 0)
                    {
                        cloudlocationordermodel.Ispay = 1;
                        cloudlocationordermodel.Paytime = DateTime.UtcNow;
                        cloudlocationordermodel.Completetime = DateTime.UtcNow;
                    }

                    if (!string.IsNullOrEmpty(scheduledtime))
                    {
                        cloudlocationordermodel.Scheduledtime = Convert.ToDateTime(scheduledtime).AddHours(-centerhotel.UtcTimeNum); //转换成utc时间并 保存
                        cloudlocationordermodel.Scheduledcompletetime = cloudlocationordermodel.Scheduledtime;
                    }

                    //now占用的时间段
                    if (!string.IsNullOrEmpty(nowscheduledtime))
                    {
                        //说明是now订单里面 有预计时间
                        cloudlocationordermodel.Scheduledtime = Convert.ToDateTime(nowscheduledtime).AddHours(-centerhotel.UtcTimeNum); //转换成utc时间并 保存
                                                                                                                                        //cloudlocationordermodel.Scheduledcompletetime = cloudlocationordermodel.Scheduledtime;
                    }
                    if (!string.IsNullOrEmpty(nowlatertime))
                    {
                        cloudlocationordermodel.Nowscheduledtime = Convert.ToDateTime(nowlatertime).AddHours(-centerhotel.UtcTimeNum); //转换成utc时间并 保存
                    }

                    //说明是pickup的now订单，需要添加计划完成时间
                    if (!string.IsNullOrEmpty(ordertype) && ordertype == "pickup" && !string.IsNullOrEmpty(nowlatertime))
                    {
                        cloudlocationordermodel.Scheduledcompletetime = Convert.ToDateTime(nowlatertime).AddHours(-centerhotel.UtcTimeNum); //转换成utc时间并 保存
                                                                                                                                            //cloudlocationordermodel.Scheduledcompletetime = DateTime.UtcNow.AddMinutes(int.Parse(schedulemealminute));
                    }
                    //说明是delivery的now订单，需要添加计划完成时间
                    if (!string.IsNullOrEmpty(ordertype) && ordertype == "delivery" && !string.IsNullOrEmpty(nowlatertime))
                    {
                        cloudlocationordermodel.Scheduledcompletetime = Convert.ToDateTime(nowlatertime).AddHours(-centerhotel.UtcTimeNum); //转换成utc时间并 保存
                                                                                                                                            //cloudlocationordermodel.Scheduledcompletetime = DateTime.UtcNow.AddMinutes(int.Parse(schedulemealminute));
                    }

                    cloudlocationordermodel.Couponcode = couponcode;

                    cloudlocationordermodel.Deliverylocation = cuslocation;
                    cloudlocationordermodel.Deliveryaddress = cuslocationname;
                    cloudlocationordermodel.Cuslocationjson = cuslocationjson;
                    cloudlocationordermodel.Chooseeatinmethod = chooseeatinmethod;

                    cloudlocationordermodel.Pricelevel = null;
                    if (datajson["pricelevel"] != null)
                    {
                        cloudlocationordermodel.Pricelevel = Convert.ToInt32(datajson["pricelevel"]);
                    }
                    if (datajson["mbrcardtypeid"] != null)
                    {
                        cloudlocationordermodel.Mbrcardtypeid = datajson["mbrcardtypeid"]?.ToString();
                    }
                    else
                    {
                        cloudlocationordermodel.Mbrcardtypeid = null;
                    }

                    bool is3rdps = false;
                    string deliveryname = "doordash";


                    if (!string.IsNullOrEmpty(ordertype) && ordertype == "delivery")
                    {
                        //判断一下商户配置是否是doordash配送
                        var pmshotel = pmsContext.PmsHotels.AsNoTracking().FirstOrDefault(x => x.Hid == centerhotel.Hid);
                        if (pmshotel != null && !string.IsNullOrEmpty(pmshotel.Locationdeliveryconfig))
                        {
                            JObject deliveryobj = JObject.Parse(pmshotel.Locationdeliveryconfig);
                            if (deliveryobj != null && deliveryobj["enable3rd"] != null)
                            {
                                if (deliveryobj["enable3rd"]?.ToString() == "doordash")
                                {
                                    deliveryname = "doordash";
                                    if (!ValidationDoorDashOrder(cloudlocationordermodel.Hid, cloudlocationordermodel))
                                    {
                                        return "Sorry, We are unable to process your order due to a system issue.";
                                    }
                                    else
                                    {
                                        is3rdps = true;
                                    }
                                }
                                else
                                {
                                    deliveryname = "foodhwy";
                                    is3rdps = true;
                                }
                            }
                        }
                    }

                    #region 生成订单号和 顺序callnumber
                    string callnumbersql = " declare @callnumber int " +
                                           " declare @hid varchar(10) " +
                                           " declare @ordernumber int " +
                                           " set @hid = '" + centerhotel.Hid + "' " +
                                           " SET NOCOUNT ON; " +
                                           " set transaction isolation level read committed " +
                                           " begin tran " +
                                           " set @callnumber = (select callnumber from cloudorderlockmodel WITH(UPDLOCK) where hid = @hid) " +
                                           " if (@callnumber > 9999) " +
                                           " begin " +
                                           " set @callnumber = 1001 " +
                                           " end " +
                                           " else " +
                                           " begin " +
                                           " set @callnumber = @callnumber + 1 " +
                                           " end " +
                                           " update cloudorderlockmodel set callnumber = @callnumber where hid = @hid " +
                                           " commit tran " +
                                           "" +
                                           " begin tran " +
                                           " set @ordernumber = (select callnumber from cloudorderlockmodel WITH(UPDLOCK) where hid = '000000') " +
                                           " if (@ordernumber >= 999) " +
                                           " begin " +
                                           " set @ordernumber = 100 " +
                                           " end " +
                                           " else " +
                                           " begin " +
                                           " set @ordernumber = @ordernumber + 1 " +
                                           " end " +
                                           " update cloudorderlockmodel set callnumber = @ordernumber where hid = '000000' " +
                                           " commit tran " +
                                           " select @callnumber,@ordernumber ";
                    if ((isonlinepay || ackordermode == "1" || ackordermode == "2") && Convert.ToDecimal(datajson["totalsumprice"]?.ToString()) > 0)
                    {
                        //如果是onlinepay 或者是应答的订单，顺序号不能先占用 修改sql
                        callnumbersql = " declare @callnumber int " +
                                           " declare @hid varchar(10) " +
                                           " declare @ordernumber int " +
                                           " set @hid = '" + centerhotel.Hid + "' " +
                                           " set @callnumber = -1 " +
                                           " SET NOCOUNT ON; " +
                                           " set transaction isolation level read committed " +
                                           " begin tran " +
                                           " set @ordernumber = (select callnumber from cloudorderlockmodel WITH(UPDLOCK) where hid = '000000') " +
                                           " if (@ordernumber >= 999) " +
                                           " begin " +
                                           " set @ordernumber = 100 " +
                                           " end " +
                                           " else " +
                                           " begin " +
                                           " set @ordernumber = @ordernumber + 1 " +
                                           " end " +
                                           " update cloudorderlockmodel set callnumber = @ordernumber where hid = '000000' " +
                                           " commit tran " +
                                           " select @callnumber,@ordernumber ";
                    }
                    string callnumber = "";
                    FormId = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    try
                    {
                        DataTable dtable = ADOHelper.ExecSql(callnumbersql, connstr);
                        if (dtable != null && dtable.Rows.Count > 0)
                        {
                            callnumber = Convert.ToString(dtable.Rows[0][0]);
                            FormId += Convert.ToInt32(dtable.Rows[0][1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        new SysLogService(_db).AddSysLog("[" + FormId + "]CallNumber Error" + ex.ToString(), "CloudLocationOrder-POS-Error", "", "", "");
                        return "The network is abnormal, please re-order";
                    }
                    #endregion
                    cloudlocationordermodel.Formid = FormId;
                    cloudlocationordermodel.Callnumber = callnumber;

                    if (is3rdps)
                    {
                        if (deliveryname == "doordash")
                        {
                            cloudlocationordermodel.Doordashdeliveryid = FormId;
                        }
                        else
                        {
                            cloudlocationordermodel.Foodhwydeliveryid = FormId;
                        }
                    }


                    decimal caltotal = cloudlocationordermodel.Totalprice;

                    //用户余额
                    MbrCardBalance profilebalancemodel = null;
                    if (!string.IsNullOrEmpty(profileid))
                    {
                        profilebalancemodel = pmsContext.MbrCardBalances.SingleOrDefault(x => x.profileId == profileguid);



                        if (!profilebalancemodel.Freezebalance.HasValue)
                        {
                            profilebalancemodel.Freezebalance = 0;
                        }
                        if (!profilebalancemodel.Freezebonus.HasValue)
                        {
                            profilebalancemodel.Freezebonus = 0;
                        }
                    }

                    if (cloudlocationordermodel.Cartpoints.HasValue && cloudlocationordermodel.Cartpoints.Value > 0 && profilebalancemodel == null)
                    {
                        return "Please login first";
                    }



                    if ((ackordermode == "1" || ackordermode == "2") && (paymentmodel == null || !paymentmodel.Isonlinepay || cloudlocationordermodel.Totalprice <= 0))
                    {
                        cloudlocationordermodel.Ispay = 1;


                        JObject missobj = JObject.Parse(GetAckNewMissTime(cloudlocationordermodel.Hid));
                        if (Convert.ToBoolean(missobj["isinbusinesstime"]))
                        {
                            cloudlocationordermodel.Isackmissforlater = false;
                        }
                        else
                        {
                            cloudlocationordermodel.Isackmissforlater = true;
                        }
                        cloudlocationordermodel.Ackmisstime = Convert.ToDateTime(missobj["misstime"]);


                    }


                    //如果订单里有使用积分，开始扣除
                    if (cloudlocationordermodel.Cartpoints.HasValue && cloudlocationordermodel.Cartpoints.Value > 0 && profilebalancemodel != null)
                    {
                        //判断一下冻结的积分
                        if (!profilebalancemodel.Freezescore.HasValue)
                        {
                            profilebalancemodel.Freezescore = 0;
                        }
                        if (cloudlocationordermodel.Cartpoints.Value > profilebalancemodel.Score)
                        {
                            return "Sorry, you don't have enough points to redeem the selected item. please remove the selected redeem items before proceed.";
                        }

                        if ((profilebalancemodel.Score - profilebalancemodel.Freezescore) < cloudlocationordermodel.Cartpoints.Value)
                        {
                            return "Sorry, you have " + profilebalancemodel.Freezescore + " points frozen, can not complete the payment.";
                        }
                        //如果不是手动应答订单、在线支付这些，或者金额不需要支付，直接扣积分就行了
                        if ((ackordermode == "0") && (paymentmodel == null || !paymentmodel.Isonlinepay || cloudlocationordermodel.Totalprice <= 0))
                        {
                            Onlineorder olorder = new Onlineorder();
                            olorder.Orderid = Guid.NewGuid();
                            olorder.Formid = cloudlocationordermodel.Formid;//+ "6";
                            olorder.Offineorderid = "";
                            olorder.Globalorderid = "";
                            olorder.Hid = cloudlocationordermodel.Hid;
                            olorder.Profileid = cloudlocationordermodel.Profileid.ToString();
                            olorder.Transationtype = 4;
                            olorder.Totalamount = 0;
                            olorder.Toamount = 0;
                            olorder.Transationamount = 0;
                            olorder.Transationbonus = 0;
                            olorder.Transationpoints = cloudlocationordermodel.Cartpoints.Value;
                            olorder.Policyid = "";
                            olorder.Status = 6;
                            olorder.Ispay = 1;
                            olorder.Iscomplete = 1;
                            olorder.Ordertype = "OnlineInterface";

                            DateTime window_transation_time = DateTime.UtcNow;

                            olorder.Paytime = window_transation_time;
                            olorder.Completetime = window_transation_time;
                            olorder.AddDate = window_transation_time;
                            olorder.Otherid = "";
                            olorder.Currency = "";

                            olorder.Poscode = null;
                            olorder.Posdesc = null;
                            olorder.Notes = "Points Redeem (" + ((string.IsNullOrWhiteSpace(ordertype) || ordertype != "kiosk") ? "Online" : "Kiosk") + " " + cloudlocationordermodel.Callnumber + ")";
                            olorder.Grpid = grpid;
                            olorder.Source = "";

                            olorder.trangiftcardstatus = null;
                            olorder.trangiftcardmembertypeid = null;
                            olorder.trangiftcardexpires = null;
                            olorder.Couponid = null;
                            olorder.otherdata = "";

                            profilebalancemodel.Score -= cloudlocationordermodel.Cartpoints.Value;
                            pmsContext.Entry(profilebalancemodel).Property(x => x.Score).IsModified = true;

                            _db.Onlineorder.Add(olorder);
                        }
                        else
                        {
                            //如果是在线支付
                            profilebalancemodel.Freezescore += cloudlocationordermodel.Cartpoints.Value;

                            //修改为积分先冻结，导入中间授权表，支付成功后再根据授权表扣积分 S ---

                            Cloudlocationorderfreezemodel freezemodel = new Cloudlocationorderfreezemodel();
                            freezemodel.Id = Guid.NewGuid();
                            freezemodel.Profileid = cloudlocationordermodel.Profileid.Value;
                            freezemodel.Formid = cloudlocationordermodel.Formid;
                            freezemodel.Balance = 0;
                            freezemodel.Bonus = 0;
                            freezemodel.Points = cloudlocationordermodel.Cartpoints.Value;
                            freezemodel.status = 1;
                            freezemodel.Lastupdatetime = DateTime.UtcNow;
                            freezemodel.Notes = "Points Redeem (Online " + cloudlocationordermodel.Callnumber + ")";

                            pmsContext.Cloudlocationorderfreezemodel.Add(freezemodel);
                            pmsContext.Entry(profilebalancemodel).Property(x => x.Freezescore).IsModified = true;
                        }
                    }



                    #region 20210715新增 paymentlist

                    List<Onlineorder> odorderlist = new List<Onlineorder>();

                    List<Cloudlocationorderfreezemodel> emoneyfreezemodellist = new List<Cloudlocationorderfreezemodel>(); //被冻结的eMoney

                    List<Cloudlocationorderpayment> orderpaymentlist = null;

                    List<Cloudlocationordercart> cartmodellist = new List<Cloudlocationordercart>();
                    List<Cloudlocationordercartmodifier> allmodifierlist = new List<Cloudlocationordercartmodifier>();
                    List<Cloudlocationordercartcomboexchange> allexchangelist = new List<Cloudlocationordercartcomboexchange>();
                    List<Cloudlocationordercartcomboexchangemodiferlist> allexchangemodiferlist = new List<Cloudlocationordercartcomboexchangemodiferlist>();
                    List<Cloudlocationorderbilldiscount> discountlist = new List<Cloudlocationorderbilldiscount>();

                    string paymethodlistname = "";
                    if (!string.IsNullOrEmpty(paymentlist))
                    {
                        orderpaymentlist = new List<Cloudlocationorderpayment>();

                        JArray paymentjarray = JArray.Parse(paymentlist);

                        JObject eMoneyObj = null;
                        JObject pointsObj = null;

                        for (int i = 0; i < paymentjarray.Count; i++)
                        {
                            if (Guid.Parse(paymentjarray[i]["id"].ToString()) == Guid.Parse("bb93f7e4-10d2-461e-9157-0fa028417b6a"))
                            {
                                eMoneyObj = new JObject();
                                eMoneyObj = (JObject)paymentjarray[i];
                                paymentjarray.RemoveAt(i);
                                i--;
                                continue;
                            }
                            if (Guid.Parse(paymentjarray[i]["id"]?.ToString()) == Guid.Parse("6acd71df-c9f3-4d15-8598-0743a1e1f478"))
                            {
                                pointsObj = new JObject();
                                pointsObj = (JObject)paymentjarray[i];
                                paymentjarray.RemoveAt(i);
                                i--;
                                continue;
                            }
                        }
                        if (eMoneyObj != null)
                        {
                            paymentjarray.Insert(0, eMoneyObj);
                        }
                        if (pointsObj != null)
                        {
                            paymentjarray.Insert(0, pointsObj);
                        }


                        foreach (JObject item in paymentjarray)
                        {
                            Cloudlocationorderpayment orderpaymentmodel = new Cloudlocationorderpayment();
                            orderpaymentmodel.Id = Guid.NewGuid();
                            orderpaymentmodel.Formid = FormId;
                            orderpaymentmodel.Payid = Guid.Parse(item["id"]?.ToString());
                            orderpaymentmodel.Paytypeid = Guid.Parse(item["typeid"]?.ToString());
                            orderpaymentmodel.Payname = item["config"]["name"]?.ToString();
                            paymethodlistname += orderpaymentmodel.Payname + ",";

                            orderpaymentmodel.Paybalance = 0;
                            orderpaymentmodel.Paybonus = 0;
                            orderpaymentmodel.Paypoints = 0;
                            orderpaymentmodel.Createdate = DateTime.UtcNow;
                            orderpaymentmodel.Hid = hid;

                            //如果是金额兑换
                            if (orderpaymentmodel.Payid == Guid.Parse("bb93f7e4-10d2-461e-9157-0fa028417b6a"))
                            {
                                if ((profilebalancemodel.Free - profilebalancemodel.Freezebonus) >= caltotal)
                                {
                                    orderpaymentmodel.Paybonus += caltotal;
                                    profilebalancemodel.Free = profilebalancemodel.Free - caltotal;
                                    caltotal = 0;
                                }
                                else
                                {
                                    orderpaymentmodel.Paybonus += (decimal)(profilebalancemodel.Free - profilebalancemodel.Freezebonus);

                                    caltotal = caltotal - (decimal)(profilebalancemodel.Free - profilebalancemodel.Freezebonus);

                                    profilebalancemodel.Free = profilebalancemodel.Freezebonus;
                                }

                                //pmsContext.Entry(profilebalancemodel).Property(x => x.Free).IsModified = true;

                                if (caltotal > 0)
                                {
                                    if ((profilebalancemodel.Balance - profilebalancemodel.Freezebalance) >= caltotal)
                                    {
                                        orderpaymentmodel.Paybalance += caltotal;

                                        profilebalancemodel.Balance = profilebalancemodel.Balance - caltotal;
                                        caltotal = 0;
                                    }
                                    else
                                    {
                                        orderpaymentmodel.Paybalance += (decimal)(profilebalancemodel.Balance - profilebalancemodel.Freezebalance);

                                        caltotal = caltotal - (decimal)(profilebalancemodel.Balance - profilebalancemodel.Freezebalance);

                                        profilebalancemodel.Balance = profilebalancemodel.Freezebalance;
                                    }
                                    //pmsContext.Entry(profilebalancemodel).Property(x => x.Balance).IsModified = true;
                                }

                                if (orderpaymentmodel.Paybalance > 0 || orderpaymentmodel.Paybonus > 0)
                                {
                                    //如果不是应答订单、或者在线支付 直接扣款
                                    if ((ackordermode == "0") && (paymentmodel == null || !paymentmodel.Isonlinepay))
                                    {
                                        Onlineorder olorder = new Onlineorder();
                                        olorder.Orderid = Guid.NewGuid();
                                        olorder.Formid = cloudlocationordermodel.Formid + "2";
                                        olorder.Offineorderid = "";
                                        olorder.Globalorderid = "";
                                        olorder.Hid = cloudlocationordermodel.Hid;
                                        olorder.Profileid = cloudlocationordermodel.Profileid.ToString();
                                        olorder.Transationtype = 2;
                                        olorder.Totalamount = orderpaymentmodel.Paybalance;
                                        olorder.Toamount = orderpaymentmodel.Paybonus;
                                        olorder.Transationamount = orderpaymentmodel.Paybalance;
                                        olorder.Transationbonus = orderpaymentmodel.Paybonus;
                                        olorder.Transationpoints = 0;
                                        olorder.Policyid = "";
                                        olorder.Status = 6;
                                        olorder.Ispay = 1;
                                        olorder.Iscomplete = 1;
                                        olorder.Ordertype = "OnlineInterface";

                                        DateTime window_transation_time = DateTime.UtcNow;

                                        olorder.Paytime = window_transation_time;
                                        olorder.Completetime = window_transation_time;
                                        olorder.AddDate = window_transation_time;
                                        olorder.Otherid = "";
                                        olorder.Currency = "";

                                        olorder.Poscode = null;
                                        olorder.Posdesc = null;
                                        olorder.Notes = "eMoney Online Pay (Online " + cloudlocationordermodel.Callnumber + ")";
                                        olorder.Grpid = grpid;
                                        olorder.Source = "";

                                        olorder.trangiftcardstatus = null;
                                        olorder.trangiftcardmembertypeid = null;
                                        olorder.trangiftcardexpires = null;
                                        olorder.Couponid = null;
                                        olorder.otherdata = "";



                                        pmsContext.Entry(profilebalancemodel).Property(x => x.Balance).IsModified = true;
                                        pmsContext.Entry(profilebalancemodel).Property(x => x.Free).IsModified = true;


                                        odorderlist.Add(olorder);
                                    }
                                    else
                                    {
                                        //修改为eMoney先冻结，导入中间授权表，支付成功后再根据授权表扣eMoney S ---

                                        Cloudlocationorderfreezemodel freezemodel = new Cloudlocationorderfreezemodel();
                                        freezemodel.Id = Guid.NewGuid();
                                        freezemodel.Profileid = cloudlocationordermodel.Profileid.Value;
                                        freezemodel.Formid = cloudlocationordermodel.Formid;
                                        freezemodel.Balance = orderpaymentmodel.Paybalance;
                                        freezemodel.Bonus = orderpaymentmodel.Paybonus;
                                        freezemodel.Points = 0;
                                        freezemodel.status = 1;
                                        freezemodel.Lastupdatetime = DateTime.UtcNow;
                                        freezemodel.Notes = "eMoney Online Pay (Online " + cloudlocationordermodel.Callnumber + ")";

                                        pmsContext.Cloudlocationorderfreezemodel.Add(freezemodel);

                                        profilebalancemodel.Freezebalance += orderpaymentmodel.Paybalance;
                                        profilebalancemodel.Freezebonus += orderpaymentmodel.Paybonus;

                                        pmsContext.Entry(profilebalancemodel).Property(x => x.Freezebalance).IsModified = true;
                                        pmsContext.Entry(profilebalancemodel).Property(x => x.Freezebonus).IsModified = true;

                                        emoneyfreezemodellist.Add(freezemodel);
                                    }

                                }
                            }
                            else if (orderpaymentmodel.Payid == Guid.Parse("6acd71df-c9f3-4d15-8598-0743a1e1f478"))
                            {
                                //如果是积分兑换
                                decimal redeememoney = Convert.ToDecimal(item["redeememoney"]);
                                orderpaymentmodel.Paypoints = Convert.ToDecimal(item["redeempoints"]);

                                if (orderpaymentmodel.Paypoints <= 0)
                                {
                                    continue;
                                }

                                if (paymentmodel == null || !paymentmodel.Isonlinepay)
                                {
                                    Onlineorder olorder = new Onlineorder();
                                    olorder.Orderid = Guid.NewGuid();
                                    olorder.Formid = cloudlocationordermodel.Formid + "1";
                                    olorder.Offineorderid = "";
                                    olorder.Globalorderid = "";
                                    olorder.Hid = cloudlocationordermodel.Hid;
                                    olorder.Profileid = cloudlocationordermodel.Profileid.ToString();
                                    olorder.Transationtype = 4;
                                    olorder.Totalamount = 0;
                                    olorder.Toamount = 0;
                                    olorder.Transationamount = 0;
                                    olorder.Transationbonus = 0;
                                    olorder.Transationpoints = orderpaymentmodel.Paypoints;
                                    olorder.Policyid = "";
                                    olorder.Status = 6;
                                    olorder.Ispay = 1;
                                    olorder.Iscomplete = 1;
                                    olorder.Ordertype = "OnlineInterface";

                                    DateTime window_transation_time = DateTime.UtcNow;

                                    olorder.Paytime = window_transation_time;
                                    olorder.Completetime = window_transation_time;
                                    olorder.AddDate = window_transation_time;
                                    olorder.Otherid = "";
                                    olorder.Currency = "";

                                    olorder.Poscode = null;
                                    olorder.Posdesc = null;
                                    olorder.Notes = "Points Redeem eMoney $" + redeememoney + " (Online " + cloudlocationordermodel.Formid + ")";
                                    olorder.Grpid = grpid;
                                    olorder.Source = "";

                                    olorder.trangiftcardstatus = null;
                                    olorder.trangiftcardmembertypeid = null;
                                    olorder.trangiftcardexpires = null;
                                    olorder.Couponid = null;
                                    olorder.otherdata = "";

                                    odorderlist.Add(olorder);


                                    orderpaymentmodel.Paybonus = 0;

                                    if (redeememoney >= caltotal)
                                    {
                                        redeememoney -= caltotal;
                                        orderpaymentmodel.Paybonus = caltotal;
                                        caltotal = 0;

                                        orderpaymentmodel.Excessamount = redeememoney;
                                    }
                                    else
                                    {
                                        orderpaymentmodel.Paybonus = redeememoney;
                                        caltotal = caltotal - redeememoney;
                                        redeememoney = 0;
                                    }

                                    if (orderpaymentmodel.Paypoints < 0 || orderpaymentmodel.Paypoints > profilebalancemodel.Score)
                                    {
                                        return "Not enough points";
                                    }

                                    profilebalancemodel.Score -= orderpaymentmodel.Paypoints;
                                    pmsContext.Entry(profilebalancemodel).Property(x => x.Score).IsModified = true;

                                    //如果积分兑换eMoney后 还有多余的钱，并且订单不需要支付其他款项，直接充值到账户，否则在线支付回调里才充值
                                    if (redeememoney > 0)
                                    {
                                        Onlineorder olineorder = new Onlineorder();
                                        olineorder.Orderid = Guid.NewGuid();
                                        olineorder.Formid = cloudlocationordermodel.Formid + "1";
                                        olineorder.Offineorderid = "";
                                        olineorder.Globalorderid = "";
                                        olineorder.Hid = cloudlocationordermodel.Hid;
                                        olineorder.Profileid = cloudlocationordermodel.Profileid.ToString();
                                        olineorder.Transationtype = 1;
                                        olineorder.Totalamount = 0;
                                        olineorder.Toamount = redeememoney;
                                        olineorder.Transationamount = 0;
                                        olineorder.Transationbonus = redeememoney;
                                        olineorder.Transationpoints = 0;
                                        olineorder.Policyid = "";
                                        olineorder.Status = 6;
                                        olineorder.Ispay = 1;
                                        olineorder.Iscomplete = 1;
                                        olineorder.Ordertype = "OnlineInterface";


                                        olineorder.Paytime = window_transation_time;
                                        olineorder.Completetime = window_transation_time;
                                        olineorder.AddDate = window_transation_time;
                                        olineorder.Otherid = "";
                                        olineorder.Currency = "";

                                        olineorder.Poscode = null;
                                        olineorder.Posdesc = null;
                                        olineorder.Notes = "Points Redeem eMoney(Remain)(Online " + cloudlocationordermodel.Callnumber + ")";
                                        olineorder.Grpid = grpid;
                                        olineorder.Source = "";

                                        olineorder.trangiftcardstatus = null;
                                        olineorder.trangiftcardmembertypeid = null;
                                        olineorder.trangiftcardexpires = null;
                                        olineorder.Couponid = null;
                                        olineorder.otherdata = "";

                                        odorderlist.Add(olineorder);
                                        if (paymentmodel == null || !paymentmodel.Isonlinepay || cloudlocationordermodel.Totalprice <= 0 || caltotal <= 0)
                                        {
                                            profilebalancemodel.Free += redeememoney;
                                            pmsContext.Entry(profilebalancemodel).Property(x => x.Free).IsModified = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                orderpaymentmodel.Paybalance = caltotal;
                                caltotal = 0;
                            }
                            orderpaymentlist.Add(orderpaymentmodel);
                            if (caltotal <= 0)
                            {
                                break;
                            }
                        }

                        if (paymentjarray != null && paymentjarray.Count > 0)
                        {
                            paymethodlistname = paymethodlistname.TrimEnd(',');

                            if (!string.IsNullOrEmpty(paymethodlistname))
                            {
                                cloudlocationordermodel.Paymethod = paymethodlistname;
                            }

                            if (caltotal != 0)
                            {
                                return null;
                            }
                        }
                    }

                    //如果是GlobalPayment的在线支付(非HPP)订单， 不更新对应的字段
                    if (profilebalancemodel != null && paymentmodel != null && !string.IsNullOrEmpty(paytypeid) && paymentmodel.Isonlinepay && Guid.Parse(paytypeid) == Guid.Parse("1800EBF9-D169-4D88-AD09-E29742F8B299") && cloudlocationordermodel.Totalprice > 0)
                    {
                        pmsContext.Entry(profilebalancemodel).Property(x => x.Balance).IsModified = false;
                        pmsContext.Entry(profilebalancemodel).Property(x => x.Free).IsModified = false;
                        pmsContext.Entry(profilebalancemodel).Property(x => x.Score).IsModified = false;

                        cloudlocationordermodel.Issuedtransaction = 1;
                    }
                    else
                    {
                        cloudlocationordermodel.Issuedtransaction = 1;
                        _db.Onlineorder.AddRange(odorderlist);
                    }


                    #region 如果是不需要在线支付，需要把sendcomplete也一起赋值 避免特殊情况时系统定义的自动重发订单也不生效问题：事务保存了订单，但是因为发布等原因导致程序中断，没有去请求pos的webhook 但是也不出现错误

                    //如果不是应答订单
                    if (!cloudlocationordermodel.Isackorder.HasValue || !cloudlocationordermodel.Isackorder.Value)
                    {
                        //如果不是在线支付才直接发订单到pos
                        if (paymentmodel == null || !paymentmodel.Isonlinepay || cloudlocationordermodel.Totalprice <= 0)
                        {
                            if (!string.IsNullOrWhiteSpace(ordertype) && ordertype == "kiosk")
                            {
                                //20230324 如果是kiosk 这里不用处理
                            }
                            else
                            {
                                cloudlocationordermodel.Issendposcomplete = false;
                            }
                        }
                    }
                    #endregion


                    pmsContext.Cloudlocationorder.Add(cloudlocationordermodel);

                    if (orderpaymentlist != null && orderpaymentlist.Count > 0)
                    {
                        pmsContext.Cloudlocationorderpayment.AddRange(orderpaymentlist);
                    }
                    #endregion



                    if (datajson["billdarray"] != null)
                    {
                        try
                        {
                            JArray billdarray = JArray.FromObject(datajson["billdarray"]);

                            int billcalindex = 0; //从小到大计算的顺序，发给pos用
                            foreach (JObject billitem in billdarray)
                            {
                                Cloudlocationorderbilldiscount billdiscountmodel = new Cloudlocationorderbilldiscount();
                                billdiscountmodel.Id = Guid.NewGuid();
                                billdiscountmodel.Hid = cloudlocationordermodel.Hid;
                                billdiscountmodel.Formid = cloudlocationordermodel.Formid;
                                billdiscountmodel.Saleid = Guid.Parse(billitem["saleid"]?.ToString());
                                billdiscountmodel.Salename = billitem["salename"]?.ToString();
                                billdiscountmodel.Salediscount = Convert.ToDecimal(billitem["salediscount"]);
                                billdiscountmodel.Salediscountrate = Convert.ToDecimal(billitem["salediscountrate"]);
                                billdiscountmodel.Createdate = DateTime.UtcNow;
                                billdiscountmodel.Billcalindex = billcalindex;

                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(billitem["ismaxdiscountmoney"]?.ToString()))
                                    {
                                        billdiscountmodel.Ismaxdiscountmoney = Convert.ToBoolean(billitem["ismaxdiscountmoney"]);
                                    }
                                }
                                catch (Exception) { }

                                billcalindex++;

                                if (billitem["ismembercoupon"] != null && !string.IsNullOrWhiteSpace(billitem["ismembercoupon"]?.ToString()))
                                {
                                    billdiscountmodel.Ismembercoupon = Convert.ToBoolean(billitem["ismembercoupon"]);
                                    //这里给优惠券增加交易记录
                                    var mrusercoupon = pmsContext.Mrusercoupon.FirstOrDefault(x => x.Id == billdiscountmodel.Saleid && x.Profileid == profileguid);
                                    if (mrusercoupon != null)
                                    {
                                        mrusercoupon.Status = 2;
                                        pmsContext.Entry(mrusercoupon).Property(x => x.Status).IsModified = true;

                                        Mrusercoupontransaction transaction = new Mrusercoupontransaction();
                                        transaction.Id = Guid.NewGuid();
                                        transaction.Hid = cloudlocationordermodel.Hid;
                                        transaction.Couponid = mrusercoupon.Id;
                                        transaction.Status = 2;
                                        transaction.Profileid = mrusercoupon.Profileid;
                                        transaction.Adddate = DateTime.UtcNow;
                                        transaction.Notes = "";
                                        transaction.Onlineorderformid = cloudlocationordermodel.Formid;

                                        pmsContext.Mrusercoupontransaction.Add(transaction);
                                    }
                                }

                                discountlist.Add(billdiscountmodel);
                                pmsContext.Cloudlocationorderbilldiscount.Add(billdiscountmodel);
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }

                    JArray cartarray = JArray.FromObject(datajson["cartinfo"]);

                    int cartindex = 0; //Cloudlocationordercart index的排序号
                    foreach (JObject item in cartarray)
                    {
                        Cloudlocationordercart cartmodel = new Cloudlocationordercart();
                        cartmodel.Id = Guid.NewGuid();
                        cartmodel.Formid = FormId;
                        cartmodel.Itemcode = item["itemcode"]?.ToString();
                        cartmodel.Classcode = item["classcode"]?.ToString();
                        cartmodel.Namedic = item["ordernamedic"]?.ToString();
                        cartmodel.Qty = Convert.ToInt32(item["qty"]?.ToString());
                        cartmodel.Itemimgurl = item["itemimgurl"]?.ToString();
                        cartmodel.Itemprice = Convert.ToDecimal(item["itemunitprice"]);
                        cartmodel.Itemsumprice = Convert.ToDecimal(item["itemprice"]);
                        cartmodel.Sizecode = item["sizecode"]?.ToString();
                        cartmodel.Sizenamedic = item["ordersizenamedic"]?.ToString();
                        cartmodel.Taxid = Convert.ToInt32(item["taxid"] == null ? 0 : item["taxid"]);
                        cartmodel.Taxrate = Convert.ToDecimal(item["taxrate"] == null ? 0 : item["taxrate"]);
                        cartmodel.Taxprice = Convert.ToDecimal(item["taxprice"] == null ? 0 : item["taxprice"]);

                        cartmodel.Menuid = Convert.ToInt32(item["menuid"] == null ? 0 : item["menuid"]);
                        cartmodel.Menuclasscode = item["menuclasscode"]?.ToString();

                        //2021-05-26 S 促销推广字段
                        if (item["saleid"] != null && item["saleid"].ToString().Trim() != "")
                        {
                            cartmodel.Saleid = Guid.Parse(item["saleid"].ToString().Trim());
                            cartmodel.Salename = item["salename"]?.ToString();
                            cartmodel.Salediscount = Math.Round(Convert.ToDecimal(item["salediscount"]), 2, MidpointRounding.AwayFromZero);
                            cartmodel.Olditemprice = Convert.ToDecimal(item["olditemprice"]);
                            cartmodel.Olditemsumprice = Convert.ToDecimal(item["olditemsumprice"]);
                            if (item["discountrate"] != null)
                            {
                                cartmodel.Salediscountrate = Convert.ToDecimal(item["discountrate"]);
                            }
                        }
                        //2021-05-26 E


                        cartmodel.Hid = hid;
                        cartmodel.Createdate = DateTime.UtcNow;
                        cartmodel.Cartseqid = cartindex;
                        if (item["special"] != null)
                        {
                            cartmodel.Special = item["special"]?.ToString();
                        }
                        if (item["isredeempoint"] != null)
                        {
                            cartmodel.Isredeempoint = Convert.ToBoolean(item["isredeempoint"]);
                        }
                        if (item["redeemsumpoints"] != null)
                        {
                            cartmodel.Redeemsumpoints = Convert.ToInt32(item["redeemsumpoints"]);
                        }
                        if (item["redeemunitpoints"] != null)
                        {
                            cartmodel.Redeemunitpoints = Convert.ToInt32(item["redeemunitpoints"]);
                        }

                        if (!string.IsNullOrWhiteSpace(item["salejoincalid"]?.ToString()))
                        {
                            cartmodel.Salejoincalid = Guid.Parse(item["salejoincalid"]?.ToString());
                        }
                        if (item["salejoincalname"] != null)
                        {
                            cartmodel.Salejoincalname = item["salejoincalname"]?.ToString();
                        }
                        if (!string.IsNullOrWhiteSpace(item["newdiscount"]?.ToString()))
                        {
                            //cartmodel.Salejoincalnewdiscount = Math.Round(Convert.ToDecimal(item["newdiscount"]), 2);

                            cartmodel.Salejoincalnewdiscount = Math.Round(Convert.ToDecimal(item["newdiscount"]), 2, MidpointRounding.AwayFromZero);
                        }

                        if (!string.IsNullOrWhiteSpace(item["salejoincalrate"]?.ToString()))
                        {
                            cartmodel.Salejoincalrate = Convert.ToDecimal(item["salejoincalrate"]);
                        }
                        if (!string.IsNullOrWhiteSpace(item["ismembercoupon"]?.ToString()))
                        {
                            cartmodel.Ismembercoupon = Convert.ToBoolean(item["ismembercoupon"]);

                            //这里给优惠券增加交易记录
                            var mrusercoupon = pmsContext.Mrusercoupon.FirstOrDefault(x => x.Id == cartmodel.Salejoincalid && x.Profileid == profileguid);
                            if (mrusercoupon != null)
                            {
                                mrusercoupon.Status = 2;
                                pmsContext.Entry(mrusercoupon).Property(x => x.Status).IsModified = true;

                                Mrusercoupontransaction transaction = new Mrusercoupontransaction();
                                transaction.Id = Guid.NewGuid();
                                transaction.Hid = cartmodel.Hid;
                                transaction.Couponid = mrusercoupon.Id;
                                transaction.Status = 2;
                                transaction.Profileid = mrusercoupon.Profileid;
                                transaction.Adddate = DateTime.UtcNow;
                                transaction.Notes = "";
                                transaction.Onlineorderformid = cloudlocationordermodel.Formid;

                                pmsContext.Mrusercoupontransaction.Add(transaction);
                            }

                        }


                        pmsContext.Cloudlocationordercart.Add(cartmodel);
                        cartmodellist.Add(cartmodel);

                        cartindex++;

                        if (item["modgroupcodelist"] != null)
                        {
                            JArray modgrouparray = JArray.FromObject(item["modgroupcodelist"]);


                            int modifierindex = 0; //Cloudlocationordercartmodifier index的排序号
                            foreach (JObject modgroupitem in modgrouparray)
                            {
                                if (modgroupitem["selectcodelist"] != null)
                                {
                                    JArray modifierarray = JArray.FromObject(modgroupitem["selectcodelist"]);
                                    foreach (JObject modifieritem in modifierarray)
                                    {
                                        Cloudlocationordercartmodifier modifier = new Cloudlocationordercartmodifier();
                                        modifier.Id = Guid.NewGuid();
                                        modifier.Formid = FormId;
                                        modifier.Modgroupcode = modgroupitem["modgroupcode"]?.ToString();
                                        modifier.Modgroupnamedic = modgroupitem["ordernamedic"]?.ToString();
                                        modifier.Modifiercode = modifieritem["modifiercode"]?.ToString();
                                        modifier.Modgroupnamedic = modgroupitem["ordernamedic"]?.ToString();
                                        modifier.Modifierqty = Convert.ToInt32(modifieritem["qty"]);
                                        modifier.Modifierprice = Convert.ToDecimal(modifieritem["modifierprice"]);
                                        modifier.Createdate = DateTime.UtcNow;
                                        modifier.Modifiernamedic = modifieritem["ordernamedic"]?.ToString();
                                        modifier.Hid = hid;
                                        modifier.Cartid = cartmodel.Id;
                                        modifier.Cartseqid = modifierindex;
                                        pmsContext.Cloudlocationordercartmodifier.Add(modifier);

                                        allmodifierlist.Add(modifier);

                                        modifierindex++;
                                    }
                                }
                            }
                        }

                        if (item["combolist"] != null)
                        {
                            int comboseqindex = 0; //Cloudlocationordercartcomboexchange index的排序号
                            JArray comboarray = JArray.FromObject(item["combolist"]);
                            foreach (JObject comitem in comboarray)
                            {
                                if (comitem["exchangelist"] != null)
                                {
                                    JArray exchangearray = JArray.FromObject(comitem["exchangelist"]);
                                    foreach (JObject exchangeitem in exchangearray)
                                    {

                                        Cloudlocationordercartcomboexchange exchange = new Cloudlocationordercartcomboexchange();
                                        exchange.Id = Guid.NewGuid();
                                        exchange.Formid = FormId;
                                        exchange.Combonamedic = comitem["ordernamedic"]?.ToString();
                                        exchange.Exitemcode = exchangeitem["itemcode"]?.ToString();
                                        exchange.Exitemqty = Convert.ToInt32(exchangeitem["qty"]);
                                        exchange.Exitemsumprice = Convert.ToDecimal(exchangeitem["exitemsumprice"]);
                                        exchange.Sizecode = exchangeitem["sizecode"]?.ToString();
                                        exchange.Sizenamedic = exchangeitem["ordersizedic"]?.ToString();
                                        exchange.Createdate = DateTime.UtcNow;
                                        exchange.Hid = hid;
                                        exchange.comboindex = int.Parse(comitem["comboindex"]?.ToString());
                                        exchange.Cartid = cartmodel.Id;
                                        exchange.Exitemnamedic = exchangeitem["ordernamedic"]?.ToString();
                                        exchange.Itemprice = 0;

                                        exchange.Cartseqid = comboseqindex;

                                        if (exchangeitem["price"] != null)
                                        {
                                            exchange.Itemprice = Convert.ToDecimal(exchangeitem["price"]);
                                        }

                                        if (exchangeitem["modlist"] != null)
                                        {
                                            int modiferseqindex = 0; //Cloudlocationordercartcomboexchangemodiferlist index的排序号
                                            JArray modlist = JArray.FromObject(exchangeitem["modlist"]);
                                            foreach (JObject moditem in modlist)
                                            {

                                                if (moditem["selectcodelist"] != null)
                                                {
                                                    JArray modifierarray = JArray.FromObject(moditem["selectcodelist"]);
                                                    foreach (JObject modifieritem in modifierarray)
                                                    {
                                                        Cloudlocationordercartcomboexchangemodiferlist modifermodel = new Cloudlocationordercartcomboexchangemodiferlist();
                                                        modifermodel.Id = Guid.NewGuid();
                                                        modifermodel.Hid = hid;
                                                        modifermodel.Formid = FormId;
                                                        modifermodel.Cartid = cartmodel.Id;
                                                        modifermodel.Exchangeid = exchange.Id;
                                                        modifermodel.Modgroupcode = moditem["modgroupcode"]?.ToString();
                                                        modifermodel.Modgroupnamedic = moditem["ordernamedic"]?.ToString();
                                                        modifermodel.Modifiernamedic = modifieritem["ordernamedic"]?.ToString();
                                                        modifermodel.Modifiercode = modifieritem["modifiercode"]?.ToString();
                                                        modifermodel.Modifierqty = Convert.ToInt32(modifieritem["qty"]);
                                                        modifermodel.Modifierprice = Convert.ToDecimal(modifieritem["modifierprice"]);
                                                        modifermodel.Cartseqid = modiferseqindex;

                                                        exchange.Itemprice += modifermodel.Modifierprice * modifermodel.Modifierqty;
                                                        modifermodel.Createdate = DateTime.UtcNow;

                                                        allexchangemodiferlist.Add(modifermodel);
                                                        pmsContext.Cloudlocationordercartcomboexchangemodiferlist.Add(modifermodel);
                                                        modiferseqindex++;
                                                    }
                                                }
                                            }
                                        }
                                        try
                                        {
                                            exchange.Exitemsumprice = (decimal)exchange.Itemprice * exchange.Exitemqty;
                                        }
                                        catch (Exception) { }


                                        allexchangelist.Add(exchange);

                                        pmsContext.Cloudlocationordercartcomboexchange.Add(exchange);


                                        comboseqindex++;
                                    }
                                }
                            }
                        }
                    }
                    try
                    {
                        try
                        {
                            pmsContext.SaveChanges();
                        }
                        catch (DbUpdateException dbexception)
                        {
                            try
                            {
                                new SysLogService(_db).AddSysLog(hid + "|" + dbexception.ToString(), "CloudCommitOrder-Error", "", "", "");
                            }
                            catch (Exception)
                            {

                            }
                        }

                        int count = 1;

                        //这里验证一下random number是否是重复提交的订单
                        if (!string.IsNullOrWhiteSpace(randomnumber))
                        {
                            try
                            {
                                DateTime beforenow = DateTime.UtcNow.AddHours(-1);
                                count = pmsContext.Cloudlocationorder.AsNoTracking().Count(x => x.Randomstr == randomnumber && x.Hid == hid && x.Createdate >= beforenow);
                            }
                            catch (Exception) { }
                        }

                        if (count >= 2)
                        {
                            pmsdbtransaction.Rollback();
                            try
                            {
                                cloudlocationordermodel = pmsContext.Cloudlocationorder.AsNoTracking().Where(x => x.Hid == hid && x.Randomstr == randomnumber).OrderByDescending(x => x.Createdate).FirstOrDefault();
                                FormId = cloudlocationordermodel.Formid;
                                paymentmodel = null;
                                Cloudlocationpayment locationpayment = pmsContext.Cloudlocationpayment.AsNoTracking().FirstOrDefault(x => x.Hid == hid && x.Id == cloudlocationordermodel.Payid && x.Isdelete == 0);
                                if (locationpayment != null)
                                {
                                    paymentmodel = _db.Cloudpayment.FirstOrDefault(x => x.Id == locationpayment.Typeid);
                                    if (paymentmodel != null)
                                    {
                                        paytypeid = paymentmodel.Id.ToString();
                                        isonlinepay = paymentmodel.Isonlinepay;
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            //这里提交事务，开始创建订单
                            pmsdbtransaction.Commit();
                        }
                        if (count < 2)
                        {

                            MbrCard profileinfo = null;
                            bool isexcuterefercampaigns = false; //需要判断是否要执行被推荐人reward campaign

                            //如果不是在线支付，给被推荐人下单操作加一条记录
                            if (paymentmodel == null || !paymentmodel.Isonlinepay || cloudlocationordermodel.Totalprice <= 0)
                            {
                                if (!string.IsNullOrWhiteSpace(profileid))
                                {
                                    try
                                    {
                                        Guid userprofileid = Guid.Parse(profileid);

                                        //查找一下他是否有推荐人
                                        profileinfo = pmsContext.MbrCards.Where(x => x.Id == userprofileid && x.Referrerprofileid.HasValue).FirstOrDefault();
                                        if (profileinfo != null)
                                        {
                                            if (_db.Referralfriendsactionrecord.Count(x => x.Profileid == userprofileid && x.Actiontype == "order") <= 0)
                                            {
                                                Referralfriendsactionrecord record = new Referralfriendsactionrecord();
                                                record.Id = Guid.NewGuid();
                                                record.Hid = grpid;
                                                record.Isuse = false;
                                                record.Createdate = DateTime.UtcNow;
                                                record.Profileid = userprofileid;
                                                record.Referprofileid = profileinfo.Referrerprofileid.Value;
                                                record.Actiontype = "order";
                                                _db.Referralfriendsactionrecord.Add(record);
                                                isexcuterefercampaigns = true;
                                            }
                                        }
                                    }
                                    catch (Exception) { }
                                }
                            }
                            _db.SaveChanges();

                            if (isexcuterefercampaigns && profileinfo != null && profileinfo.Referrerprofileid.HasValue)
                            {
                                //这里处理推荐人
                                new Thread(() =>
                                {
                                    CampaignWebService campaignwebservice = new CampaignWebService();
                                    JObject referparamjson = new JObject();
                                    referparamjson["triggername"] = "referreward";
                                    referparamjson["profileid"] = profileinfo.Referrerprofileid.Value;
                                    referparamjson["hid"] = grpid;
                                    campaignwebservice.ExcuteCampaign(referparamjson.ToString());
                                }).Start();
                            }

                            //如果不用应答该订单，才直接发给pos
                            if (!cloudlocationordermodel.Isackorder.HasValue || !cloudlocationordermodel.Isackorder.Value)
                            {
                                //如果不是在线支付才直接发订单到pos
                                if (paymentmodel == null || !paymentmodel.Isonlinepay || cloudlocationordermodel.Totalprice <= 0)
                                {
                                    //如果是doordash配送订单，发送到doordash api创建一下
                                    if (!string.IsNullOrWhiteSpace(cloudlocationordermodel.Doordashdeliveryid))
                                    {
                                        CreateDoorDashOrder(hid, FormId);
                                    }
                                    if (!string.IsNullOrWhiteSpace(cloudlocationordermodel.Foodhwydeliveryid))
                                    {
                                        CreateFoodHWYDeliveryOrder(hid, FormId);
                                    }
                                    //如果是app下的单
                                    if (profileguid.HasValue && cloudlocationordermodel.Isapp.HasValue && cloudlocationordermodel.Isapp.Value)
                                    {
                                        //这里处理campaigns-onlineorder 下单超过X元 获得奖励
                                        new Thread(() =>
                                        {
                                            CampaignWebService onlineordercampaignwebservice = new CampaignWebService();
                                            JObject referparamjson = new JObject();
                                            referparamjson["triggername"] = "onlineorder";
                                            referparamjson["profileid"] = profileguid.Value;
                                            referparamjson["hid"] = grpid;
                                            referparamjson["formid"] = cloudlocationordermodel.Formid;
                                            onlineordercampaignwebservice.ExcuteCampaign(referparamjson.ToString());
                                        }).Start();
                                    }


                                    if (!string.IsNullOrWhiteSpace(ordertype) && ordertype == "kiosk")
                                    {
                                        //20230324 如果是kiosk 这里不用处理
                                    }
                                    else
                                    {
                                        #region 这里把订单发送给pos 超过3次未接收 发送短信并跳出循环
                                        NotifyPOSByOnlineOrder(centerhotel.Name, hid, FormId, connstr);
                                        #endregion
                                    }

                                    try
                                    {
                                        ADOHelper.ExecNonQuery("if (select isnull(count(*),0) from cloudlocationorder where hid = '" + cloudlocationordermodel.Hid + "' and mobilephone = '" + cloudlocationordermodel.Mobilephone + "'  and isfirstorder = 1) <= 0 begin update cloudlocationorder set isfirstorder = 1 where formid = '" + cloudlocationordermodel.Formid + "' and mobilephone <> '' end", connstr);
                                    }
                                    catch (Exception) { }
                                }
                            }
                            else
                            {
                                #region 如果是免在线支付的订单，这里直接通知到收单程序

                                try
                                {
                                    if (paymentmodel == null || !paymentmodel.Isonlinepay || cloudlocationordermodel.Totalprice <= 0)
                                    {
                                        CallAckSystem(cloudlocationordermodel.Hid);
                                        if (cloudlocationordermodel.Isackmissforlater.HasValue && cloudlocationordermodel.Isackmissforlater.Value)
                                        {
                                            SendEmailByAckorder(FormId, cloudlocationordermodel.Hid);
                                        }
                                        ADOHelper.ExecNonQuery("if (select isnull(count(*),0) from cloudlocationorder where hid = '" + cloudlocationordermodel.Hid + "' and mobilephone = '" + cloudlocationordermodel.Mobilephone + "'  and isfirstorder = 1) <= 0 begin update cloudlocationorder set isfirstorder = 1 where formid = '" + cloudlocationordermodel.Formid + "' and mobilephone <> '' end", connstr);
                                    }
                                }
                                catch (Exception ex) { }
                                #endregion
                            }
                        }

                        JObject forminfo = new JObject();
                        if (!string.IsNullOrEmpty(ordertype) && (ordertype == "pickup" || ordertype == "delivery"))
                        {
                            forminfo["time"] = cloudlocationordermodel.Scheduledcompletetime.Value.AddHours(centerhotel.UtcTimeNum).ToString("yyyy-MM-dd HH:mm");
                            forminfo["formno"] = FormId;
                        }
                        else
                        {
                            forminfo["formno"] = FormId;
                        }

                        if (cloudlocationordermodel.Ackmisstime.HasValue)
                        {
                            forminfo["ackformid"] = FormId;
                            forminfo["ackmisstime"] = cloudlocationordermodel.Ackmisstime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        if(ordertype == "pickup" || ordertype == "delivery")
                        {
                            try
                            {
                                var ackhotel = _db.Hotels.AsNoTracking().SingleOrDefault(x => x.Hid == hid);
                                forminfo["hasackmode"] = true;
                                forminfo["ackordermode"] = ackhotel.Ackordermode.HasValue ? ackhotel.Ackordermode.Value : 0;
                            }
                            catch (Exception)
                            {
                                forminfo["hasackmode"] = false;
                            }
                        }

                        //如果是在线支付 需要传回一个支付地址 （大于0才需要支付）
                        if (paymentmodel != null && paymentmodel.Isonlinepay && cloudlocationordermodel.Totalprice > 0)
                        {
                            string payurl = "";

                            //如果是globalpayment
                            if (paymentmodel.Id == Guid.Parse("1800EBF9-D169-4D88-AD09-E29742F8B299"))
                            {
                                payurl = "pages/user/rechargewebview?hotelid=" + hid + "&formid=" + cloudlocationordermodel.Formid + "&onlineorder=1&ptype=global";
                            }
                            //如果是Stripe
                            if (paymentmodel.Id == Guid.Parse("81800266-17F7-4E5D-8DB7-F9D29D6E75AC"))
                            {
                                payurl = "pages/user/rechargewebview?hotelid=" + hid + "&formid=" + cloudlocationordermodel.Formid + "&onlineorder=1&ptype=stripe&email=" + cloudlocationordermodel.Email;
                            }
                            //如果是Elavon(Converge)
                            if (paymentmodel.Id == Guid.Parse("177B665D-FFFB-4C1C-82FD-EF77500D5297"))
                            {
                                payurl = "pages/user/rechargewebview?hotelid=" + hid + "&formid=" + cloudlocationordermodel.Formid + "&onlineorder=1&ptype=elavon";
                            }
                            //如果是Google Pay
                            if (paymentmodel.Id == Guid.Parse("3060D413-DADF-4AC3-AB84-64F729A85E44"))
                            {
                                payurl = "pages/user/rechargewebview?hotelid=" + hid + "&formid=" + cloudlocationordermodel.Formid + "&onlineorder=1&ptype=googlepay";
                            }
                            if (paymentmodel.Id == Guid.Parse("A15208B8-03EC-42FA-8D58-5F34E049ABC8"))
                            {
                                payurl = "pages/user/rechargewebview?hotelid=" + hid + "&formid=" + cloudlocationordermodel.Formid + "&onlineorder=1&ptype=applepay";
                            }
                            forminfo["payurl"] = payurl;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(ordertype) && ordertype != "kiosk")
                            {
                                if (!cloudlocationordermodel.Isackorder.HasValue || !cloudlocationordermodel.Isackorder.Value)
                                {
                                    try
                                    {
                                        forminfo["orderinfo"] = GetOrderInfoPublic(hid, FormId, language, _db, pmsContext, connstr, centerhotel, dbitem, cloudlocationordermodel, orderpaymentlist, cartmodellist, allmodifierlist, allexchangelist, allexchangemodiferlist, discountlist);
                                    }
                                    catch (Exception)
                                    {
                                        forminfo["orderinfo"] = null;
                                    }
                                }
                            }
                        }

                        //如果是Kiosk 直接返回OrderInfo
                        if (!string.IsNullOrWhiteSpace(ordertype) && ordertype == "kiosk")
                        {
                            return "formid:" + GetLocationOrderListPublic(hid, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), FormId, _db);
                        }
                        else
                        {
                            return "formid:" + forminfo.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            new SysLogService(_db).AddSysLog(hid + "|" + ex.ToString(), "CloudCommitOrder-Error", "", "", "");

                            if (false)
                            {
                                JObject requestdata = new JObject();
                                requestdata["hid"] = hid;
                                requestdata["callnumber"] = callnumber;
                                requestdata["formid"] = FormId;
                                requestdata["cartinfo"] = cartinfo;
                                requestdata["countrycode"] = countrycode;
                                requestdata["telephone"] = telephone;
                                requestdata["verifycode"] = verifycode;
                                requestdata["special"] = special;
                                requestdata["sectionid"] = sectionid;
                                requestdata["tablenumber"] = tablenumber;
                                requestdata["email"] = email;
                                requestdata["lastname"] = lastname;
                                requestdata["firstname"] = firstname;
                                requestdata["ordertype"] = ordertype;
                                requestdata["scheduledtime"] = scheduledtime;
                                requestdata["paymethod"] = paymethod;
                                requestdata["schedulemealminute"] = schedulemealminute;
                                requestdata["profileid"] = profileid;
                                requestdata["nowscheduledtime"] = nowscheduledtime;
                                requestdata["nowlatertime"] = nowlatertime;
                                requestdata["couponcode"] = couponcode;
                                requestdata["paytypeid"] = paytypeid;
                                requestdata["payid"] = payid;
                                requestdata["cuslocation"] = cuslocation;
                                requestdata["cuslocationname"] = cuslocationname;
                                requestdata["cuslocationjson"] = cuslocationjson;
                                requestdata["paymentlist"] = paymentlist;
                                requestdata["isapp"] = isapp;
                                requestdata["virtualprofileid"] = virtualprofileid;
                                requestdata["tokennumber"] = tokennumber;
                                requestdata["kiosknumber"] = kiosknumber;
                                requestdata["deliverydistance"] = deliverydistance;
                                requestdata["randomnumber"] = randomnumber;
                                requestdata["language"] = language;

                                new SysLogService(_db).AddSysLog(hid + "|" + requestdata.ToString(), "CloudCommitOrder-Error", "", "", "");
                            }
                        }
                        catch (Exception)
                        {
                        }
                        if (ex.ToString().Contains("System.ArgumentNullException"))
                        {
                            return "The network is abnormal, please re-order";
                        }
                        return ex.Message;
                    }
                }
            }
        }
