/// <summary>
        /// 获取订单详细信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="formid"></param>
        /// <returns></returns>
        public string GetOrderInfoPublic(string hid, string formid, string language, DbCommonContext _db = null, DbHotelPmsContext pmsContext = null, string connstr = null, CenterHotel centerhotel = null, DataBaseList dbitem = null, Cloudlocationorder ordermodel = null,
            List<Cloudlocationorderpayment> paymentlist = null,
            List<Cloudlocationordercart> cartmodellist = null, List<Cloudlocationordercartmodifier> allmodifierlist = null,
            List<Cloudlocationordercartcomboexchange> allexchangelist = null, List<Cloudlocationordercartcomboexchangemodiferlist> allexchangemodiferlist = null,
            List<Cloudlocationorderbilldiscount> discountlist = null)
        {
            JObject jobj = new JObject();
            jobj["cartinfo"] = null;
            if (_db == null)
            {
                string dbconnstr = ConfigurationManager.AppSettings["connstr"];
                _db = new DbCommonContext(dbconnstr);
            }
            if (centerhotel == null)
            {
                centerhotel = _db.Hotels.AsNoTracking().SingleOrDefault(x => x.Hid == hid);
            }
            if (dbitem == null)
            {
                dbitem = _db.DbLists.AsNoTracking().SingleOrDefault(x => x.Id == centerhotel.Dbid);
            }
            if (connstr == null)
            {
                connstr = ConnStrHelper.GetConnStr(dbitem.DbServer, dbitem.DbName, dbitem.LogId, dbitem.LogPwd, "GemstarBSPMS", dbitem.IntIp, _db.IsConnectViaInternetIp());
            }
            if (pmsContext == null)
            {
                pmsContext = GetPmsContext(_db, dbitem);
            }
            List<Ackblacklist> blacklist = null;

            string grpid = centerhotel.Hid;
            if (!string.IsNullOrWhiteSpace(centerhotel.Grpid) && centerhotel.Hid != centerhotel.Grpid)
            {
                grpid = centerhotel.Grpid;
            }


            JArray cartarray = new JArray();
            if (ordermodel == null)
            {
                ordermodel = pmsContext.Cloudlocationorder.AsNoTracking().SingleOrDefault(x => x.Formid == formid);
            }
            PmsHotel hotelinfo = pmsContext.PmsHotels.AsNoTracking().Single(x => x.Hid == ordermodel.Hid);
            HotelLocation hotellocation = pmsContext.HotelLocation.AsNoTracking().Where(x => x.Hid == ordermodel.Hid).FirstOrDefault();
            if (ordermodel.Isackorder.HasValue && ordermodel.Isackorder.Value)
            {
                blacklist = pmsContext.Ackblacklist.AsNoTracking().Where(x => x.Hid == hid).ToList();
            }

            if (paymentlist == null)
            {
                paymentlist = pmsContext.Cloudlocationorderpayment.AsNoTracking().Where(x => x.Formid == formid).OrderBy(x => x.Createdate).ToList();
            }
            else
            {
                paymentlist = paymentlist.OrderBy(x => x.Createdate).ToList();
            }
            if (ordermodel != null && hotelinfo != null)
            {
                JObject locationconfig = new JObject();
                JObject pickupconfig = new JObject();
                if (!string.IsNullOrWhiteSpace(hotelinfo.Locationorderconfig))
                {
                    locationconfig = JObject.Parse(hotelinfo.Locationorderconfig);
                }
                if (!string.IsNullOrWhiteSpace(hotelinfo.Locationpickupconfig))
                {
                    pickupconfig = JObject.Parse(hotelinfo.Locationpickupconfig);
                }
                jobj["isackorder"] = ordermodel.Isackorder;

                if(!ordermodel.Isackorder.HasValue || !ordermodel.Isackorder.Value)
                {
                    ordermodel.Ackstatus = 1;
                    ordermodel.Acktime = ordermodel.Createdate;
                    jobj["isautoackorder"] = true;
                }
                else
                {
                    jobj["isautoackorder"] = false;
                }

                jobj["ackstatus"] = ordermodel.Ackstatus;
                jobj["ackordermode"] = centerhotel.Ackordermode.HasValue ? centerhotel.Ackordermode : 0;

                if (blacklist != null && blacklist.Count(x => x.Mobilephone == ordermodel.Mobilephone) >= 1)
                {
                    jobj["isblack"] = true;
                }
                else
                {
                    jobj["isblack"] = false;
                }

                jobj["istestorder"] = ordermodel.Istestorder.HasValue && ordermodel.Istestorder.Value ? true : false;

                if (ordermodel.Acktime.HasValue)
                {
                    jobj["acktime"] = ordermodel.Acktime.Value.AddHours(centerhotel.UtcTimeNum);


                    jobj["acktimewebshop"] = ordermodel.Acktime.Value.AddHours(centerhotel.UtcTimeNum).ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (ordermodel.Ackmisstime.HasValue)
                {
                    jobj["ackmisstime"] = ordermodel.Ackmisstime.Value.AddHours(centerhotel.UtcTimeNum);
                }
                else
                {
                    jobj["ackmisstime"] = "";
                }

                if (ordermodel.Isonlinepay.HasValue && ordermodel.Isonlinepay.Value)
                {
                    jobj["isonlinepay"] = true;
                }
                else
                {
                    jobj["isonlinepay"] = false;
                }

                if (ordermodel.Isfirstorder.HasValue && ordermodel.Isfirstorder.Value)
                {
                    jobj["isfirstorder"] = true;
                }
                else
                {
                    jobj["isfirstorder"] = false;
                }

                if (ordermodel.Ackmissisconfirm.HasValue && ordermodel.Ackmissisconfirm.Value)
                {
                    jobj["ackmissisconfirm"] = ordermodel.Ackmissisconfirm.Value;
                }
                else
                {
                    jobj["ackmissisconfirm"] = false;
                }

                if (ordermodel.Scheduledcompletetime.HasValue)
                {
                    jobj["scheduledcompletetime"] = ordermodel.Scheduledcompletetime.Value.AddHours(centerhotel.UtcTimeNum);
                }
                if (ordermodel.Ackmissmustconfirm.HasValue && ordermodel.Ackmissmustconfirm.Value)
                {
                    jobj["ackmissmustconfirm"] = ordermodel.Ackmissmustconfirm.Value;
                }
                else
                {
                    jobj["ackmissmustconfirm"] = false;
                }

                jobj["issimplifyackorder"] = ordermodel.issimplifyackorder.HasValue && ordermodel.issimplifyackorder.Value ? true : false;

                //说明是now订单，需要返回计划完成时间
                if (ordermodel.Nowscheduledtime.HasValue && ordermodel.Scheduledcompletetime.HasValue && ordermodel.Scheduledtime.HasValue)
                {
                    TimeSpan tspan = ordermodel.Scheduledcompletetime.Value - Convert.ToDateTime(ordermodel.Createdate.ToString("yyyy-MM-dd HH:mm") + ":00");

                    jobj["ackscheduledcompletemin"] = Convert.ToInt32(Math.Abs(tspan.TotalMinutes));
                }
                else
                {
                    jobj["ackscheduledcompletemin"] = 0;
                }



                jobj["ordertime"] = ordermodel.Createdate.AddHours(centerhotel.UtcTimeNum);
                jobj["rejecterrormessage"] = ordermodel.Rejecterrormessage;

                jobj["isackmissforlater"] = ordermodel.Isackmissforlater.HasValue ? ordermodel.Isackmissforlater.Value : false;

                jobj["service"] = "";
                jobj["ordertype"] = ordermodel.Ordertype;

                jobj["tablenumber"] = ordermodel.Tablenumber;

                if (ordermodel.Ordertype == "pickup")
                {
                    jobj["service"] = "到店自取";
                }
                else if (ordermodel.Ordertype == "dinein")
                {
                    jobj["service"] = "堂食";
                }
                else if (ordermodel.Ordertype == "delivery")
                {
                    jobj["service"] = "商家配送";

                    try
                    {
                        string deliverydistance = ordermodel.Deliverydistance;
                        if (!string.IsNullOrWhiteSpace(deliverydistance) && Convert.ToDecimal(deliverydistance) > 0)
                        {
                            jobj["distance"] = (Convert.ToDecimal(deliverydistance) / 1000).ToString("0.00");
                        }
                        else
                        {
                            jobj["distance"] = 0;
                        }
                    }
                    catch (Exception)
                    {
                        jobj["distance"] = 0;
                    }
                }
                else if (ordermodel.Ordertype == "kiosk")
                {
                    jobj["service"] = "Kiosk";
                }

                jobj["pickuptime"] = "";
                if (ordermodel.Scheduledcompletetime.HasValue)
                {
                    jobj["pickuptime"] = ordermodel.Scheduledcompletetime.Value.AddHours(centerhotel.UtcTimeNum).ToString("yyyy-MM-dd HH:mm");


                    jobj["pickuptimewebshop"] = ordermodel.Scheduledcompletetime.Value.AddHours(centerhotel.UtcTimeNum).ToString("yyyy-MM-dd HH:mm:ss");


                    if (ordermodel.Acktime.HasValue)
                    {
                        jobj["pickupovermin"] = DateTimeHelper.ExecDateMinuteDiff(ordermodel.Acktime.Value, ordermodel.Scheduledcompletetime.Value);
                    }
                }

                if (ordermodel.Isonlinepayrefund.HasValue)
                {
                    jobj["isonlinepayrefund"] = ordermodel.Isonlinepayrefund.Value;
                }
                else
                {
                    jobj["isonlinepayrefund"] = false;
                }

                jobj["onlinepayrefundtime"] = "";
                if (ordermodel.Onlinepayrefundtime.HasValue)
                {
                    jobj["onlinepayrefundtime"] = ordermodel.Onlinepayrefundtime.Value.AddHours(centerhotel.UtcTimeNum).ToString("yyyy-MM-dd HH:mm:ss");
                }


                jobj["tip"] = !ordermodel.Tipprice.HasValue ? "0.00" : ordermodel.Tipprice.Value.ToString("0.00");
                jobj["tax"] = ordermodel.Taxprice.ToString("0.00");
                jobj["total"] = ordermodel.Totalprice.ToString("0.00");

                jobj["cartpoints"] = ordermodel.Cartpoints.HasValue ? Convert.ToInt32(ordermodel.Cartpoints) : 0;

                if (ordermodel.Drivertipprice.HasValue)
                {
                    jobj["drivertip"] = ordermodel.Drivertipprice.Value.ToString("0.00");
                }
                else
                {
                    jobj["drivertip"] = "0.00";
                }
                jobj["tokennumber"] = ordermodel.Tokennumber;
                jobj["storeaddress"] = "";
                jobj["storelocation"] = "";
                jobj["storephone"] = "";
                jobj["storename"] = "";


                if (hotellocation != null)
                {
                    jobj["storename"] = !string.IsNullOrWhiteSpace(hotellocation.StoreTitle) ? hotellocation.StoreTitle : "";
                    jobj["storephone"] = !string.IsNullOrWhiteSpace(hotellocation.StorePhone) ? hotellocation.StorePhone : "";
                    jobj["storeaddress"] = !string.IsNullOrWhiteSpace(hotellocation.StoreAddress) ? hotellocation.StoreAddress : "";
                    jobj["storelocation"] = !string.IsNullOrWhiteSpace(hotellocation.StoreLocation) ? hotellocation.StoreLocation : "";
                    string storecity = !string.IsNullOrWhiteSpace(hotellocation.City) ? hotellocation.City : "";
                    if (!string.IsNullOrWhiteSpace(storecity))
                    {
                        jobj["storeaddress"] += "," + storecity;
                    }
                }

                JObject customerinfo = new JObject();
                customerinfo["lastname"] = ordermodel.Lastname;
                customerinfo["firstname"] = ordermodel.Firstname;
                customerinfo["email"] = ordermodel.Email;
                if (!string.IsNullOrEmpty(ordermodel.Mobilephone))
                {
                    customerinfo["mobile"] = "+" + ordermodel.Countrycode + ordermodel.Mobilephone;
                }
                else
                {
                    customerinfo["mobile"] = "";
                }
                jobj["cartprice"] = ordermodel.Cartprice;
                jobj["customerinfo"] = customerinfo;
                jobj["customerspecial"] = ordermodel.Customerspecial;
                jobj["paymethod"] = ordermodel.Paymethod;

                jobj["deliverylocation"] = ordermodel.Deliverylocation;
                jobj["deliveryaddress"] = ordermodel.Deliveryaddress;
                jobj["deliveryfee"] = !ordermodel.Deliveryprice.HasValue ? 0 : ordermodel.Deliveryprice;

                jobj["cuslocationjson"] = ordermodel.Cuslocationjson;

                jobj["nowscheduledtime"] = ordermodel.Nowscheduledtime;
                if (!string.IsNullOrEmpty(ordermodel.Callnumber))
                {
                    jobj["callnumber"] = ordermodel.Callnumber;
                }

                jobj["formid"] = ordermodel.Formid;


                int seconds = 0;

                if (ordermodel.Ackmisstime.HasValue)
                {
                    DateTime ackmisstime = ordermodel.Ackmisstime.Value;
                    if (ackmisstime >= DateTime.UtcNow)
                    {
                        TimeSpan ts = ackmisstime - DateTime.UtcNow;    //计算时间差
                        seconds = (int)Math.Floor(ts.TotalSeconds); //将时间差转换为秒
                    }
                }
                jobj["oversecond"] = seconds;


                bool isgooglepay = false;
                bool isapplepay = false;

                if (pickupconfig["payment"] != null)
                {
                    JArray paymentarray = JArray.FromObject(pickupconfig["payment"]);

                    if (paymentlist == null || paymentlist.Count <= 0)
                    {
                        foreach (JObject item in paymentarray)
                        {
                            if (item["type"]?.ToString() == ordermodel.Paymethod)
                            {
                                jobj["paymethod"] = item["label"]?.ToString();
                                break;
                            }
                        }
                    }
                }
                if (paymentlist != null && paymentlist.Count > 0)
                {
                    string paymethod = "";
                    foreach (var item in paymentlist)
                    {
                        if (item.Paytypeid == Guid.Parse("3060D413-DADF-4AC3-AB84-64F729A85E44"))
                        {
                            isgooglepay = true;
                        }
                        if (item.Paytypeid == Guid.Parse("A15208B8-03EC-42FA-8D58-5F34E049ABC8"))
                        {
                            isapplepay = true;
                        }
                        paymethod += item.Payname + ",";
                    }
                    paymethod = paymethod.TrimEnd(',');
                    jobj["paymethod"] = paymethod;
                }

                jobj["isgooglepay"] = isgooglepay;
                jobj["isapplepay"] = isapplepay;


                decimal promotiondiscount = 0;
                if (cartmodellist == null)
                {
                    cartmodellist = pmsContext.Cloudlocationordercart.AsNoTracking().Where(x => x.Hid == hid && x.Formid == ordermodel.Formid).OrderBy(x => x.Cartseqid).ToList();
                }
                else
                {
                    cartmodellist = cartmodellist.OrderBy(x => x.Cartseqid).ToList();
                }
                if (allmodifierlist == null)
                {
                    allmodifierlist = pmsContext.Cloudlocationordercartmodifier.AsNoTracking().Where(x => x.Hid == hid && x.Formid == ordermodel.Formid).OrderBy(x => x.Cartseqid).ToList();
                }
                else
                {
                    allmodifierlist = allmodifierlist.OrderBy(x => x.Cartseqid).ToList();
                }
                if (allexchangelist == null)
                {
                    allexchangelist = pmsContext.Cloudlocationordercartcomboexchange.AsNoTracking().Where(x => x.Hid == hid && x.Formid == ordermodel.Formid).OrderBy(x => x.Cartseqid).ToList();
                }
                else
                {
                    allexchangelist = allexchangelist.OrderBy(x => x.Cartseqid).ToList();
                }
                if (allexchangemodiferlist == null)
                {
                    allexchangemodiferlist = pmsContext.Cloudlocationordercartcomboexchangemodiferlist.AsNoTracking().Where(x => x.Hid == hid && x.Formid == ordermodel.Formid).OrderBy(x => x.Cartseqid).ToList();
                }
                else
                {
                    allexchangemodiferlist = allexchangemodiferlist.OrderBy(x => x.Cartseqid).ToList();
                }
                foreach (var item in cartmodellist)
                {
                    JObject cartitemobj = new JObject();
                    cartitemobj["menuid"] = item.Menuid;
                    cartitemobj["menuclasscode"] = item.Menuclasscode;
                    cartitemobj["namedic"] = GetNameByDic(language, item.Namedic);
                    cartitemobj["qty"] = item.Qty;
                    cartitemobj["itemprice"] = item.Itemsumprice.ToString("0.00");
                    cartitemobj["itemunitprice"] = item.Itemprice.ToString("0.00");
                    if (item.Salejoincalnewdiscount.HasValue)
                    {
                        cartitemobj["salejoincalnewdiscount"] = item.Salejoincalnewdiscount;
                    }
                    if (!string.IsNullOrEmpty(item.Salejoincalname))
                    {
                        cartitemobj["salejoincalname"] = item.Salejoincalname;
                    }

                    cartitemobj["discountprice"] = null;
                    cartitemobj["special"] = item.Special;
                    cartitemobj["itemcode"] = item.Itemcode;

                    if (item.Saleid.HasValue)
                    {
                        cartitemobj["itemprice"] = item.Olditemsumprice.Value.ToString("0.00");
                        cartitemobj["itemunitprice"] = item.Olditemprice.Value.ToString("0.00");
                        cartitemobj["discountprice"] = item.Itemsumprice.ToString("0.00");
                        cartitemobj["salename"] = item.Salename;

                        promotiondiscount += item.Salediscount.Value;


                        //仅用于前台显示折扣
                        if (item.Salediscount.Value > 0)
                        {
                            cartitemobj["itemunitsalediscount"] = (item.Itemprice - (item.Salediscount.Value / item.Qty)).ToString("0.00");
                        }

                    }
                    if (item.Salejoincalid.HasValue)
                    {
                        promotiondiscount += item.Salejoincalnewdiscount.Value;
                    }


                    cartitemobj["sizenamedic"] = GetNameByDic(language, item.Sizenamedic);
                    cartitemobj["modnamedic"] = "";
                    cartitemobj["comnamedic"] = "";


                    cartitemobj["redeemsumpoints"] = item.Redeemsumpoints.HasValue ? Convert.ToInt32(item.Redeemsumpoints) : 0;
                    cartitemobj["redeemunitpoints"] = item.Redeemunitpoints.HasValue ? Convert.ToInt32(item.Redeemunitpoints) : 0;
                    cartitemobj["isredeempoint"] = item.Isredeempoint.HasValue ? item.Isredeempoint : false;


                    Dictionary<string, string> moddic = new Dictionary<string, string>();
                    List<Cloudlocationordercartmodifier> modifierlist = allmodifierlist.Where(x => x.Cartid == item.Id).OrderBy(x => x.Cartseqid).ToList();
                    foreach (var modifieritem in modifierlist)
                    {
                        if (!moddic.Keys.Contains(modifieritem.Modgroupcode))
                        {
                            moddic.Add(modifieritem.Modgroupcode, "");
                        }
                        if (string.IsNullOrEmpty(moddic[modifieritem.Modgroupcode]))
                        {
                            moddic[modifieritem.Modgroupcode] = GetNameByDic(language, modifieritem.Modgroupnamedic) + "：";
                        }
                        moddic[modifieritem.Modgroupcode] += GetNameByDic(language, modifieritem.Modifiernamedic) + " x" + modifieritem.Modifierqty + "|";
                    }
                    foreach (var moddickey in moddic.Keys)
                    {
                        cartitemobj["modnamedic"] += "[" + moddic[moddickey].TrimEnd('|') + "]" + " / ";
                    }
                    cartitemobj["modnamedic"] = cartitemobj["modnamedic"].ToString().Trim().TrimEnd('/');


                    Dictionary<string, Dictionary<string, string>> exchangedic = new Dictionary<string, Dictionary<string, string>>();
                    List<Cloudlocationordercartcomboexchange> exchangelist = allexchangelist.Where(x => x.Cartid == item.Id).OrderBy(x => x.Cartseqid).ToList();
                    foreach (var exitem in exchangelist)
                    {
                        string exchangemodlistnamedic = "";

                        if (!exchangedic.Keys.Contains(exitem.Exitemcode))
                        {
                            exchangedic.Add(exitem.Exitemcode, new Dictionary<string, string>());
                        }
                        var modiferlist = allexchangemodiferlist.Where(x => x.Exchangeid == exitem.Id && x.Cartid == item.Id).OrderBy(x => x.Cartseqid).ToList();
                        foreach (var moditem in modiferlist)
                        {
                            if (!exchangedic[exitem.Exitemcode].Keys.Contains(moditem.Modgroupcode))
                            {
                                exchangedic[exitem.Exitemcode].Add(moditem.Modgroupcode, "");
                            }
                            if (string.IsNullOrEmpty(exchangedic[exitem.Exitemcode][moditem.Modgroupcode]))
                            {
                                exchangedic[exitem.Exitemcode][moditem.Modgroupcode] = GetNameByDic(language, moditem.Modgroupnamedic) + "：";
                            }
                            exchangedic[exitem.Exitemcode][moditem.Modgroupcode] += GetNameByDic(language, moditem.Modifiernamedic) + " x" + moditem.Modifierqty + "|";
                        }
                    }

                    foreach (var exchangeitem in exchangedic.Keys)
                    {
                        string exchangemodlistnamedic = "";
                        foreach (var moditem in exchangedic[exchangeitem].Keys)
                        {
                            exchangemodlistnamedic += exchangedic[exchangeitem][moditem];
                        }
                        var exchangeitemmodel = exchangelist.FirstOrDefault(x => x.Exitemcode == exchangeitem);
                        cartitemobj["comnamedic"] += GetNameByDic(language, exchangeitemmodel.Exitemnamedic) + " x" + exchangeitemmodel.Exitemqty + " " + (!string.IsNullOrEmpty(exchangemodlistnamedic) ? "[" + exchangemodlistnamedic.TrimEnd('|') + "]" : "") + " /";
                    }
                    cartitemobj["comnamedic"] = cartitemobj["comnamedic"].ToString().Trim().TrimEnd('/');
                    cartarray.Add(cartitemobj);
                }
                if (discountlist == null)
                {
                    discountlist = pmsContext.Cloudlocationorderbilldiscount.AsNoTracking().Where(x => x.Formid == ordermodel.Formid && x.Hid == ordermodel.Hid).OrderBy(x => x.Billcalindex).ToList();
                }
                if (discountlist != null && discountlist.Count > 0)
                {
                    jobj["billdiscount"] = JArray.FromObject(discountlist);
                    promotiondiscount += discountlist.Sum(x => x.Salediscount);
                }
                jobj["promotiondiscount"] = promotiondiscount;

                JArray orderpaymentjarray = new JArray();

                if (paymentlist == null)
                {
                    paymentlist = pmsContext.Cloudlocationorderpayment.AsNoTracking().Where(x => x.Formid == ordermodel.Formid && x.Hid == ordermodel.Hid).OrderBy(x => x.Createdate).ToList();
                }
                if (paymentlist != null && paymentlist.Count > 0)
                {
                    JArray orderpaymentdbjarray = JArray.FromObject(paymentlist);
                    foreach (JObject payitem in orderpaymentdbjarray)
                    {
                        decimal paybalance = Convert.ToDecimal(payitem["Paybalance"]);
                        decimal paybonus = Convert.ToDecimal(payitem["Paybonus"]);

                        payitem["Payamount"] = (paybalance + paybonus).ToString("0.00");

                        orderpaymentjarray.Add(payitem);
                    }
                }
                if (ordermodel.Cartpoints.HasValue && ordermodel.Cartpoints.Value > 0)
                {
                    Cloudlocationorderpayment paymentmodel = new Cloudlocationorderpayment();
                    paymentmodel.Id = Guid.NewGuid();
                    paymentmodel.Formid = ordermodel.Formid;
                    paymentmodel.Payid = Guid.Parse("13D5572D-763D-4EAC-A2A3-21790EB5120E");
                    paymentmodel.Paytypeid = paymentmodel.Id;
                    paymentmodel.Payname = "Online Redeem";
                    paymentmodel.Paypoints = ordermodel.Cartpoints.Value;
                    paymentmodel.Paybalance = 0;
                    paymentmodel.Paybonus = 0;
                    paymentmodel.Createdate = DateTime.UtcNow;
                    paymentmodel.Hid = ordermodel.Hid;
                    paymentmodel.Excessamount = 0;

                    orderpaymentjarray.Add(JObject.FromObject(paymentmodel));
                }


                jobj["orderpaymentlist"] = orderpaymentjarray;
            }
            jobj["cartinfo"] = cartarray;

            return jobj.ToString();
        }
