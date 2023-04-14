  /// <summary>
        /// 获取店铺配置
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public string GetLocationorderSetting(string hid,bool getpickuplatertime,string globalcustomerid,string usertoken,string cuslocation = "",string orderusertokenisapp = "",string tableid = "")
        {
            CenterHotel hotelModel = _db.Hotels.AsNoTracking().Where(x => x.Hid == hid).FirstOrDefault();

            string googlekey = hotelModel.googlekey;

            string grpid = hotelModel.Hid;

            if (!string.IsNullOrWhiteSpace(hotelModel.Grpid) && hotelModel.Grpid != hotelModel.Hid)
            {
                grpid = hotelModel.Grpid;

                if (string.IsNullOrWhiteSpace(googlekey))
                {
                    CenterHotel grphotelModel = _db.Hotels.Where(x => x.Hid == grpid).FirstOrDefault();
                    if(grphotelModel != null)
                    {
                        googlekey = grphotelModel.googlekey;
                    }
                }
            }

            int customerpricelevel = 0;

            bool addcustomercouponing = false; //该账户是否包含优惠券


            string sectionidfortable = null;

            string memberprofileid = null;
            Entities.Member member = null;
            if (!string.IsNullOrEmpty(usertoken))
            {
                member = _db.Member.FirstOrDefault(x => x.Restaurantloginkey == usertoken && x.hotelid == grpid);
                if (member != null)
                {
                    memberprofileid = member.Profileid;
                }
            }

            DataBaseList dbitem = _db.DbLists.AsNoTracking().Where(x => x.Id == hotelModel.Dbid).FirstOrDefault();
            string connstr = ConnStrHelper.GetConnStr(dbitem.DbServer, dbitem.DbName, dbitem.LogId, dbitem.LogPwd, "GemstarBSPMS", dbitem.IntIp, _db.IsConnectViaInternetIp());
            //string connstr1 = "data source=rm-0xi7v0wo82zmrd9a0vo.sqlserver.rds.aliyuncs.com,1544;initial catalog=quickpos001;user id=pms;password=JxdPms@598;MultipleActiveResultSets=True;App=GemstarBSPMS"; //

            string couponnowtime = DateTime.UtcNow.AddHours(hotelModel.UtcTimeNum).ToString("yyyy-MM-dd HH:mm:ss");
            string couponwheresql = " and uc.status = 0 and cp.status = 0 and cp.isDelete = 0 and (cp.sdate is null or '" + couponnowtime + "' >= cp.sdate) and (cp.edate is null or cp.edate >= '" + couponnowtime + "') and (uc.expirydate is null or uc.expirydate >= GETUTCDATE()) and (cp.location is null or cp.location = '' or cp.location like '%" + hotelModel.Hid + "%') ";

            string sql = " select isnull(locationorderconfig,'') as locationorderconfig,isnull(locationpickupconfig,'') as locationpickupconfig,isnull(locationdeliveryconfig,'') as locationdeliveryconfig,isnull(locationkioskconfig,'') as locationkioskconfig from pmsHotel where hid = '" + hotelModel.Hid + "';select isnull(offineconfig,'') from pmshotel where hid = '" + grpid + "' ";
            if (!string.IsNullOrEmpty(memberprofileid))
            {
                sql += " ;select isnull(balance,0) as balance,isnull(free,0) as free,isnull(score,0) as score,isnull(freezebalance,0) as freezebalance,isnull(freezebonus,0) as freezebonus,isnull(freezescore,0) as freezescore from profileBalance where profileid = '" + memberprofileid + "'  ";
                sql += " ;select top 1 pl.id as policyid,pl.eventID as eventid,pl.policyDesc,amountrate,pointsrate,es.* from pointpolicy pl left join eventset es on pl.eventID  = es.id where pl.hid = '" + grpid + "' and pl.status = 0 and (pl.eventID is null or es.status = 0) and pl.mbrCardTypeid = (select top 1 mbrcardtypeid from profile where id = '" + memberprofileid + "') ";
                sql += " ;select * from PosHoliday where hid = '" + grpid + "' and status = 0 ";
                sql += " ;select ISNULL(pricelevel,0) as pricelevel from mbrCardType where id = (select top 1 mbrCardTypeid from profile where id = '" + memberprofileid + "') ";
                sql += " ;select isnull(COUNT(*),0) from mrusercoupon uc left join mrcoupon cp on uc.couponid = cp.id where uc.profileid = '" + memberprofileid + "' " + couponwheresql;
            }
            if (!string.IsNullOrWhiteSpace(tableid))
            {
                sql += " ;select section.id from cloudtablemodel tb left join cloudsectionmodel section on tb.sectionindex = section.sectionindex where tb.hid = '" + hotelModel.Hid + "' and section.hid = '" + hotelModel.Hid + "' and tb.id = '" + Guid.Parse(tableid) + "' ";
            }

            DataSet configdset = ADOHelper.ExecuteSql(sql, connstr);

            if (!string.IsNullOrEmpty(memberprofileid))
            {
                try
                {
                    customerpricelevel = Convert.ToInt32(configdset.Tables[5].Rows[0][0]);
                }
                catch (Exception) { }
                try
                {
                    addcustomercouponing = Convert.ToInt32(configdset.Tables[6].Rows[0][0]) > 0 ? true : false;
                }
                catch (Exception) { }
            }
            if (!string.IsNullOrWhiteSpace(tableid))
            {
                try
                {
                    if (configdset.Tables[configdset.Tables.Count - 1] != null && configdset.Tables[configdset.Tables.Count - 1].Rows.Count > 0)
                    {
                        sectionidfortable = configdset.Tables[configdset.Tables.Count - 1].Rows[0][0]?.ToString();
                    }
                }
                catch (Exception) { }
            }

            DataTable configdtable = configdset.Tables[0];

            JObject jobj = new JObject();
            JObject pickupobj = null;
            JObject deliveryobj = null;
            JObject kioskobj = null;

            JObject offineconfig = null;



            if(configdset.Tables[1] != null && configdset.Tables[1].Rows.Count > 0)
            {
                string offineconfigstr =  configdset.Tables[1].Rows[0][0]?.ToString();
                if (!string.IsNullOrWhiteSpace(offineconfigstr))
                {
                    offineconfig = JObject.Parse(offineconfigstr);
                }
            }

            if (configdtable != null && configdtable.Rows.Count > 0)
            {
                try
                {
                    string locationorderconfig = configdtable.Rows[0]["locationorderconfig"]?.ToString();
                    if (!string.IsNullOrEmpty(locationorderconfig))
                    {
                        jobj = JObject.Parse(locationorderconfig);


                        if (!string.IsNullOrWhiteSpace(jobj["submitpwdverify"]?.ToString()) && string.IsNullOrWhiteSpace(jobj["submitpwdverifymethod"]?.ToString()))
                        {
                            jobj["submitpwdverifymethod"] = Convert.ToBoolean(jobj["submitpwdverify"]) ? "dynamic" : "none";
                        }
                        if (!string.IsNullOrWhiteSpace(sectionidfortable) && !string.IsNullOrWhiteSpace(jobj["submitpwdverifymethod"]?.ToString()) && jobj["submitpwdverifymethod"]?.ToString() == "dynamic")
                        {
                            string noverificationsection = jobj["submitpwdverifymethoddynamicnoverificationsection"]?.ToString(); //免验证的区域
                            if (!string.IsNullOrWhiteSpace(noverificationsection))
                            {
                                if (noverificationsection.ToLower().Contains(sectionidfortable.ToLower()))
                                {
                                    jobj["submitpwdverifymethod"] = "none";
                                }
                            }
                        }

                        #region 设置openhour显示的内容
                        try
                        {
                            string todaytitle = "";
                            string todayopenhourtext = "";
                            int dayforweek = (int)DateTime.UtcNow.AddHours(hotelModel.UtcTimeNum).DayOfWeek;
                            switch (dayforweek)
                            {
                                case 1:
                                    todaytitle = "Monday";
                                    break;
                                case 2:
                                    todaytitle = "Tuesday";
                                    break;
                                case 3:
                                    todaytitle = "Wednesday";
                                    break;
                                case 4:
                                    todaytitle = "Thursday";
                                    break;
                                case 5:
                                    todaytitle = "Friday";
                                    break;
                                case 6:
                                    todaytitle = "Saturday";
                                    break;
                                case 0:
                                    todaytitle = "Sunday";
                                    break;
                                default:
                                    break;
                            }
                            jobj["todaytitle"] = todaytitle;
                            jobj["showopenhour"] = true;
                            string timeliststr = jobj["timelist"]?.ToString();
                            if (!string.IsNullOrEmpty(timeliststr))
                            {
                                JArray timelist = JArray.Parse(timeliststr);

                                if (timelist != null && timelist.Count > 0)
                                {
                                    foreach (JObject timeitem in timelist)
                                    {
                                        if (timeitem["dayforweek"] != null)
                                        {
                                            int timedayforweek = Convert.ToInt32(timeitem["dayforweek"]);
                                            if(timedayforweek == 7)
                                            {
                                                timedayforweek = 0;
                                            }
                                            if (timedayforweek == dayforweek)
                                            {
                                                bool wholeday = Convert.ToBoolean(timeitem["wholeday"]);
                                                if (wholeday)
                                                {
                                                    todayopenhourtext = "Open 24 Hours";
                                                    break;
                                                }
                                                else
                                                {
                                                    string starttime = timeitem["starttime"]?.ToString();
                                                    string endtime = timeitem["endtime"]?.ToString();

                                                    if (!string.IsNullOrEmpty(todayopenhourtext))
                                                    {
                                                        todayopenhourtext += " , ";
                                                    }
                                                    todayopenhourtext += starttime + " ~ " + endtime;
                                                }
                                            }
                                        }
                                    }
                                    if (string.IsNullOrWhiteSpace(todayopenhourtext))
                                    {
                                        jobj["todayopenhourtext"] = "Close";
                                    }
                                    else
                                    {
                                        jobj["todayopenhourtext"] = todayopenhourtext;
                                    }
                                }
                                else
                                {
                                    jobj["todayopenhourtext"] = "Open 24 Hours";
                                }
                            }
                            else
                            {
                                jobj["todayopenhourtext"] = "Open 24 Hours";
                            }
                        }
                        catch (Exception) { }
                        #endregion



                        #region 获取一周7天 openhour显示的内容


                        JArray sevenopenhour = new JArray();
                        for (int i = 1; i <= 7; i++)
                        {
                            JObject dayobj = new JObject();
                            try
                            {
                                string todaytitle = "";
                                string todayopenhourtext = "";
                                int dayforweek = (i == 7 ? 0 : i);
                                switch (dayforweek)
                                {
                                    case 1:
                                        todaytitle = "Monday";
                                        break;
                                    case 2:
                                        todaytitle = "Tuesday";
                                        break;
                                    case 3:
                                        todaytitle = "Wednesday";
                                        break;
                                    case 4:
                                        todaytitle = "Thursday";
                                        break;
                                    case 5:
                                        todaytitle = "Friday";
                                        break;
                                    case 6:
                                        todaytitle = "Saturday";
                                        break;
                                    case 0:
                                        todaytitle = "Sunday";
                                        break;
                                    default:
                                        break;
                                }
                                dayobj["todaytitle"] = todaytitle;
                                dayobj["showopenhour"] = true;
                                string timeliststr = jobj["timelist"]?.ToString();
                                if (!string.IsNullOrEmpty(timeliststr))
                                {
                                    JArray timelist = JArray.Parse(timeliststr);

                                    if (timelist != null && timelist.Count > 0)
                                    {
                                        foreach (JObject timeitem in timelist)
                                        {
                                            if (timeitem["dayforweek"] != null)
                                            {
                                                int timedayforweek = Convert.ToInt32(timeitem["dayforweek"]);
                                                if (timedayforweek == 7)
                                                {
                                                    timedayforweek = 0;
                                                }
                                                if (timedayforweek == dayforweek)
                                                {
                                                    bool wholeday = Convert.ToBoolean(timeitem["wholeday"]);
                                                    if (wholeday)
                                                    {
                                                        todayopenhourtext = "Open 24 Hours";
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        string starttime = timeitem["starttime"]?.ToString();
                                                        string endtime = timeitem["endtime"]?.ToString();

                                                        if (!string.IsNullOrEmpty(todayopenhourtext))
                                                        {
                                                            todayopenhourtext += " , ";
                                                        }
                                                        todayopenhourtext += starttime + " ~ " + endtime;
                                                    }
                                                }
                                            }
                                        }
                                        if (string.IsNullOrWhiteSpace(todayopenhourtext))
                                        {
                                            dayobj["todayopenhourtext"] = "Close";
                                        }
                                        else
                                        {
                                            dayobj["todayopenhourtext"] = todayopenhourtext;
                                        }
                                    }
                                    else
                                    {
                                        dayobj["todayopenhourtext"] = "Open 24 Hours";
                                    }
                                }
                                else
                                {
                                    dayobj["todayopenhourtext"] = "Open 24 Hours";
                                }
                            }
                            catch (Exception) { }
                            sevenopenhour.Add(dayobj);
                        }

                        var isrepeat = true;

                        string openhourtext = "";
                        foreach (var item in sevenopenhour)
                        {
                            if (string.IsNullOrWhiteSpace(openhourtext))
                            {
                                openhourtext = item["todayopenhourtext"]?.ToString();
                            }
                            else
                            {
                                if(openhourtext != item["todayopenhourtext"]?.ToString())
                                {
                                    isrepeat = false;
                                }
                            }
                        }
                        if (isrepeat)
                        {
                            JObject sevenobj = JObject.FromObject(sevenopenhour[0]);
                            sevenobj["todaytitle"] = "Monday - Sunday";
                            sevenopenhour = new JArray();
                            sevenopenhour.Add(sevenobj);
                        }

                        jobj["sevenopenhour"] = sevenopenhour;
                        #endregion
                    }

                    string locationpickupconfig = configdtable.Rows[0]["locationpickupconfig"]?.ToString();
                    if (!string.IsNullOrEmpty(locationpickupconfig))
                    {
                        pickupobj = JObject.Parse(locationpickupconfig);

                        #region 针对指定的ordertype设置了openhour 执行一下事件
                        try
                        {
                            if (pickupobj["timelist"] != null && pickupobj["timelist"].ToString().Trim() != "")
                            {
                                JArray testtimelist = JArray.Parse(pickupobj["timelist"].ToString());
                                if (testtimelist != null && testtimelist.Count > 0)
                                {
                                    pickupobj["firstorderoffset"] = 0;
                                    pickupobj["lastorderoffset"] = 0;

                                    pickupobj["latertimefirstorderoffset"] = 0;
                                    pickupobj["latertimelastorderoffset"] = 0;
                                }
                            }
                        }
                        catch (Exception) { }
                        #endregion
                    }

                    string locationdeliveryconfig = configdtable.Rows[0]["locationdeliveryconfig"]?.ToString();
                    if (!string.IsNullOrEmpty(locationdeliveryconfig))
                    {
                        deliveryobj = JObject.Parse(locationdeliveryconfig);

                        #region 针对指定的ordertype设置了openhour 执行一下事件
                        try
                        {
                            if (deliveryobj["timelist"] != null && deliveryobj["timelist"].ToString().Trim() != "")
                            {
                                JArray testtimelist = JArray.Parse(deliveryobj["timelist"].ToString());
                                if (testtimelist != null && testtimelist.Count > 0)
                                {
                                    deliveryobj["firstorderoffset"] = 0;
                                    deliveryobj["lastorderoffset"] = 0;

                                    deliveryobj["latertimefirstorderoffset"] = 0;
                                    deliveryobj["latertimelastorderoffset"] = 0;
                                }
                            }
                        }
                        catch (Exception) { }
                        #endregion
                    }
                    string locationkioskconfig = configdtable.Rows[0]["locationkioskconfig"]?.ToString();
                    if (!string.IsNullOrEmpty(locationkioskconfig))
                    {
                        kioskobj = JObject.Parse(locationkioskconfig);

                        #region 针对指定的ordertype设置了openhour 执行一下事件
                        try
                        {
                            if (kioskobj["timelist"] != null && kioskobj["timelist"].ToString().Trim() != "")
                            {
                                JArray testtimelist = JArray.Parse(kioskobj["timelist"].ToString());
                                if (testtimelist != null && testtimelist.Count > 0)
                                {
                                    kioskobj["firstorderoffset"] = 0;
                                    kioskobj["lastorderoffset"] = 0;

                                    kioskobj["latertimefirstorderoffset"] = 0;
                                    kioskobj["latertimelastorderoffset"] = 0;
                                }
                            }
                        }
                        catch (Exception) { }
                        #endregion
                    }

                }
                catch (Exception)
                {
                    jobj = new JObject();
                    pickupobj = new JObject();
                    deliveryobj = new JObject();
                    kioskobj = new JObject();
                }
            }

            if (hotelModel.Ackordermode.HasValue)
            {
                jobj["ackordermode"] = hotelModel.Ackordermode;
            }

            jobj["canusemembersystem"] = false;


            if (offineconfig != null)
            {
                if (!string.IsNullOrWhiteSpace(offineconfig["Onlineordercanusemembersystem"]?.ToString()) && offineconfig["Onlineordercanusemembersystem"]?.ToString() == "1")
                {
                    jobj["canusemembersystem"] = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(orderusertokenisapp))
            {
                jobj["canusemembersystem"] = true;
            }




            DataTable tagtable = ADOHelper.ExecSql("  select * from clouditemtagmodel where hid = @hid or ispublic = 1 order by displayindex asc ", connstr, new List<SqlParameter>() {
                new SqlParameter("@hid",hid)
            });

            jobj["taglist"] = null;
            if (tagtable != null && tagtable.Rows.Count > 0)
            {
                jobj["taglist"] = JArray.FromObject(tagtable);
            }


            //客户端唯一的customerid，当 用户没有id的时候 就会读取存这个id
            jobj["paycustomerid"] = Guid.NewGuid();
            jobj["customerpricelevel"] = customerpricelevel;
            jobj["addcustomercouponing"] = addcustomercouponing;

            jobj["utctimenum"] = hotelModel.UtcTimeNum;

            jobj["googlekey"] = googlekey;

            jobj["isenablehmscy"] = _db.HotelProducts.Count(x => x.Hid == hid && x.IsEnable && x.ProductCode == "hmscy") >= 1;


            jobj["membersystem"] = false;

            var hotelproduct = _db.HotelProducts.FirstOrDefault(x => x.IsEnable && x.ProductCode == "member" && x.Hid == grpid);
            if(hotelproduct != null)
            {
                jobj["membersystem"] = true;
            }

            if (!Convert.ToBoolean(jobj["canusemembersystem"]))
            {
                jobj["membersystem"] = false;
            }


            jobj["grpid"] = grpid;
            jobj["grptitle"] = "";
            jobj["regmethod"] = 3;
            if(offineconfig!= null)
            {
                if(offineconfig["RegisterMethod"] != null)
                {
                    jobj["regmethod"] = Convert.ToInt32(offineconfig["RegisterMethod"]);
                }
                if (offineconfig["IndexTitle"] != null)
                {
                    jobj["grptitle"] = offineconfig["IndexTitle"]?.ToString();
                }
            }


            if (jobj["logo"] == null)
            {
                jobj["logo"] = "";
            }

            #region 点餐默认配置
            if (jobj["dineinenable"] == null)
            {
                jobj["dineinenable"] = true;
            }
            #endregion


            if (jobj["showcheckoutpage"] == null)
            {
                jobj["showcheckoutpage"] = true;
            }

            if (jobj["submitverify"] == null)
            {
                jobj["submitverify"] = true;
            }
            if (jobj["hotelname"] == null)
            {
                jobj["hotelname"] = hotelModel.Name;
            }
            if (jobj["submitpwdverify"] == null)
            {
                jobj["submitpwdverify"] = false;
            }
            if (jobj["submitpwdperiod"] == null)
            {
                jobj["submitpwdperiod"] = 0;
            }
            if (jobj["peoplemaxordercount"] == null)
            {
                jobj["peoplemaxordercount"] = 0;
            }
            if (jobj["perorder"] == null)
            {
                jobj["perorder"] = 0;
            }
            if (jobj["languagelist"] == null)
            {
                JArray languagelist = new JArray();
                JObject languageobj = new JObject();
                languageobj["isenable"] = true;
                languageobj["language"] = "en";
                languagelist.Add(languageobj);
                jobj["languagelist"] = languagelist;
            }
            else
            {
                try
                {
                    JArray languagelist = JArray.FromObject(jobj["languagelist"]);
                    for (int i = 0; i < languagelist.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(languagelist[i]["isenable"]?.ToString()) && !Convert.ToBoolean(languagelist[i]["isenable"]))
                        {
                            languagelist.RemoveAt(i);
                            i--;
                        }
                    }
                    jobj["languagelist"] = languagelist;
                }
                catch (Exception) { 
                }
                try
                {
                    jobj["peoplemaxordercount"] = Convert.ToInt32(jobj["peoplemaxordercount"]);
                }
                catch (Exception)
                {
                    jobj["peoplemaxordercount"] = 0;
                }
            }
            if (jobj["showtemplate"] == null)
            {
                jobj["showtemplate"] = "default";
            }

            if (jobj["nopicshowlogo"] == null)
            {
                jobj["nopicshowlogo"] = "none";
            }
            else if (jobj["nopicshowlogo"] != null)
            {
                if (jobj["nopicshowlogo"]?.ToString().ToLower() == "true")
                {
                    jobj["nopicshowlogo"] = "showlogo";
                }
                if (jobj["nopicshowlogo"]?.ToString().ToLower() == "false")
                {
                    jobj["nopicshowlogo"] = "none";
                }
            }
            if(jobj["nopicshowlogo"]?.ToString() == "showlogo")
            {
                jobj["itemcustompic"] = jobj["logo"]?.ToString();
            }

            if (jobj["itemmodifiergroupshrink"] == null)
            {
                jobj["itemmodifiergroupshrink"] = true;
            }
            if (jobj["itemmodifiergroupshrinknum"] == null)
            {
                jobj["itemmodifiergroupshrinknum"] = 2;
            }

            if(jobj["itemspecialdescription"] == null)
            {
                jobj["itemspecialdescription"] = false;
            }

            if (jobj["showselectitem"] == null)
            {
                jobj["showselectitem"] = true;
            }
            if(jobj["pricelevel"] == null)
            {
                jobj["pricelevel"] = 1;
            }

            if (jobj["autoclearcarthour"] == null)
            {
                jobj["autoclearcarthour"] = 3;
            }

            if (jobj["ordertipdisplayname"] == null)
            {
                jobj["ordertipdisplayname"] = "Tip the Store Servers";
            }

            if (jobj["hideunavailableitems"] == null)
            {
                jobj["hideunavailableitems"] = true;
            }
            if (jobj["hidesoldoutitems"] == null)
            {
                jobj["hidesoldoutitems"] = true;
            }

            jobj["locationinfo"] = null;
            //位置
            try
            {
                DataTable dtable = ADOHelper.ExecSql(" select top 1 * from hotelLocation where hid = @hotelid ", connstr, new List<SqlParameter>() {
                      new SqlParameter("@hotelid",hotelModel.Hid),
                });
                if (dtable != null && dtable.Rows.Count > 0)
                {
                    JObject locationobj = new JObject();
                    locationobj["StoreTitle"] = dtable.Rows[0]["storeTitle"].ToString();
                    locationobj["StoreAddress"] = dtable.Rows[0]["storeAddress"].ToString();
                    locationobj["StoreLocation"] = dtable.Rows[0]["storeLocation"].ToString();
                    locationobj["StoreSTime"] = dtable.Rows[0]["storeSTime"].ToString();
                    locationobj["StoreETime"] = dtable.Rows[0]["storeETime"].ToString();
                    locationobj["StorePhone"] = dtable.Rows[0]["storePhone"].ToString();
                    locationobj["city"] = dtable.Rows[0]["city"].ToString();

                    locationobj["unit"] = null;
                    if(dtable.Rows[0]["unit"] != DBNull.Value && dtable.Rows[0]["unit"] != null)
                    {
                        string unit = dtable.Rows[0]["unit"]?.ToString();

                        if (!string.IsNullOrWhiteSpace(unit))
                        {
                            locationobj["StoreAddress"] = unit + ", " + locationobj["StoreAddress"];
                        }
                    }

                    jobj["locationinfo"] = locationobj;
                }
            }
            catch (Exception) { }

            try
            {
                List<Guid> onlinepaymethod = _db.Cloudpayment.AsNoTracking().Where(x => x.Isonlinepay).Select(x => x.Id).ToList();

                DataTable dtable = ADOHelper.ExecSql(" select * from cloudlocationpayment where hid = @hotelid and isdelete = 0 order by displayindex ", connstr,new List<SqlParameter>() {
                      new SqlParameter("@hotelid",hotelModel.Hid),
                });
                for (int i = 0; i < dtable.Rows.Count; i++)
                {
                    JObject configobj = JObject.Parse(dtable.Rows[i]["config"]?.ToString());
                    if (configobj["enable"] != null && !Convert.ToBoolean(configobj["enable"]))
                    {
                        dtable.Rows[i].Delete();
                    }
                }
                dtable.AcceptChanges();
                if (dtable != null && dtable.Rows.Count > 0)
                {
                    JArray paymentlist = JArray.FromObject(dtable);

                    int typecount = 0;
                    int eMoneycount = 0;
                    for (int i = 0; i < paymentlist.Count; i++)
                    {
                        paymentlist[i]["isavailable"] = true;

                        string paytypeid = paymentlist[i]["typeid"]?.ToString();

                        try
                        {
                            Guid paytypegid = Guid.Parse(paytypeid);

                            if (onlinepaymethod != null && onlinepaymethod.Contains(paytypegid))
                            {
                                paymentlist[i]["isonlinepay"] = true;
                            }
                            else
                            {
                                paymentlist[i]["isonlinepay"] = false;
                            }
                        }
                        catch (Exception)
                        {
                            paymentlist[i]["isonlinepay"] = false;
                        }

                        //如果是积分兑换emoney，目前不可用
                        if (paytypeid.ToLower() == "413a8b65-7522-4659-8fa5-260e602a32b9")
                        {
                            if (member == null)
                            {
                                paymentlist[i]["isavailable"] = false;
                            }


                            typecount++;

                            bool pointsredeem = false; //是否允许积分兑换
                            paymentlist[i]["id"] = "6acd71df-c9f3-4d15-8598-0743a1e1f478"; //6acd71df-c9f3-4d15-8598-0743a1e1f478
                            paymentlist[i]["typeid"] = "413a8b65-7522-4659-8fa5-260e602a32b9"; //413a8b65-7522-4659-8fa5-260e602a32b9
                            paymentlist[i]["hid"] = hotelModel.Hid;
                            paymentlist[i]["points"] = 0;
                            paymentlist[i]["redeempoints"] = 0;
                            paymentlist[i]["redeememoney"] = 0;
                            paymentlist[i]["oldmax"] = 0;
                            paymentlist[i]["max"] = 0;

                            if (typecount == 1 && member != null && configdset.Tables[2] != null && configdset.Tables[2].Rows.Count > 0)
                            {
                                try
                                {
                                    decimal userscore = 0;
                                    if (member != null)
                                    {
                                        userscore = Convert.ToDecimal(configdset.Tables[2].Rows[0]["score"]);
                                    }
                                    paymentlist[i]["points"] = userscore;
                                    if (userscore > 0)
                                    {
                                        //读取积分政策
                                        if (configdset.Tables[3] != null && configdset.Tables[3].Rows.Count > 0)
                                        {
                                            string pointrate = Convert.ToString(configdset.Tables[3].Rows[0]["pointsrate"] == DBNull.Value || configdset.Tables[3].Rows[0]["pointsrate"] == null ? "" : configdset.Tables[3].Rows[0]["pointsrate"]);

                                            if (!string.IsNullOrEmpty(pointrate) && pointrate.Contains(":"))
                                            {
                                                decimal rateA = Convert.ToDecimal(pointrate.Split(':')[0]);

                                                paymentlist[i]["rateA"] = rateA;
                                                paymentlist[i]["rateB"] = Convert.ToDecimal(pointrate.Split(':')[1]);

                                                if (rateA == 1)
                                                {
                                                    paymentlist[i]["max"] = userscore;
                                                }
                                                else if (rateA > 0)
                                                {
                                                    decimal canusescore = 0;
                                                    while (true)
                                                    {
                                                        canusescore = canusescore + rateA;
                                                        if ((canusescore + rateA) > userscore)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    paymentlist[i]["oldmax"] = canusescore;
                                                    paymentlist[i]["max"] = canusescore;
                                                }
                                            }
                                            #region 如果政策有限制时间段，进行时间段限制
                                            string eventid = Convert.ToString(configdset.Tables[3].Rows[0]["eventid"] == DBNull.Value || configdset.Tables[3].Rows[0]["eventid"] == null ? "" : configdset.Tables[3].Rows[0]["eventid"]);
                                            if (!string.IsNullOrEmpty(eventid))
                                            {

                                                DateTime storeutcnow = DateTime.UtcNow.AddHours(hotelModel.UtcTimeNum);

                                                int dayofweek = (int)storeutcnow.DayOfWeek;

                                                int isAvailableBetween = Convert.ToInt32(configdset.Tables[3].Rows[0]["isAvailableBetween"]);
                                                string fromdate = Convert.ToString(configdset.Tables[3].Rows[0]["FromDate"] == DBNull.Value || configdset.Tables[3].Rows[0]["FromDate"] == null ? "" : configdset.Tables[3].Rows[0]["FromDate"]);
                                                string todate = Convert.ToString(configdset.Tables[3].Rows[0]["ToDate"] == DBNull.Value || configdset.Tables[3].Rows[0]["ToDate"] == null ? "" : configdset.Tables[3].Rows[0]["ToDate"]);

                                                string monday = Convert.ToString(configdset.Tables[3].Rows[0]["MonDay"] == DBNull.Value || configdset.Tables[3].Rows[0]["MonDay"] == null ? "" : configdset.Tables[3].Rows[0]["MonDay"]);
                                                string tuesDay = Convert.ToString(configdset.Tables[3].Rows[0]["TuesDay"] == DBNull.Value || configdset.Tables[3].Rows[0]["TuesDay"] == null ? "" : configdset.Tables[3].Rows[0]["TuesDay"]);
                                                string WednesDay = Convert.ToString(configdset.Tables[3].Rows[0]["WednesDay"] == DBNull.Value || configdset.Tables[3].Rows[0]["WednesDay"] == null ? "" : configdset.Tables[3].Rows[0]["WednesDay"]);
                                                string ThursDay = Convert.ToString(configdset.Tables[3].Rows[0]["ThursDay"] == DBNull.Value || configdset.Tables[3].Rows[0]["ThursDay"] == null ? "" : configdset.Tables[3].Rows[0]["ThursDay"]);
                                                string FriDay = Convert.ToString(configdset.Tables[3].Rows[0]["FriDay"] == DBNull.Value || configdset.Tables[3].Rows[0]["FriDay"] == null ? "" : configdset.Tables[3].Rows[0]["FriDay"]);
                                                string SaturDay = Convert.ToString(configdset.Tables[3].Rows[0]["SaturDay"] == DBNull.Value || configdset.Tables[3].Rows[0]["SaturDay"] == null ? "" : configdset.Tables[3].Rows[0]["SaturDay"]);
                                                string SunDay = Convert.ToString(configdset.Tables[3].Rows[0]["SunDay"] == DBNull.Value || configdset.Tables[3].Rows[0]["SunDay"] == null ? "" : configdset.Tables[3].Rows[0]["SunDay"]);
                                                string HoliDay = Convert.ToString(configdset.Tables[3].Rows[0]["HoliDay"] == DBNull.Value || configdset.Tables[3].Rows[0]["HoliDay"] == null ? "" : configdset.Tables[3].Rows[0]["HoliDay"]);
                                                //SaturDay

                                                if (isAvailableBetween == 1)
                                                {
                                                    //判断一下是否在指定的日期范围内
                                                    if (storeutcnow >= Convert.ToDateTime(fromdate + " 00:00:00") && storeutcnow <= Convert.ToDateTime(todate + " 23:59:59"))
                                                    {
                                                        pointsredeem = true;
                                                    }
                                                    else
                                                    {
                                                        pointsredeem = false;
                                                    }
                                                }
                                                else
                                                {
                                                    pointsredeem = true;
                                                }

                                                if (pointsredeem)
                                                {
                                                    bool isHoliday = false;
                                                    string useday = "";


                                                    if (configdset.Tables[4] != null && configdset.Tables[4].Rows.Count > 0)
                                                    {
                                                        foreach (DataRow item in configdset.Tables[4].Rows)
                                                        {
                                                            string vDate = item["vDate"]?.ToString();
                                                            string eDate = item["eDate"]?.ToString();
                                                            if (storeutcnow >= Convert.ToDateTime(vDate + " 00:00:00") && storeutcnow <= Convert.ToDateTime(eDate + " 23:59:59"))
                                                            {
                                                                isHoliday = true;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (isHoliday)
                                                    {
                                                        useday = HoliDay;
                                                    }
                                                    else
                                                    {
                                                        if (dayofweek == 0)
                                                        {
                                                            useday = SunDay;
                                                        }
                                                        else if (dayofweek == 1)
                                                        {
                                                            useday = monday;
                                                        }
                                                        else if (dayofweek == 2)
                                                        {
                                                            useday = tuesDay;
                                                        }
                                                        else if (dayofweek == 3)
                                                        {
                                                            useday = WednesDay;
                                                        }
                                                        else if (dayofweek == 4)
                                                        {
                                                            useday = ThursDay;
                                                        }
                                                        else if (dayofweek == 5)
                                                        {
                                                            useday = FriDay;
                                                        }
                                                        else if (dayofweek == 6)
                                                        {
                                                            useday = SaturDay;
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(useday))
                                                    {
                                                        string[] usedayarr = useday.Split('-');
                                                        string[] hourarr = usedayarr[1].Split('~');
                                                        if (usedayarr[0] == "Yes")
                                                        {
                                                            if (storeutcnow >= Convert.ToDateTime(storeutcnow.ToString("yyyy-MM-dd") + " " + hourarr[0]) && storeutcnow <= Convert.ToDateTime(storeutcnow.ToString("yyyy-MM-dd") + " " + hourarr[1]))
                                                            {
                                                                pointsredeem = true;
                                                            }
                                                            else
                                                            {
                                                                pointsredeem = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            pointsredeem = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        pointsredeem = false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                pointsredeem = true;
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    pointsredeem = false;
                                }
                            }
                            else
                            {
                                if(typecount == 1 && member == null)
                                {
                                    pointsredeem = true;
                                }
                            }

                            if (!pointsredeem)
                            {
                                paymentlist.RemoveAt(i);
                                i--;
                                continue;
                            }
                        }
                        //如果是emoney
                        else if (paytypeid.ToLower() == "6FE6D7B1-3C98-4853-81F5-5D59C736C76C".ToLower())
                        {
                            if(eMoneycount >= 1)
                            {
                                paymentlist.RemoveAt(i);
                                i--;
                                continue;
                            }
                            eMoneycount++;
                            paymentlist[i]["id"] = "bb93f7e4-10d2-461e-9157-0fa028417b6a"; //bb93f7e4-10d2-461e-9157-0fa028417b6a
                            paymentlist[i]["typeid"] = "b026e4c5-c122-4279-be80-a6052785d7d5"; //b026e4c5-c122-4279-be80-a6052785d7d5
                            paymentlist[i]["hid"] = hotelModel.Hid;
                            paymentlist[i]["balance"] = 0;
                            paymentlist[i]["isavailable"] = false;

                            if (member != null)
                            {
                                try
                                {
                                    paymentlist[i]["isavailable"] = true;
                                    paymentlist[i]["balance"] = Convert.ToDecimal(configdset.Tables[2].Rows[0]["balance"]) + Convert.ToDecimal(configdset.Tables[2].Rows[0]["free"]) - Convert.ToDecimal(configdset.Tables[2].Rows[0]["freezebalance"]) - Convert.ToDecimal(configdset.Tables[2].Rows[0]["freezebonus"]);
                                    paymentlist[i]["freezebalance"] = Convert.ToDecimal(configdset.Tables[2].Rows[0]["freezebalance"]) + Convert.ToDecimal(configdset.Tables[2].Rows[0]["freezebonus"]);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                    //if (member != null)
                    //{
                    //    JObject emoneyitem = new JObject();
                    //    emoneyitem["id"] = "bb93f7e4-10d2-461e-9157-0fa028417b6a"; //bb93f7e4-10d2-461e-9157-0fa028417b6a
                    //    emoneyitem["typeid"] = "b026e4c5-c122-4279-be80-a6052785d7d5"; //b026e4c5-c122-4279-be80-a6052785d7d5
                    //    emoneyitem["hid"] = hotelModel.Hid;
                    //    emoneyitem["adddate"] = DateTime.UtcNow;
                    //    emoneyitem["isdelete"] = 0;
                    //    emoneyitem["balance"] = 0;

                    //    if (configdset.Tables[2] != null && configdset.Tables[2].Rows.Count > 0)
                    //    {
                    //        try
                    //        {
                    //            emoneyitem["balance"] = Convert.ToDecimal(configdset.Tables[2].Rows[0]["balance"]) + Convert.ToDecimal(configdset.Tables[2].Rows[0]["free"]);
                    //        }
                    //        catch (Exception ex)
                    //        {

                    //        }
                    //    }
                    //    if (Convert.ToDecimal(emoneyitem["balance"]) > 0)
                    //    {
                    //        paymentlist.Insert(0, emoneyitem);
                    //    }
                    //}



                    #region 这里获取一下GlobalPayment的card storage

                    bool isglobalexist = false;
                    foreach (JObject item in paymentlist)
                    {
                        if (item["typeid"]?.ToString().ToLower() == "1800EBF9-D169-4D88-AD09-E29742F8B299".ToLower())
                        {
                            isglobalexist = true;
                            break;
                        }
                    }
                    //如果支付方式 包含globalpayment 就查找缓存的card
                    if (isglobalexist)
                    {
                        DataTable cardtable = null;
                        if (!string.IsNullOrEmpty(globalcustomerid) && string.IsNullOrEmpty(usertoken))
                        {
                            cardtable = ADOHelper.ExecSql(" select * from globalcustomerinfo where hid = @hotelid and status = 1 and customerid = @customerid and profileid is null order by createdate desc ", connstr, new List<SqlParameter>() {
                                new SqlParameter("@hotelid",hotelModel.Hid),
                                new SqlParameter("@customerid",globalcustomerid),
                            });
                        }
                        if (!string.IsNullOrEmpty(usertoken))
                        {
                            if (member != null)
                            {
                                if (string.IsNullOrEmpty(globalcustomerid))
                                {
                                    globalcustomerid = "";
                                }
                                cardtable = ADOHelper.ExecSql(" select * from globalcustomerinfo where hid = @hotelid and status = 1 and (profileid = @profileid " + (!string.IsNullOrEmpty(globalcustomerid) ? " or (customerid = @customerid and profileid is null) " : "") + ") order by createdate desc ", connstr, new List<SqlParameter>() {
                                    new SqlParameter("@hotelid",hotelModel.Hid),
                                    new SqlParameter("@profileid",member.Profileid),
                                    new SqlParameter("@customerid",globalcustomerid),
                                });
                            }
                        }
                        if (cardtable != null && cardtable.Rows.Count > 0)
                        {
                            JArray cardlist = JArray.FromObject(cardtable);
                            foreach (var item in cardlist)
                            {
                                if(item["carddigits"] != null && item["carddigits"]?.ToString().Trim() !="" && item["carddigits"].ToString().Length > 4)
                                {
                                    string carddigits = item["carddigits"].ToString().Trim();
                                    carddigits = "Ending in " + carddigits.Remove(0, carddigits.Length - 4);
                                    item["carddigits"] = carddigits;
                                }
                            }

                            foreach (var item in cardlist)
                            {
                                if(item["cvn"] != null && item["cvn"]?.ToString().Trim() !="")
                                {
                                    try
                                    {
                                        item["cvn"] = CryptHelper.AesEncrypt(item["cvn"]?.ToString());
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                            }
                            foreach (JObject item in paymentlist)
                            {
                                if (item["typeid"]?.ToString().ToLower() == "1800EBF9-D169-4D88-AD09-E29742F8B299".ToLower())
                                {
                                    item["cardlist"] = cardlist;
                                }
                            }
                        }
                    }
                    #endregion

                    jobj["paymentlist"] = paymentlist;
                }
            }
            catch (Exception)
            {
            }


            if (pickupobj != null)
            {
                JArray oldtimelist = new JArray();
                DateTime storetime = DateTime.UtcNow.AddHours(hotelModel.UtcTimeNum);
                #region 判断是否符合BlockTime
                if (jobj["blocktimelist"] != null && !string.IsNullOrWhiteSpace(jobj["blocktimelist"]?.ToString()))
                {
                    try
                    {
                        JArray specialdaylist = JArray.Parse(jobj["blocktimelist"]?.ToString());
                        foreach (var item in specialdaylist)
                        {
                            string startdate = item["date"]?.ToString();
                            string enddate = item["date"]?.ToString();

                            if (storetime >= Convert.ToDateTime(startdate + " 00:00:00") && storetime <= Convert.ToDateTime(enddate + " 23:59:59"))
                            {
                                string requestservices = item["requestservices"]?.ToString();
                                if (string.IsNullOrWhiteSpace(requestservices) || requestservices == "[]" || requestservices.Contains("pickup"))
                                {
                                    JArray closehourlist = JArray.FromObject(item["closehourlist"]);
                                    foreach (JObject closehouritem in closehourlist)
                                    {
                                        string starthour = closehouritem["starthour"]?.ToString();
                                        string endhour = closehouritem["endhour"]?.ToString();
                                        if (storetime >= Convert.ToDateTime(startdate + " " + starthour) && storetime <= Convert.ToDateTime(enddate + " " + endhour))
                                        {
                                            pickupobj["enableimmediateorder"] = false;
                                            pickupobj["pickupispausetipsmessage"] = item["notes"]?.ToString();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                }
                #endregion
                try
                {
                    pickupobj["inbusinesstime"] = true; //目前是否在营业时间
                    JArray timelist = new JArray(); //营业时间
                                                    //先查找pickupsetting是否设置了营业时间，没有的话就使用sytem默认的时间
                    if (pickupobj["timelist"] != null && pickupobj["timelist"].ToString().Trim() != "")
                    {
                        timelist = JArray.Parse(pickupobj["timelist"].ToString());
                    }
                    if ((timelist == null || timelist.Count <= 0) && jobj["timelist"] != null && jobj["timelist"].ToString().Trim() != "")
                    {
                        timelist = JArray.Parse(jobj["timelist"].ToString());
                    }
                    if (timelist.Count <= 0)
                    {
                        //说明没有限制 手动添加7天的
                        for (int i = 0; i < 7; i++)
                        {
                            JObject timejobj = new JObject();
                            timejobj["dayforweek"] = i;
                            timejobj["starttime"] = "00:00";
                            timejobj["endtime"] = "23:59";
                            timejobj["wholeday"] = true;
                            timelist.Add(timejobj);
                        }
                    }


                    #region 判断是否符合SpecialDay
                    if (jobj["specialdaylist"] != null && !string.IsNullOrWhiteSpace(jobj["specialdaylist"]?.ToString()))
                    {
                        try
                        {
                            JArray specialdaylist = JArray.Parse(jobj["specialdaylist"]?.ToString());
                            foreach (var item in specialdaylist)
                            {
                                JArray datebetween = JArray.FromObject(item["datebetween"]);
                                string startdate = datebetween[0].ToString();
                                string enddate = datebetween[1].ToString();

                                DateTime maxenddatetime = Convert.ToDateTime(enddate + " 23:59:59");
                                bool betisopen = Convert.ToBoolean(item["isopen"]);
                                if (betisopen)
                                {
                                    JArray openhourlist = JArray.FromObject(item["openhourlist"]);
                                    foreach (JObject openhouritem in openhourlist)
                                    {
                                        if (Convert.ToDateTime(startdate + " " + openhouritem["starthour"]?.ToString()) > Convert.ToDateTime(startdate + " " + openhouritem["endhour"]?.ToString()))
                                        {
                                            //说明是跨天
                                            if (Convert.ToDateTime(enddate + " " + openhouritem["endhour"]?.ToString()).AddDays(1) > maxenddatetime)
                                            {
                                                maxenddatetime = Convert.ToDateTime(enddate + " " + openhouritem["endhour"]?.ToString()).AddDays(1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    maxenddatetime = Convert.ToDateTime(enddate + " 23:59:59");
                                }
                                foreach (JObject timeitem in timelist)
                                {
                                    int dayforweek = timeitem["dayforweek"] == null ? -1 : Convert.ToInt32(timeitem["dayforweek"]);
                                    if (dayforweek == 7)
                                    {
                                        dayforweek = 0;
                                    }
                                    if (dayforweek == (int)maxenddatetime.DayOfWeek)
                                    {
                                        if (Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd") + " " + timeitem["starttime"]) > Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd") + " " + timeitem["endtime"]))
                                        {
                                            maxenddatetime = Convert.ToDateTime(maxenddatetime.ToString("yyyy-MM-dd") + " " + timeitem["endtime"]).AddDays(1);
                                        }
                                    }
                                }


                                if (storetime >= Convert.ToDateTime(startdate + " 00:00:00") && storetime <= maxenddatetime)
                                {
                                    string requestservices = item["requestservices"]?.ToString();
                                    if (string.IsNullOrWhiteSpace(requestservices) || requestservices == "[]" || requestservices.Contains("pickup"))
                                    {
                                        bool isopen = Convert.ToBoolean(item["isopen"]);
                                        if (!isopen)
                                        {
                                            pickupobj["enableimmediateorder"] = false;
                                            pickupobj["pickupispausetipsmessage"] = item["notes"]?.ToString();
                                            break;
                                        }
                                        else
                                        {
                                            JArray openhourlist = JArray.FromObject(item["openhourlist"]);
                                            foreach (JObject openhouritem in openhourlist)
                                            {
                                                JObject timejobj = new JObject();
                                                timejobj["dayforweek"] = (int)storetime.Date.DayOfWeek;

                                                if (storetime > Convert.ToDateTime(enddate + " 23:59:59"))
                                                {
                                                    timejobj["dayforweek"] = (int)storetime.AddDays(-1).Date.DayOfWeek;
                                                }
                                                else if (storetime.Date.ToString("yyyy-MM-dd") != startdate && Convert.ToDateTime(storetime.Date.ToString("yyyy-MM-dd") + " " + openhouritem["starthour"]?.ToString()) > Convert.ToDateTime(storetime.Date.ToString("yyyy-MM-dd") + " " + openhouritem["endhour"]?.ToString()))
                                                {
                                                    timejobj["dayforweek"] = (int)storetime.AddDays(-1).Date.DayOfWeek;
                                                }

                                                timejobj["starttime"] = openhouritem["starthour"]?.ToString();
                                                timejobj["endtime"] = openhouritem["endhour"]?.ToString();
                                                timejobj["wholeday"] = false;
                                                oldtimelist.Add(timejobj);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                    #endregion

                    if (oldtimelist.Count > 0)
                    {
                        timelist = new JArray();
                        pickupobj["firstorderoffset"] = 0;
                        pickupobj["lastorderoffset"] = 0;

                        pickupobj["latertimefirstorderoffset"] = 0;
                        pickupobj["latertimelastorderoffset"] = 0;
                        foreach (var item in oldtimelist)
                        {
                            timelist.Add(item);
                        }
                    }

                    #region 获取一周7天 openhour显示的内容


                    JArray sevenopenhour = new JArray();
                    for (int i = 1; i <= 7; i++)
                    {
                        JObject dayobj = new JObject();
                        try
                        {
                            string todaytitle = "";
                            string todayopenhourtext = "";
                            int dayforweek = (i == 7 ? 0 : i);
                            switch (dayforweek)
                            {
                                case 1:
                                    todaytitle = "Monday";
                                    break;
                                case 2:
                                    todaytitle = "Tuesday";
                                    break;
                                case 3:
                                    todaytitle = "Wednesday";
                                    break;
                                case 4:
                                    todaytitle = "Thursday";
                                    break;
                                case 5:
                                    todaytitle = "Friday";
                                    break;
                                case 6:
                                    todaytitle = "Saturday";
                                    break;
                                case 0:
                                    todaytitle = "Sunday";
                                    break;
                                default:
                                    break;
                            }
                            dayobj["todaytitle"] = todaytitle;
                            dayobj["showopenhour"] = true;
                            string timeliststr = jobj["timelist"]?.ToString();
                            if (!string.IsNullOrEmpty(timeliststr))
                            {
                                if (timelist != null && timelist.Count > 0)
                                {
                                    foreach (JObject timeitem in timelist)
                                    {
                                        if (timeitem["dayforweek"] != null)
                                        {
                                            int timedayforweek = Convert.ToInt32(timeitem["dayforweek"]);
                                            if (timedayforweek == 7)
                                            {
                                                timedayforweek = 0;
                                            }
                                            if (timedayforweek == dayforweek)
                                            {
                                                bool wholeday = Convert.ToBoolean(timeitem["wholeday"]);
                                                if (wholeday)
                                                {
                                                    todayopenhourtext = "Open 24 Hours";
                                                    break;
                                                }
                                                else
                                                {
                                                    string starttime = timeitem["starttime"]?.ToString();
                                                    string endtime = timeitem["endtime"]?.ToString();

                                                    if (!string.IsNullOrEmpty(todayopenhourtext))
                                                    {
                                                        todayopenhourtext += " , ";
                                                    }
                                                    todayopenhourtext += starttime + " ~ " + endtime;
                                                }
                                            }
                                        }
                                    }
                                    if (string.IsNullOrWhiteSpace(todayopenhourtext))
                                    {
                                        dayobj["todayopenhourtext"] = "Close";
                                    }
                                    else
                                    {
                                        dayobj["todayopenhourtext"] = todayopenhourtext;
                                    }
                                }
                                else
                                {
                                    dayobj["todayopenhourtext"] = "Open 24 Hours";
                                }
                            }
                            else
                            {
                                dayobj["todayopenhourtext"] = "Open 24 Hours";
                            }
                        }
                        catch (Exception) { }
                        sevenopenhour.Add(dayobj);
                    }

                    var isrepeat = true;

                    string openhourtext = "";
                    foreach (var item in sevenopenhour)
                    {
                        if (string.IsNullOrWhiteSpace(openhourtext))
                        {
                            openhourtext = item["todayopenhourtext"]?.ToString();
                        }
                        else
                        {
                            if (openhourtext != item["todayopenhourtext"]?.ToString())
                            {
                                isrepeat = false;
                            }
                        }
                    }
                    if (isrepeat)
                    {
                        JObject sevenobj = JObject.FromObject(sevenopenhour[0]);
                        sevenobj["todaytitle"] = "Monday - Sunday";
                        sevenopenhour = new JArray();
                        sevenopenhour.Add(sevenobj);
                    }

                    pickupobj["sevenopenhour"] = sevenopenhour;
                    #endregion



                    int storetimeADayOfWeek = Convert.ToInt32(storetime.DayOfWeek);
                    if (storetimeADayOfWeek != 0)
                    {
                        storetimeADayOfWeek = storetimeADayOfWeek - 1;
                    }
                    else
                    {
                        storetimeADayOfWeek = 6;
                    }






                    bool isinbusinesstime = false;
                    foreach (JObject item in timelist)
                    {
                        bool isacrossday = false;
                        //先判断一下是否是跨一天时间段,如果开始时间大于结束时间，认为是跨天营业
                        if (Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00") > Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59"))
                        {
                            isacrossday = true;
                        }

                        int dayforweek = item["dayforweek"] == null ? -1 : Convert.ToInt32(item["dayforweek"]);
                        if(dayforweek == 7)
                        {
                            dayforweek = 0;
                        }
                        if (dayforweek == Convert.ToInt32(storetime.DayOfWeek) || (isacrossday && dayforweek == storetimeADayOfWeek))
                        {
                            bool wholeday = Convert.ToBoolean(item["wholeday"]);
                            if (wholeday)
                            {
                                isinbusinesstime = true;
                                break;
                            }
                            if (!isacrossday)
                            {
                                if (storetime >= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"]).AddMinutes(Convert.ToInt32(pickupobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"]).AddMinutes(-Convert.ToInt32(pickupobj["lastorderoffset"])))
                                {
                                    isinbusinesstime = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (dayforweek == storetimeADayOfWeek)
                                {
                                    if (storetime >= Convert.ToDateTime(storetime.AddDays(-1).ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00").AddMinutes(Convert.ToInt32(pickupobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59").AddMinutes(-Convert.ToInt32(pickupobj["lastorderoffset"])))
                                    {
                                        isinbusinesstime = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (storetime >= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00").AddMinutes(Convert.ToInt32(pickupobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.AddDays(1).ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59").AddMinutes(-Convert.ToInt32(pickupobj["lastorderoffset"])))
                                    {
                                        isinbusinesstime = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    pickupobj["inbusinesstime"] = isinbusinesstime;

                    if (getpickuplatertime && pickupobj["enablelaterorder"] != null && Convert.ToBoolean(pickupobj["enablelaterorder"]))
                    {
                        string latertimelist= GetPickupSelectLaterTime(hid, true);
                        if (!string.IsNullOrWhiteSpace(latertimelist))
                        {
                            JArray timearray = JArray.Parse(latertimelist);
                            if(timearray == null || timearray.Count <= 0)
                            {
                                pickupobj["enablelaterorder"] = false;
                            }
                            else
                            {
                                var isbusy = true;
                                foreach (JObject timeitem in timearray)
                                {
                                    if(timeitem["closed"] != null && Convert.ToBoolean(timeitem["closed"]))
                                    {
                                        continue;
                                    }
                                    if(timeitem["value"]== null)
                                    {
                                        continue;
                                    }
                                    JArray latertimevalue = JArray.FromObject(timeitem["value"]);
                                    foreach (JObject item in latertimevalue)
                                    {
                                        if (item["isbusy"] != null && !Convert.ToBoolean(item["isbusy"]))
                                        {
                                            isbusy = false;
                                            break;
                                        }
                                    }
                                    if (!isbusy)
                                    {
                                        break;
                                    }
                                }
                                if (isbusy)
                                {
                                    pickupobj["enablelaterorder"] = false;
                                }
                            }
                        }
                        else
                        {
                            pickupobj["enablelaterorder"] = false;
                        }
                    }

                    //如果在营业时间，判断一下 now订单是否可用
                    if (isinbusinesstime && getpickuplatertime && pickupobj["enableimmediateorder"] != null && Convert.ToBoolean(pickupobj["enableimmediateorder"]))
                    {
                        try
                        {
                            JObject timeobj = JObject.Parse(GetPickupNowTime(hid, null));
                            if(timeobj == null || timeobj["scheduledtime"] == null)
                            {
                                pickupobj["enableimmediateorder"] = false;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                }
                catch (Exception)
                {
                    pickupobj["inbusinesstime"] = false;
                }
                if (pickupobj["pricelevel"] == null)
                {
                    pickupobj["pricelevel"] = 1;
                }
                if (pickupobj["autoclearcarthour"] == null)
                {
                    pickupobj["autoclearcarthour"] = 3;
                }
            }
            jobj["pickupconfig"] = pickupobj;


            if (deliveryobj != null)
            {
                JArray oldtimelist = new JArray();
                DateTime storetime = DateTime.UtcNow.AddHours(hotelModel.UtcTimeNum);
                
                #region 判断是否符合BlockTime
                if (jobj["blocktimelist"] != null && !string.IsNullOrWhiteSpace(jobj["blocktimelist"]?.ToString()))
                {
                    try
                    {
                        JArray specialdaylist = JArray.Parse(jobj["blocktimelist"]?.ToString());
                        foreach (var item in specialdaylist)
                        {
                            string startdate = item["date"]?.ToString();
                            string enddate = item["date"]?.ToString();

                            if (storetime >= Convert.ToDateTime(startdate + " 00:00:00") && storetime <= Convert.ToDateTime(enddate + " 23:59:59"))
                            {
                                string requestservices = item["requestservices"]?.ToString();
                                if (string.IsNullOrWhiteSpace(requestservices) || requestservices == "[]" || requestservices.Contains("delivery"))
                                {
                                    JArray closehourlist = JArray.FromObject(item["closehourlist"]);
                                    foreach (JObject closehouritem in closehourlist)
                                    {
                                        string starthour = closehouritem["starthour"]?.ToString();
                                        string endhour = closehouritem["endhour"]?.ToString();
                                        if (storetime >= Convert.ToDateTime(startdate + " " + starthour) && storetime <= Convert.ToDateTime(enddate + " " + endhour))
                                        {
                                            deliveryobj["enableimmediateorder"] = false;
                                            deliveryobj["pickupispausetipsmessage"] = item["notes"]?.ToString();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                }
                #endregion
                try
                {
                    if(deliveryobj["enable3rd"] != null && deliveryobj["enable3rd"]?.ToString() != "none")
                    {
                        deliveryobj["submithaveemail"] = true;
                    }
                    
                    //foodhwy 不支持laterorder
                    if (deliveryobj["enable3rd"] != null && deliveryobj["enable3rd"]?.ToString() == "foodhwy")
                    {
                        deliveryobj["enablelaterorder"] = false;
                    }

                    if(deliveryobj["doordashordertipdisplayname"] == null)
                    {
                        deliveryobj["doordashordertipdisplayname"] = "Tip the Driver";
                    }

                    deliveryobj["inbusinesstime"] = true; //目前是否在营业时间
                    JArray timelist = new JArray(); //营业时间
                                                    //先查找pickupsetting是否设置了营业时间，没有的话就使用sytem默认的时间
                    if (deliveryobj["timelist"] != null && deliveryobj["timelist"].ToString().Trim() != "")
                    {
                        timelist = JArray.Parse(deliveryobj["timelist"].ToString());
                    }
                    if ((timelist == null || timelist.Count <= 0) && jobj["timelist"] != null && jobj["timelist"].ToString().Trim() != "")
                    {
                        timelist = JArray.Parse(jobj["timelist"].ToString());
                    }
                    if (timelist.Count <= 0)
                    {
                        //说明没有限制 手动添加7天的
                        for (int i = 0; i < 7; i++)
                        {
                            JObject timejobj = new JObject();
                            timejobj["dayforweek"] = i;
                            timejobj["starttime"] = "00:00";
                            timejobj["endtime"] = "23:59";
                            timejobj["wholeday"] = true;
                            timelist.Add(timejobj);
                        }
                    }
                    #region 判断是否符合SpecialDay
                    if (jobj["specialdaylist"] != null && !string.IsNullOrWhiteSpace(jobj["specialdaylist"]?.ToString()))
                    {
                        try
                        {
                            JArray specialdaylist = JArray.Parse(jobj["specialdaylist"]?.ToString());
                            foreach (var item in specialdaylist)
                            {
                                JArray datebetween = JArray.FromObject(item["datebetween"]);
                                string startdate = datebetween[0].ToString();
                                string enddate = datebetween[1].ToString();

                                DateTime maxenddatetime = Convert.ToDateTime(enddate + " 23:59:59");
                                bool betisopen = Convert.ToBoolean(item["isopen"]);
                                if (betisopen)
                                {
                                    JArray openhourlist = JArray.FromObject(item["openhourlist"]);
                                    foreach (JObject openhouritem in openhourlist)
                                    {
                                        if (Convert.ToDateTime(startdate + " " + openhouritem["starthour"]?.ToString()) > Convert.ToDateTime(startdate + " " + openhouritem["endhour"]?.ToString()))
                                        {
                                            //说明是跨天
                                            if (Convert.ToDateTime(enddate + " " + openhouritem["endhour"]?.ToString()).AddDays(1) > maxenddatetime)
                                            {
                                                maxenddatetime = Convert.ToDateTime(enddate + " " + openhouritem["endhour"]?.ToString()).AddDays(1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    maxenddatetime = Convert.ToDateTime(enddate + " 23:59:59");
                                }
                                foreach (JObject timeitem in timelist)
                                {
                                    int dayforweek = timeitem["dayforweek"] == null ? -1 : Convert.ToInt32(timeitem["dayforweek"]);
                                    if (dayforweek == 7)
                                    {
                                        dayforweek = 0;
                                    }
                                    if (dayforweek == (int)maxenddatetime.DayOfWeek)
                                    {
                                        if (Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd") + " " + timeitem["starttime"]) > Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd") + " " + timeitem["endtime"]))
                                        {
                                            maxenddatetime = Convert.ToDateTime(maxenddatetime.ToString("yyyy-MM-dd") + " " + timeitem["endtime"]).AddDays(1);
                                        }
                                    }
                                }

                                if (storetime >= Convert.ToDateTime(startdate + " 00:00:00") && storetime <= maxenddatetime)
                                {
                                    string requestservices = item["requestservices"]?.ToString();
                                    if (string.IsNullOrWhiteSpace(requestservices) || requestservices == "[]" || requestservices.Contains("delivery"))
                                    {
                                        bool isopen = Convert.ToBoolean(item["isopen"]);
                                        if (!isopen)
                                        {
                                            deliveryobj["enableimmediateorder"] = false;
                                            deliveryobj["deliveryispausetipsmessage"] = item["notes"]?.ToString();
                                            break;
                                        }
                                        else
                                        {
                                            JArray openhourlist = JArray.FromObject(item["openhourlist"]);
                                            foreach (JObject openhouritem in openhourlist)
                                            {
                                                JObject timejobj = new JObject();
                                                timejobj["dayforweek"] = (int)storetime.Date.DayOfWeek;

                                                if (storetime > Convert.ToDateTime(enddate + " 23:59:59"))
                                                {
                                                    timejobj["dayforweek"] = (int)storetime.AddDays(-1).Date.DayOfWeek;
                                                }
                                                else if (storetime.Date.ToString("yyyy-MM-dd") != startdate && Convert.ToDateTime(storetime.Date.ToString("yyyy-MM-dd") + " " + openhouritem["starthour"]?.ToString()) > Convert.ToDateTime(storetime.Date.ToString("yyyy-MM-dd") + " " + openhouritem["endhour"]?.ToString()))
                                                {
                                                    timejobj["dayforweek"] = (int)storetime.AddDays(-1).Date.DayOfWeek;
                                                }

                                                timejobj["starttime"] = openhouritem["starthour"]?.ToString();
                                                timejobj["endtime"] = openhouritem["endhour"]?.ToString();
                                                timejobj["wholeday"] = false;
                                                oldtimelist.Add(timejobj);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                    #endregion
                    if (oldtimelist.Count > 0)
                    {
                        timelist = new JArray();
                        deliveryobj["firstorderoffset"] = 0;
                        deliveryobj["lastorderoffset"] = 0;

                        deliveryobj["latertimefirstorderoffset"] = 0;
                        deliveryobj["latertimelastorderoffset"] = 0;
                        foreach (var item in oldtimelist)
                        {
                            timelist.Add(item);
                        }
                    }


                    #region 获取一周7天 openhour显示的内容


                    JArray sevenopenhour = new JArray();
                    for (int i = 1; i <= 7; i++)
                    {
                        JObject dayobj = new JObject();
                        try
                        {
                            string todaytitle = "";
                            string todayopenhourtext = "";
                            int dayforweek = (i == 7 ? 0 : i);
                            switch (dayforweek)
                            {
                                case 1:
                                    todaytitle = "Monday";
                                    break;
                                case 2:
                                    todaytitle = "Tuesday";
                                    break;
                                case 3:
                                    todaytitle = "Wednesday";
                                    break;
                                case 4:
                                    todaytitle = "Thursday";
                                    break;
                                case 5:
                                    todaytitle = "Friday";
                                    break;
                                case 6:
                                    todaytitle = "Saturday";
                                    break;
                                case 0:
                                    todaytitle = "Sunday";
                                    break;
                                default:
                                    break;
                            }
                            dayobj["todaytitle"] = todaytitle;
                            dayobj["showopenhour"] = true;
                            string timeliststr = jobj["timelist"]?.ToString();
                            if (!string.IsNullOrEmpty(timeliststr))
                            {
                                if (timelist != null && timelist.Count > 0)
                                {
                                    foreach (JObject timeitem in timelist)
                                    {
                                        if (timeitem["dayforweek"] != null)
                                        {
                                            int timedayforweek = Convert.ToInt32(timeitem["dayforweek"]);
                                            if (timedayforweek == 7)
                                            {
                                                timedayforweek = 0;
                                            }
                                            if (timedayforweek == dayforweek)
                                            {
                                                bool wholeday = Convert.ToBoolean(timeitem["wholeday"]);
                                                if (wholeday)
                                                {
                                                    todayopenhourtext = "Open 24 Hours";
                                                    break;
                                                }
                                                else
                                                {
                                                    string starttime = timeitem["starttime"]?.ToString();
                                                    string endtime = timeitem["endtime"]?.ToString();

                                                    if (!string.IsNullOrEmpty(todayopenhourtext))
                                                    {
                                                        todayopenhourtext += " , ";
                                                    }
                                                    todayopenhourtext += starttime + " ~ " + endtime;
                                                }
                                            }
                                        }
                                    }
                                    if (string.IsNullOrWhiteSpace(todayopenhourtext))
                                    {
                                        dayobj["todayopenhourtext"] = "Close";
                                    }
                                    else
                                    {
                                        dayobj["todayopenhourtext"] = todayopenhourtext;
                                    }
                                }
                                else
                                {
                                    dayobj["todayopenhourtext"] = "Open 24 Hours";
                                }
                            }
                            else
                            {
                                dayobj["todayopenhourtext"] = "Open 24 Hours";
                            }
                        }
                        catch (Exception) { }
                        sevenopenhour.Add(dayobj);
                    }

                    var isrepeat = true;

                    string openhourtext = "";
                    foreach (var item in sevenopenhour)
                    {
                        if (string.IsNullOrWhiteSpace(openhourtext))
                        {
                            openhourtext = item["todayopenhourtext"]?.ToString();
                        }
                        else
                        {
                            if (openhourtext != item["todayopenhourtext"]?.ToString())
                            {
                                isrepeat = false;
                            }
                        }
                    }
                    if (isrepeat)
                    {
                        JObject sevenobj = JObject.FromObject(sevenopenhour[0]);
                        sevenobj["todaytitle"] = "Monday - Sunday";
                        sevenopenhour = new JArray();
                        sevenopenhour.Add(sevenobj);
                    }

                    deliveryobj["sevenopenhour"] = sevenopenhour;
                    #endregion


                    int storetimeADayOfWeek = Convert.ToInt32(storetime.DayOfWeek);
                    if (storetimeADayOfWeek != 0)
                    {
                        storetimeADayOfWeek = storetimeADayOfWeek - 1;
                    }
                    else
                    {
                        storetimeADayOfWeek = 6;
                    }


                    bool isinbusinesstime = false;
                    foreach (JObject item in timelist)
                    {
                        bool isacrossday = false;
                        //先判断一下是否是跨一天时间段,如果开始时间大于结束时间，认为是跨天营业
                        if (Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00") > Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59"))
                        {
                            isacrossday = true;
                        }

                        int dayforweek = item["dayforweek"] == null ? -1 : Convert.ToInt32(item["dayforweek"]);
                        if (dayforweek == 7)
                        {
                            dayforweek = 0;
                        }
                        if (dayforweek == Convert.ToInt32(storetime.DayOfWeek) || (isacrossday && dayforweek == storetimeADayOfWeek))
                        {
                            bool wholeday = Convert.ToBoolean(item["wholeday"]);
                            if (wholeday)
                            {
                                isinbusinesstime = true;
                                break;
                            }
                            //if (storetime >= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"]).AddMinutes(Convert.ToInt32(deliveryobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"]).AddMinutes(-Convert.ToInt32(deliveryobj["lastorderoffset"])))
                            //{
                            //    isinbusinesstime = true;
                            //    break;
                            //}



                            if (!isacrossday)
                            {
                                if (storetime >= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"]).AddMinutes(Convert.ToInt32(deliveryobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"]).AddMinutes(-Convert.ToInt32(deliveryobj["lastorderoffset"])))
                                {
                                    isinbusinesstime = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (dayforweek == storetimeADayOfWeek)
                                {
                                    if (storetime >= Convert.ToDateTime(storetime.AddDays(-1).ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00").AddMinutes(Convert.ToInt32(deliveryobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59").AddMinutes(-Convert.ToInt32(deliveryobj["lastorderoffset"])))
                                    {
                                        isinbusinesstime = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (storetime >= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00").AddMinutes(Convert.ToInt32(deliveryobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.AddDays(1).ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59").AddMinutes(-Convert.ToInt32(deliveryobj["lastorderoffset"])))
                                    {
                                        isinbusinesstime = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    deliveryobj["inbusinesstime"] = isinbusinesstime;

                    if (getpickuplatertime && deliveryobj["enablelaterorder"] != null && Convert.ToBoolean(deliveryobj["enablelaterorder"]))
                    {
                        string latertimelist = GetPickupSelectLaterTime(hid, true,true);
                        if (!string.IsNullOrWhiteSpace(latertimelist))
                        {
                            JArray timearray = JArray.Parse(latertimelist);
                            if (timearray == null || timearray.Count <= 0)
                            {
                                deliveryobj["enablelaterorder"] = false;
                            }
                            else
                            {
                                var isbusy = true;
                                foreach (JObject timeitem in timearray)
                                {
                                    if (timeitem["closed"] != null && Convert.ToBoolean(timeitem["closed"]))
                                    {
                                        continue;
                                    }
                                    if (timeitem["value"] == null)
                                    {
                                        continue;
                                    }
                                    JArray latertimevalue = JArray.FromObject(timeitem["value"]);
                                    foreach (JObject item in latertimevalue)
                                    {
                                        if (item["isbusy"] != null && !Convert.ToBoolean(item["isbusy"]))
                                        {
                                            isbusy = false;
                                            break;
                                        }
                                    }
                                    if (!isbusy)
                                    {
                                        break;
                                    }
                                }
                                if (isbusy)
                                {
                                    deliveryobj["enablelaterorder"] = false;
                                }
                            }
                        }
                        else
                        {
                            deliveryobj["enablelaterorder"] = false;
                        }
                    }

                    //如果在营业时间，判断一下 now订单是否可用
                    if (isinbusinesstime && getpickuplatertime && deliveryobj["enableimmediateorder"] != null && Convert.ToBoolean(deliveryobj["enableimmediateorder"]))
                    {
                        try
                        {
                            JObject timeobj = JObject.Parse(GetPickupNowTime(hid, null, true, cuslocation));
                            if (timeobj == null || timeobj["scheduledtime"] == null)
                            {
                                if (!string.IsNullOrWhiteSpace(cuslocation))
                                {
                                    deliveryobj["enableimmediateorder"] = false;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                }
                catch (Exception)
                {
                    deliveryobj["inbusinesstime"] = false;
                }
                if (deliveryobj["pricelevel"] == null)
                {
                    deliveryobj["pricelevel"] = 1;
                }
                if (deliveryobj["autoclearcarthour"] == null)
                {
                    deliveryobj["autoclearcarthour"] = 3;
                }
            }
            jobj["deliveryconfig"] = deliveryobj;



            if (kioskobj != null)
            {
                JArray oldtimelist = new JArray();
                DateTime storetime = DateTime.UtcNow.AddHours(hotelModel.UtcTimeNum);
                #region 判断是否符合BlockTime
                if (jobj["blocktimelist"] != null && !string.IsNullOrWhiteSpace(jobj["blocktimelist"]?.ToString()))
                {
                    try
                    {
                        JArray specialdaylist = JArray.Parse(jobj["blocktimelist"]?.ToString());
                        foreach (var item in specialdaylist)
                        {
                            string startdate = item["date"]?.ToString();
                            string enddate = item["date"]?.ToString();

                            if (storetime >= Convert.ToDateTime(startdate + " 00:00:00") && storetime <= Convert.ToDateTime(enddate + " 23:59:59"))
                            {
                                string requestservices = item["requestservices"]?.ToString();
                                if (string.IsNullOrWhiteSpace(requestservices) || requestservices == "[]" || requestservices.Contains("pickup"))
                                {
                                    JArray closehourlist = JArray.FromObject(item["closehourlist"]);
                                    foreach (JObject closehouritem in closehourlist)
                                    {
                                        string starthour = closehouritem["starthour"]?.ToString();
                                        string endhour = closehouritem["endhour"]?.ToString();
                                        if (storetime >= Convert.ToDateTime(startdate + " " + starthour) && storetime <= Convert.ToDateTime(enddate + " " + endhour))
                                        {
                                            kioskobj["ispausetipsmessage"] = item["notes"]?.ToString();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                }
                #endregion


                if (string.IsNullOrWhiteSpace(kioskobj["canbindmember"]?.ToString()))
                {
                    kioskobj["canbindmember"] = true;
                }

                if (hotelproduct == null)
                {
                    kioskobj["canbindmember"] = false;
                }

                try
                {
                    kioskobj["inbusinesstime"] = true; //目前是否在营业时间
                    JArray timelist = new JArray(); //营业时间
                                                    //先查找pickupsetting是否设置了营业时间，没有的话就使用sytem默认的时间
                    if (kioskobj["timelist"] != null && kioskobj["timelist"].ToString().Trim() != "")
                    {
                        timelist = JArray.Parse(kioskobj["timelist"].ToString());
                    }
                    if ((timelist == null || timelist.Count <= 0) && jobj["timelist"] != null && jobj["timelist"].ToString().Trim() != "")
                    {
                        timelist = JArray.Parse(jobj["timelist"].ToString());
                    }
                    if (timelist.Count <= 0)
                    {
                        //说明没有限制 手动添加7天的
                        for (int i = 0; i < 7; i++)
                        {
                            JObject timejobj = new JObject();
                            timejobj["dayforweek"] = i;
                            timejobj["starttime"] = "00:00";
                            timejobj["endtime"] = "23:59";
                            timejobj["wholeday"] = true;
                            timelist.Add(timejobj);
                        }
                    }


                    #region 判断是否符合SpecialDay
                    if (jobj["specialdaylist"] != null && !string.IsNullOrWhiteSpace(jobj["specialdaylist"]?.ToString()))
                    {
                        try
                        {
                            JArray specialdaylist = JArray.Parse(jobj["specialdaylist"]?.ToString());
                            foreach (var item in specialdaylist)
                            {
                                JArray datebetween = JArray.FromObject(item["datebetween"]);
                                string startdate = datebetween[0].ToString();
                                string enddate = datebetween[1].ToString();

                                DateTime maxenddatetime = Convert.ToDateTime(enddate + " 23:59:59");
                                bool betisopen = Convert.ToBoolean(item["isopen"]);
                                if (betisopen)
                                {
                                    JArray openhourlist = JArray.FromObject(item["openhourlist"]);
                                    foreach (JObject openhouritem in openhourlist)
                                    {
                                        if (Convert.ToDateTime(startdate + " " + openhouritem["starthour"]?.ToString()) > Convert.ToDateTime(startdate + " " + openhouritem["endhour"]?.ToString()))
                                        {
                                            //说明是跨天
                                            if (Convert.ToDateTime(enddate + " " + openhouritem["endhour"]?.ToString()).AddDays(1) > maxenddatetime)
                                            {
                                                maxenddatetime = Convert.ToDateTime(enddate + " " + openhouritem["endhour"]?.ToString()).AddDays(1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    maxenddatetime = Convert.ToDateTime(enddate + " 23:59:59");
                                }
                                foreach (JObject timeitem in timelist)
                                {
                                    int dayforweek = timeitem["dayforweek"] == null ? -1 : Convert.ToInt32(timeitem["dayforweek"]);
                                    if (dayforweek == 7)
                                    {
                                        dayforweek = 0;
                                    }
                                    if (dayforweek == (int)maxenddatetime.DayOfWeek)
                                    {
                                        if (Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd") + " " + timeitem["starttime"]) > Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd") + " " + timeitem["endtime"]))
                                        {
                                            maxenddatetime = Convert.ToDateTime(maxenddatetime.ToString("yyyy-MM-dd") + " " + timeitem["endtime"]).AddDays(1);
                                        }
                                    }
                                }


                                if (storetime >= Convert.ToDateTime(startdate + " 00:00:00") && storetime <= maxenddatetime)
                                {
                                    string requestservices = item["requestservices"]?.ToString();
                                    if (string.IsNullOrWhiteSpace(requestservices) || requestservices == "[]" || requestservices.Contains("pickup"))
                                    {
                                        bool isopen = Convert.ToBoolean(item["isopen"]);
                                        if (!isopen)
                                        {
                                            kioskobj["ispausetipsmessage"] = item["notes"]?.ToString();
                                            break;
                                        }
                                        else
                                        {
                                            JArray openhourlist = JArray.FromObject(item["openhourlist"]);
                                            foreach (JObject openhouritem in openhourlist)
                                            {
                                                JObject timejobj = new JObject();
                                                timejobj["dayforweek"] = (int)storetime.Date.DayOfWeek;

                                                if (storetime > Convert.ToDateTime(enddate + " 23:59:59"))
                                                {
                                                    timejobj["dayforweek"] = (int)storetime.AddDays(-1).Date.DayOfWeek;
                                                }
                                                else if (storetime.Date.ToString("yyyy-MM-dd") != startdate && Convert.ToDateTime(storetime.Date.ToString("yyyy-MM-dd") + " " + openhouritem["starthour"]?.ToString()) > Convert.ToDateTime(storetime.Date.ToString("yyyy-MM-dd") + " " + openhouritem["endhour"]?.ToString()))
                                                {
                                                    timejobj["dayforweek"] = (int)storetime.AddDays(-1).Date.DayOfWeek;
                                                }

                                                timejobj["starttime"] = openhouritem["starthour"]?.ToString();
                                                timejobj["endtime"] = openhouritem["endhour"]?.ToString();
                                                timejobj["wholeday"] = false;
                                                oldtimelist.Add(timejobj);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                    #endregion

                    if (oldtimelist.Count > 0)
                    {
                        timelist = new JArray();
                        kioskobj["firstorderoffset"] = 0;
                        kioskobj["lastorderoffset"] = 0;
                        foreach (var item in oldtimelist)
                        {
                            timelist.Add(item);
                        }
                    }

                    int storetimeADayOfWeek = Convert.ToInt32(storetime.DayOfWeek);
                    if (storetimeADayOfWeek != 0)
                    {
                        storetimeADayOfWeek = storetimeADayOfWeek - 1;
                    }
                    else
                    {
                        storetimeADayOfWeek = 6;
                    }


                    bool isinbusinesstime = false;
                    foreach (JObject item in timelist)
                    {
                        bool isacrossday = false;
                        //先判断一下是否是跨一天时间段,如果开始时间大于结束时间，认为是跨天营业
                        if (Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00") > Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59"))
                        {
                            isacrossday = true;
                        }

                        int dayforweek = item["dayforweek"] == null ? -1 : Convert.ToInt32(item["dayforweek"]);
                        if (dayforweek == 7)
                        {
                            dayforweek = 0;
                        }
                        if (dayforweek == Convert.ToInt32(storetime.DayOfWeek) || (isacrossday && dayforweek == storetimeADayOfWeek))
                        {
                            bool wholeday = Convert.ToBoolean(item["wholeday"]);
                            if (wholeday)
                            {
                                isinbusinesstime = true;
                                break;
                            }
                            if (!isacrossday)
                            {
                                if (storetime >= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"]).AddMinutes(Convert.ToInt32(kioskobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"]).AddMinutes(-Convert.ToInt32(kioskobj["lastorderoffset"])))
                                {
                                    isinbusinesstime = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (dayforweek == storetimeADayOfWeek)
                                {
                                    if (storetime >= Convert.ToDateTime(storetime.AddDays(-1).ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00").AddMinutes(Convert.ToInt32(kioskobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59").AddMinutes(-Convert.ToInt32(kioskobj["lastorderoffset"])))
                                    {
                                        isinbusinesstime = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (storetime >= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00").AddMinutes(Convert.ToInt32(kioskobj["firstorderoffset"])) && storetime <= Convert.ToDateTime(storetime.AddDays(1).ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59").AddMinutes(-Convert.ToInt32(kioskobj["lastorderoffset"])))
                                    {
                                        isinbusinesstime = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    kioskobj["inbusinesstime"] = isinbusinesstime;

                    if (!isinbusinesstime)
                    {
                        JArray tipstimelist = JArray.Parse(timelist.ToString());
                        foreach (JObject item in tipstimelist)
                        {
                            item["starttime"] = Convert.ToDateTime("2021-01-01 " + item["starttime"].ToString()).AddMinutes(Convert.ToInt32(kioskobj["firstorderoffset"])).ToString("HH:mm");
                            item["endtime"] = Convert.ToDateTime("2021-01-01 " + item["endtime"].ToString()).AddMinutes(-Convert.ToInt32(kioskobj["lastorderoffset"])).ToString("HH:mm");
                        }
                        kioskobj["tipsbusinesstime"] = tipstimelist;
                    }

                }
                catch (Exception)
                {
                    kioskobj["inbusinesstime"] = false;
                }
                if (kioskobj["pricelevel"] == null)
                {
                    kioskobj["pricelevel"] = 1;
                }
                if (kioskobj["autoclearcarthour"] == null)
                {
                    kioskobj["autoclearcarthour"] = 3;
                }
            }
            jobj["kiosksetting"] = kioskobj;



            #region 判断扫码点餐是否在时间段内

            try
            {

                DateTime storetime = DateTime.UtcNow.AddHours(Convert.ToInt32(jobj["utctimenum"]));
                JArray oldtimelist = new JArray(); //用来检测是否是Special Day
                #region 判断是否符合SpecialDay
                if (jobj["specialdaylist"] != null && !string.IsNullOrWhiteSpace(jobj["specialdaylist"]?.ToString()))
                {
                    try
                    {
                        JArray specialdaylist = JArray.Parse(jobj["specialdaylist"]?.ToString());
                        foreach (var item in specialdaylist)
                        {
                            JArray datebetween = JArray.FromObject(item["datebetween"]);
                            string startdate = datebetween[0].ToString();
                            string enddate = datebetween[1].ToString();

                            DateTime maxenddatetime = Convert.ToDateTime(enddate + " 23:59:59");
                            bool betisopen = Convert.ToBoolean(item["isopen"]);
                            if (betisopen)
                            {
                                JArray openhourlist = JArray.FromObject(item["openhourlist"]);
                                foreach (JObject openhouritem in openhourlist)
                                {
                                    if (Convert.ToDateTime(startdate + " " + openhouritem["starthour"]?.ToString()) > Convert.ToDateTime(startdate + " " + openhouritem["endhour"]?.ToString()))
                                    {
                                        //说明是跨天
                                        if(Convert.ToDateTime(enddate + " " + openhouritem["endhour"]?.ToString()).AddDays(1) > maxenddatetime)
                                        {
                                            maxenddatetime = Convert.ToDateTime(enddate + " " + openhouritem["endhour"]?.ToString()).AddDays(1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                maxenddatetime = Convert.ToDateTime(enddate + " 23:59:59");
                            }


                            if (storetime >= Convert.ToDateTime(startdate + " 00:00:00") && storetime <= maxenddatetime)
                            {
                                string requestservices = item["requestservices"]?.ToString();
                                if (string.IsNullOrWhiteSpace(requestservices) || requestservices == "[]" || requestservices.Contains("dinein"))
                                {
                                    bool isopen = Convert.ToBoolean(item["isopen"]);
                                    if (!isopen)
                                    {
                                        jobj["dineinpause"] = true;
                                        jobj["dineinpausetipsmessage"] = item["notes"]?.ToString();
                                        break;
                                    }
                                    else
                                    {
                                        JArray openhourlist = JArray.FromObject(item["openhourlist"]);
                                        foreach (JObject openhouritem in openhourlist)
                                        {
                                            JObject timejobj = new JObject();
                                            timejobj["dayforweek"] = (int)storetime.Date.DayOfWeek;

                                            if (storetime > Convert.ToDateTime(enddate + " 23:59:59"))
                                            {
                                                timejobj["dayforweek"] = (int)storetime.AddDays(-1).Date.DayOfWeek;
                                            }
                                            else if (storetime.Date.ToString("yyyy-MM-dd") != startdate && Convert.ToDateTime(storetime.Date.ToString("yyyy-MM-dd") + " " + openhouritem["starthour"]?.ToString()) > Convert.ToDateTime(storetime.Date.ToString("yyyy-MM-dd") + " " + openhouritem["endhour"]?.ToString()))
                                            {
                                                timejobj["dayforweek"] = (int)storetime.AddDays(-1).Date.DayOfWeek;
                                            }

                                            timejobj["starttime"] = openhouritem["starthour"]?.ToString();
                                            timejobj["endtime"] = openhouritem["endhour"]?.ToString();
                                            timejobj["wholeday"] = false;
                                            oldtimelist.Add(timejobj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                }
                #endregion
                #region 判断是否符合BlockTime
                if (jobj["blocktimelist"] != null && !string.IsNullOrWhiteSpace(jobj["blocktimelist"]?.ToString()))
                {
                    try
                    {
                        JArray specialdaylist = JArray.Parse(jobj["blocktimelist"]?.ToString());
                        foreach (var item in specialdaylist)
                        {
                            string startdate = item["date"]?.ToString();
                            string enddate = item["date"]?.ToString();

                            if (storetime >= Convert.ToDateTime(startdate + " 00:00:00") && storetime <= Convert.ToDateTime(enddate + " 23:59:59"))
                            {
                                string requestservices = item["requestservices"]?.ToString();
                                if (string.IsNullOrWhiteSpace(requestservices) || requestservices == "[]" || requestservices.Contains("dinein"))
                                {
                                    JArray closehourlist = JArray.FromObject(item["closehourlist"]);
                                    foreach (JObject closehouritem in closehourlist)
                                    {
                                        string starthour = closehouritem["starthour"]?.ToString();
                                        string endhour = closehouritem["endhour"]?.ToString();
                                        if (storetime >= Convert.ToDateTime(startdate + " " + starthour) && storetime <= Convert.ToDateTime(enddate + " " + endhour))
                                        {
                                            jobj["dineinpause"] = true;
                                            jobj["dineinpausetipsmessage"] = item["notes"]?.ToString();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                }
                #endregion
                int firstorderoffset = 0;
                int lastorderoffset = 0;

                if (jobj["firstorderoffset"] != null)
                {
                    firstorderoffset = Convert.ToInt32(jobj["firstorderoffset"]);
                }
                if (jobj["lastorderoffset"] != null)
                {
                    lastorderoffset = Convert.ToInt32(jobj["lastorderoffset"]);
                }

                JArray timelist = new JArray();
                if (jobj["dineintimelist"] != null && jobj["dineintimelist"].ToString().Trim() != "")
                {
                    timelist = JArray.Parse(jobj["dineintimelist"].ToString());
                    if(timelist != null && timelist.Count > 0)
                    {
                        firstorderoffset = 0;
                        lastorderoffset = 0;
                    }
                }
                if ((timelist == null || timelist.Count <= 0) && jobj["timelist"] != null && jobj["timelist"].ToString().Trim() != "")
                {
                    timelist = JArray.Parse(jobj["timelist"].ToString());
                }
                if (timelist.Count <= 0)
                {
                    //说明没有限制 手动添加7天的
                    for (int i = 0; i < 7; i++)
                    {
                        JObject timejobj = new JObject();
                        timejobj["dayforweek"] = i;
                        timejobj["starttime"] = "00:00";
                        timejobj["endtime"] = "23:59";
                        timejobj["wholeday"] = true;
                        timelist.Add(timejobj);
                    }
                }
                if (oldtimelist.Count > 0)
                {
                    timelist = new JArray();
                    firstorderoffset = 0;
                    lastorderoffset = 0;
                    foreach (var item in oldtimelist)
                    {
                        timelist.Add(item);
                    }
                }

                int storetimeADayOfWeek = Convert.ToInt32(storetime.DayOfWeek);
                if (storetimeADayOfWeek != 0)
                {
                    storetimeADayOfWeek = storetimeADayOfWeek - 1;
                }
                else
                {
                    storetimeADayOfWeek = 6;
                }

                bool isinbusinesstime = false;
                foreach (JObject item in timelist)
                {
                    bool isacrossday = false;
                    //先判断一下是否是跨一天时间段,如果开始时间大于结束时间，认为是跨天营业
                    if (Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00") > Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59"))
                    {
                        isacrossday = true;
                    }

                    int dayforweek = item["dayforweek"] == null ? -1 : Convert.ToInt32(item["dayforweek"]);
                    if (dayforweek == 7)
                    {
                        dayforweek = 0;
                    }
                    if (dayforweek == Convert.ToInt32(storetime.DayOfWeek) || (isacrossday && dayforweek == storetimeADayOfWeek))
                    {
                        bool wholeday = Convert.ToBoolean(item["wholeday"]);
                        if (wholeday)
                        {
                            isinbusinesstime = true;
                            break;
                        }
                        if (!isacrossday)
                        {
                            if (storetime >= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00").AddMinutes(Convert.ToInt32(firstorderoffset)) && storetime <= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59").AddMinutes(-Convert.ToInt32(lastorderoffset)))
                            {
                                isinbusinesstime = true;
                                break;
                            }
                        }
                        else
                        {
                            if (dayforweek == storetimeADayOfWeek)
                            {
                                if (storetime >= Convert.ToDateTime(storetime.AddDays(-1).ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00").AddMinutes(Convert.ToInt32(firstorderoffset)) && storetime <= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59").AddMinutes(-Convert.ToInt32(lastorderoffset)))
                                {
                                    isinbusinesstime = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (storetime >= Convert.ToDateTime(storetime.ToString("yyyy-MM-dd") + " " + item["starttime"] + ":00").AddMinutes(Convert.ToInt32(firstorderoffset)) && storetime <= Convert.ToDateTime(storetime.AddDays(1).ToString("yyyy-MM-dd") + " " + item["endtime"] + ":59").AddMinutes(-Convert.ToInt32(lastorderoffset)))
                                {
                                    isinbusinesstime = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!isinbusinesstime)
                {
                    foreach (JObject item in timelist)
                    {
                        item["starttime"] = Convert.ToDateTime("2021-01-01 " + item["starttime"].ToString()).AddMinutes(firstorderoffset).ToString("HH:mm");
                        item["endtime"] = Convert.ToDateTime("2021-01-01 " + item["endtime"].ToString()).AddMinutes(-lastorderoffset).ToString("HH:mm");
                    }
                    jobj["dineinnotinbusinesstime"] = true;
                    jobj["dineinnotinbusinesstimedata"] = timelist;
                }
            }
            catch (Exception ex) { }
            #endregion

            return JsonConvert.SerializeObject(jobj);
        }
