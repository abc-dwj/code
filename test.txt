public ResultMsg GetStoreLocationList(string hotelid, string loginkey)
        {
            ResultMsg resultMsg = new ResultMsg(StatusCodeEnum.Success);
            CenterHotel hotelModel = _db.Hotels.AsNoTracking().FirstOrDefault(x => x.Hid == hotelid);
            List<CenterHotel> hotellist = new List<CenterHotel>();
            if (hotelModel != null)
            {
                hotellist.Add(hotelModel);
                if (!string.IsNullOrEmpty(hotelModel.Grpid) && hotelModel.Grpid == hotelid)
                {
                    hotellist.AddRange(_db.Hotels.AsNoTracking().Where(x => x.Grpid == hotelid && x.Hid != hotelModel.Hid).ToList());
                }
            }
            else
            {
                resultMsg = new ResultMsg(StatusCodeEnum.NotSuccess);
                return resultMsg;
            }
            List<string> hotelidlist = hotellist.Select(x => x.Hid).ToList();
            List<HotelProducts> productslist = _db.HotelProducts.AsNoTracking().Where(x => hotelidlist.Contains(x.Hid)).ToList();

            DataBaseList dbitem = _db.DbLists.AsNoTracking().FirstOrDefault(x => x.Id == hotelModel.Dbid);
            string connstr = ConnStrHelper.GetConnStr(dbitem.DbServer, dbitem.DbName, dbitem.LogId, dbitem.LogPwd, "GemstarBSPMS", dbitem.IntIp, _db.IsConnectViaInternetIp());

            string hidlist = "";


            hotelidlist.ForEach(x =>
            {
                hidlist += ("'" + x + "'" + ",");
            });
            hidlist = hidlist.TrimEnd(',');


            string sql = "select * from hotellocation where hid in (" + hidlist + ");select hid,isnull(locationbookingconfig,'') as locationbookingconfig,isnull(locationbookingorderurl,'') as locationbookingorderurl,isnull(locationlineupconfig,'') as locationlineupconfig,isnull(locationorderconfig,'') as locationorderconfig,isnull(locationbookinglineupurl,'') as locationbookinglineupurl from pmshotel where hid in (" + hidlist + ") ";

            DataSet configset = ADOHelper.ExecuteSql(sql, connstr, null);


            DataTable dtable = new DataTable();
            dtable.Columns.Add("hid");
            dtable.Columns.Add("restaurant_uid");
            dtable.Columns.Add("restaurant_token");
            dtable.Columns.Add("server_key");
            dtable.Columns.Add("environment_key");
            dtable.Columns.Add("usertoken_prefix");
            dtable.Columns.Add("currency");
            dtable.Columns.Add("hide_paymemt_methods");

            dtable.Columns.Add("storeLogo");
            dtable.Columns.Add("storeTitle");
            dtable.Columns.Add("storeAddress");
            dtable.Columns.Add("storeBg");
            dtable.Columns.Add("storeLocation");
            dtable.Columns.Add("storeSTime");
            dtable.Columns.Add("storeETime");
            dtable.Columns.Add("storePhone");
            dtable.Columns.Add("storeCity");
            dtable.Columns.Add("storeIsShow");
            dtable.Columns.Add("payorderIsShow", typeof(bool));
            dtable.Columns.Add("bookingorderIsShow", typeof(bool));
            dtable.Columns.Add("lineuporderIsShow", typeof(bool));


            dtable.Columns.Add("showopenhour", typeof(bool));
            dtable.Columns.Add("todaytitle");
            dtable.Columns.Add("todayopenhourtext");

            dtable.Columns.Add("onlineUrl");
            dtable.Columns.Add("bookingUrl");
            dtable.Columns.Add("lineupUrl");



            foreach (var item in hotellist)
            {
                string bookingorderurl = "";
                string lineuporderurl = "";

                DataRow drow = dtable.NewRow();
                drow["hid"] = item.Hid.Trim();
                drow["restaurant_uid"] = item.Restaurant_uid;
                drow["restaurant_token"] = item.Restaurant_token;
                drow["server_key"] = item.Server_key;
                drow["environment_key"] = item.Environment_key;
                drow["usertoken_prefix"] = item.Usertoken_prefix;
                drow["currency"] = item.Currency;
                drow["hide_paymemt_methods"] = item.hide_paymemt_methods;

                drow["storeIsShow"] = false;

                drow["payorderIsShow"] = true;

                drow["bookingorderIsShow"] = false;
                drow["lineuporderIsShow"] = false;
                string usertoken = loginkey;
                if (!string.IsNullOrEmpty(item.Usertoken_prefix))
                {
                    usertoken = item.Usertoken_prefix + ":" + usertoken;
                }
                drow["onlineUrl"] = "https://www.QuickPosOnline.ca/ordering/?restaurant_uid=" + item.Restaurant_uid + "&user_token=" + usertoken;

                if (!string.IsNullOrEmpty(item.hide_paymemt_methods) && item.hide_paymemt_methods.Trim() == "1")
                {
                    drow["onlineUrl"] = drow["onlineUrl"].ToString() + "&hide_payment_methods=" + item.hide_paymemt_methods;
                }
                if (item.OrderPlatform != null)
                {
                    if (item.OrderPlatform == 1)
                    {
                        drow["onlineUrl"] = item.CPlatformUrl;
                    }
                    //如果是Q平台（quickpos）
                    if (item.OrderPlatform == 2)
                    {
                        drow["onlineUrl"] = item.QPlatformUrl;
                        if (!string.IsNullOrWhiteSpace(item.QPlatformUrl))
                        {
                            if (item.QPlatformUrl.IndexOf("?") >= 0)
                            {
                                drow["onlineUrl"] += "&usertoken=" + loginkey;
                            }
                            else
                            {
                                drow["onlineUrl"] += "?usertoken=" + loginkey;
                            }
                        }
                        #region 设置openhour显示的内容
                        try
                        {
                            if (configset.Tables[1] != null && configset.Tables[1].Rows.Count > 0)
                            {
                                foreach (DataRow configdrow in configset.Tables[1].Rows)
                                {
                                    string hid = configdrow[0] == DBNull.Value || configdrow[0] == null ? "" : configdrow[0]?.ToString();
                                    if (item.Hid == hid)
                                    {
                                        string todaytitle = "";
                                        string todayopenhourtext = "";
                                        int dayforweek = (int)DateTime.UtcNow.AddHours(item.UtcTimeNum).DayOfWeek;
                                        switch (dayforweek)
                                        {
                                            case 1:
                                                todaytitle = "Monday";
                                                break;
                                            case 2:
                                                todaytitle = "Tueday";
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
                                        drow["todaytitle"] = todaytitle;

                                        string locationorderconfigstr = configdrow["locationorderconfig"]?.ToString();
                                        if (!string.IsNullOrEmpty(locationorderconfigstr))
                                        {
                                            drow["showopenhour"] = true;
                                            JObject locationorderconfig = JObject.Parse(locationorderconfigstr);
                                            string timeliststr = locationorderconfig["timelist"]?.ToString();
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
                                                                    drow["todayopenhourtext"] = "24 Hours";
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
                                                        drow["todayopenhourtext"] = "Close";
                                                    }
                                                    else
                                                    {
                                                        drow["todayopenhourtext"] = todayopenhourtext;
                                                    }
                                                }
                                                else
                                                {
                                                    drow["todayopenhourtext"] = "24 Hours";
                                                }
                                            }
                                            else
                                            {
                                                drow["todayopenhourtext"] = "24 Hours";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception) { }
                        #endregion
                    }
                    if (item.OrderPlatform == -1)
                    {
                        drow["payorderIsShow"] = false;
                    }
                }
                #region 这里判断是否有权限使用booking
                if (productslist != null && productslist.Count(x => x.Hid == item.Hid.Trim() && x.IsEnable && x.ProductCode == "booking") > 0)
                {
                    if (configset.Tables[1] != null && configset.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow configdrow in configset.Tables[1].Rows)
                        {
                            string hid = configdrow[0] == DBNull.Value || configdrow[0] == null ? "" : configdrow[0]?.ToString();
                            if (item.Hid == hid)
                            {
                                string bookingstr = configdrow[1] == DBNull.Value || configdrow[1] == null ? "" : configdrow[1]?.ToString();

                                if (string.IsNullOrEmpty(bookingorderurl))
                                {
                                    bookingorderurl = configdrow[2] == DBNull.Value || configdrow[2] == null ? "" : configdrow[2]?.ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(bookingstr))
                                {
                                    JObject bookingconfig = JObject.Parse(bookingstr);
                                    if (bookingconfig != null && bookingconfig["enable"] != null && Convert.ToBoolean(bookingconfig["enable"]))
                                    {
                                        drow["bookingorderIsShow"] = true;

                                        if (string.IsNullOrEmpty(bookingorderurl))
                                        {
                                            Random rd = new Random(Guid.NewGuid().GetHashCode());
                                            string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
                                            //生成booking url
                                            while (true)
                                            {
                                                string result = "";
                                                for (int i = 0; i < 10; i++)
                                                {
                                                    result += str[rd.Next(str.Length)];
                                                }
                                                if (_db.Shorturlinfo.Count(x => x.Shortcode == result) > 0)
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    Shorturlinfo info = new Shorturlinfo();
                                                    info.Id = Guid.NewGuid();
                                                    info.Shortcode = result;
                                                    info.Hid = hid;
                                                    info.Longurl = "https://cloud.quickposhub.com/onlineorder/#/pages/user/reservation?hotelid=" + hid;
                                                    info.Createdate = DateTime.UtcNow;
                                                    bookingorderurl = "https://cloud.quickposhub.com/onlineorder/#/pages/order/tableurl?code=" + result;

                                                    _db.Shorturlinfo.Add(info);
                                                    _db.SaveChanges();
                                                    ADOHelper.ExecNonQuery(" update pmshotel set locationbookingorderurl = '" + bookingorderurl + "' where hid = '" + hid + "' ", connstr);
                                                    break;
                                                }
                                            }
                                        }
                                        drow["bookingUrl"] = bookingorderurl + "&usertoken=" + loginkey + "&showtav=0&checkmode=1";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 这里判断是否有权限使用lineup
                if (productslist != null && productslist.Count(x => x.Hid == item.Hid.Trim() && x.IsEnable && x.ProductCode == "lineup") > 0)
                {
                    if (configset.Tables[1] != null && configset.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow configdrow in configset.Tables[1].Rows)
                        {
                            string hid = configdrow[0] == DBNull.Value || configdrow[0] == null ? "" : configdrow[0]?.ToString();
                            if (item.Hid == hid)
                            {
                                if (string.IsNullOrEmpty(lineuporderurl))
                                {
                                    lineuporderurl = configdrow[5] == DBNull.Value || configdrow[5] == null ? "" : configdrow[5]?.ToString();
                                }
                                drow["lineuporderIsShow"] = true;

                                if (string.IsNullOrEmpty(lineuporderurl))
                                {
                                    Random rd = new Random(Guid.NewGuid().GetHashCode());
                                    string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
                                    //生成booking url
                                    while (true)
                                    {
                                        string result = "";
                                        for (int i = 0; i < 10; i++)
                                        {
                                            result += str[rd.Next(str.Length)];
                                        }
                                        if (_db.Shorturlinfo.Count(x => x.Shortcode == result) > 0)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            Shorturlinfo info = new Shorturlinfo();
                                            info.Id = Guid.NewGuid();
                                            info.Shortcode = result;
                                            info.Hid = hid;
                                            info.Longurl = "https://cloud.quickposhub.com/onlineorder/#/pages/user/lineup?hotelid=" + hid;
                                            info.Createdate = DateTime.UtcNow;
                                            lineuporderurl = "https://cloud.quickposhub.com/onlineorder/#/pages/order/tableurl?code=" + result;

                                            _db.Shorturlinfo.Add(info);
                                            _db.SaveChanges();
                                            ADOHelper.ExecNonQuery(" update pmshotel set locationbookinglineupurl = '" + lineuporderurl + "' where hid = '" + hid + "' ", connstr);
                                            break;
                                        }
                                    }
                                }
                                drow["lineupUrl"] = lineuporderurl + "&usertoken=" + loginkey + "&showtav=-1";
                                break;
                            }
                        }
                    }
                }
                #endregion
                dtable.Rows.Add(drow);
            }

            DataTable locationtable = configset.Tables[0];
            foreach (DataRow item in locationtable.Rows)
            {
                for (int i = 0; i < dtable.Rows.Count; i++)
                {
                    string hid = dtable.Rows[i]["hid"].ToString();
                    if (hid == item["hid"].ToString().Trim())
                    {
                        dtable.Rows[i]["storeLogo"] = item["storeLogo"].ToString();
                        dtable.Rows[i]["storeTitle"] = item["storeTitle"].ToString();
                        dtable.Rows[i]["storeAddress"] = item["storeAddress"].ToString();
                        dtable.Rows[i]["storeBg"] = item["storeBg"].ToString();
                        dtable.Rows[i]["storeLocation"] = item["storeLocation"].ToString();


                        if (item["unit"] != DBNull.Value && item["unit"] != null)
                        {
                            string unit = item["unit"]?.ToString();

                            if (!string.IsNullOrWhiteSpace(unit))
                            {
                                dtable.Rows[i]["storeAddress"] = unit + ", " + dtable.Rows[i]["storeAddress"]?.ToString();
                            }
                        }

                        dtable.Rows[i]["storeSTime"] = item["storeSTime"].ToString();
                        dtable.Rows[i]["storeETime"] = item["storeETime"].ToString();
                        dtable.Rows[i]["storePhone"] = item["storePhone"].ToString();
                        dtable.Rows[i]["storeCity"] = item["city"].ToString();

                        bool isshow = (item["isshow"].ToString() == "1" ? true : false);
                        if (!isshow)
                        {
                            dtable.Rows[i].Delete();
                            continue;
                        }
                        dtable.Rows[i]["storeIsShow"] = (item["isshow"].ToString() == "1" ? true : false);
                        break;
                    }
                }
            }
            dtable.AcceptChanges();

            for (int i = 0; i < dtable.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(dtable.Rows[i]["storeIsShow"]))
                {
                    dtable.Rows[i].Delete();
                }
            }
            dtable.AcceptChanges();
            resultMsg.data = JArray.FromObject(dtable);
            resultMsg.dataCount = dtable.Rows.Count;
            return resultMsg;
        }
