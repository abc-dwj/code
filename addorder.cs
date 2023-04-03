
        /// <summary>
        /// 把onlineorder订单发送到POS
        /// </summary>
        /// <param name="hotelname"></param>
        /// <param name="hid"></param>
        /// <param name="FormId"></param>
        [WebMethod]
        public void NotifyPOSByOnlineOrder(string hotelname, string hid, string FormId, string hidconnstr)
        {
            new Thread(() =>
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 200;
                string newhid = hid + "";
                string newformid = FormId + "";
                string newhidconnstr = hidconnstr + "";
                string dbconnstr = ConfigurationManager.AppSettings["connstr"];
                string newhotelname = hotelname + "";

                DbCommonContext _db = new DbCommonContext(dbconnstr);

                var poswebhookurlmodel = _db.SysParas.FirstOrDefault(x => x.Code == "poswebhookurl");
                var postestwebhookurlmodel = _db.SysParas.FirstOrDefault(x => x.Code == "postestwebhookurl");


                string defaultrequesturl = poswebhookurlmodel.Value + "/CloudPOS/CPOS/NewOrder";
                try
                {
                    if (newhid == "999000" || newhid == "999111" || newhid == "999112" || newhid == "999999")
                    {
                        defaultrequesturl = postestwebhookurlmodel.Value + "/CloudPOS/CPOS/NewOrder";
                    }
                }
                catch (Exception) { }

                JArray jarray = new JArray();

                try
                {
                    int reset = 0;
                    while (true)
                    {
                        try
                        {
                            jarray = JArray.Parse(GetLocationOrderListPublic(newhid, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), newformid, _db));
                            break;
                        }
                        catch (Exception ex)
                        {
                            new SysLogService(_db).AddSysLog("[" + FormId + "]" + ex.ToString(), "CloudLocationOrder-POS-Error", "", "", "");
                            if (ex.ToString().Contains("Duplicate Order ID"))
                            {
                                try
                                {
                                    ADOHelper.ExecNonQuery(" update cloudlocationorder set issendposcomplete = 1 where formid = @newformid and isackorder = 1 ", newhidconnstr, new List<SqlParameter>()
                                    {
                                        new SqlParameter("@newformid",newformid)
                                    });
                                    //ADOHelper.ExecNonQuery(" update cloudlocationorder set formid = @newformid where formid = @newformid ", newhidconnstr, new List<SqlParameter>()
                                    //{
                                    //    new SqlParameter("@newformid",newformid)
                                    //});
                                }
                                catch (Exception exac)
                                {
                                    new SysLogService(_db).AddSysLog("[" + FormId + "]" + exac.ToString(), "CloudLocationOrder-POS-Error", "", "", "");
                                }
                            }
                            Thread.Sleep(1000);
                        }
                        reset++;

                        if(reset >= 6)
                        {
                            bool sendplivosms = true;
                            try
                            {
                                if (newhid == "999000" || newhid == "999111" || newhid == "999112" || newhid == "999999")
                                {
                                    sendplivosms = false;
                                }
                            }
                            catch (Exception) { }
                            try
                            {
                                if (sendplivosms)
                                {
                                    SmsPlivoPara sendmodel = null;
                                    if (sendmodel == null || string.IsNullOrWhiteSpace(sendmodel.Auth_id))
                                    {
                                        sendmodel = new SmsPlivoPara();
                                        sendmodel.Auth_id = _db.SysParas.Where(x => x.Code == "Default SMS-AuthID").FirstOrDefault().Value;
                                        sendmodel.Auth_token = _db.SysParas.Where(x => x.Code == "Default SMS-AuthToken").FirstOrDefault().Value;
                                        sendmodel.SendPhone = _db.SysParas.Where(x => x.Code == "Default SMS-SendPhone").FirstOrDefault().Value;
                                    }
                                    sendmodel.Hid = "999000";
                                    if (sendmodel != null && !string.IsNullOrEmpty(sendmodel.Auth_id) && !string.IsNullOrEmpty(sendmodel.Auth_token) && !string.IsNullOrEmpty(sendmodel.SendPhone))
                                    {
                                        PlivoSMSWebService soap = new PlivoSMSWebService();
                                        soap.PlivoSmsSend(JsonConvert.SerializeObject(sendmodel), "+14163155669", "[" + hotelname + "][" + newformid + "]WebHook & Error Receiving POS Order");
                                    }
                                    ADOHelper.ExecNonQuery(" update cloudlocationorder set issendposcomplete = 0 where formid = '" + newformid + "' and issendposcomplete is null ", newhidconnstr, new List<SqlParameter>()
                                    {
                                        new SqlParameter("@hid",newhid)
                                    });


                                    new SmsLogService("999000", dbconnstr, "").AddLog("14163155669", "[" + newhotelname + "][" + newformid + "]OnlineOrder Error Order", "发送成功", null, newhid, "system", "SMS999", sendmodel.SendPhone);
                                }
                            }
                            catch (Exception) { }
                            break;
                        }
                    }

                    int sendcount = 0;
                    foreach (JObject item in jarray)
                    {
                        while (true)
                        {
                            try
                            {
                                XJPWebHook.SendWebHookSoapClient soap = new XJPWebHook.SendWebHookSoapClient();
                                JObject responseobj = JObject.Parse(soap.Send(defaultrequesturl, CryptHelper.AesEncrypt(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "@@" + CryptHelper.AesEncrypt(hid)), "15000", item.ToString()));
                                if (int.Parse(responseobj["status"].ToString()) != 0)
                                {
                                    new SysLogService(_db).AddSysLog("[" + FormId + "]" + responseobj["errormessage"].ToString(), "CloudLocationOrder-POS-Error", "", "", "");
                                }
                                else
                                {
                                    sendcount = 0;
                                    break;
                                }

                                //string url = defaultrequesturl;
                                //string jsonParam = item.ToString();
                                //var request = (HttpWebRequest)WebRequest.Create(url);
                                //request.Method = "POST";
                                //request.ContentType = "application/json;charset=UTF-8";
                                //request.Headers.Add("restaurant_ID", CryptHelper.AesEncrypt(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "@@" + CryptHelper.AesEncrypt(hid)));
                                //byte[] byteData = Encoding.UTF8.GetBytes(jsonParam);
                                //int length = byteData.Length;
                                //request.ContentLength = length;
                                //request.Timeout = 20000;
                                //Stream writer = request.GetRequestStream();
                                //writer.Write(byteData, 0, length);
                                //writer.Close();
                                //var response = (HttpWebResponse)request.GetResponse();
                                //var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();

                                //JObject responseobj = JObject.Parse(responseString);
                                //if (int.Parse(responseobj["status"].ToString()) != 0)
                                //{
                                //    new SysLogService(_db).AddSysLog("[" + FormId + "]" + responseobj["errormessage"].ToString(), "CloudLocationOrder-POS-Error", "", "", "");
                                //}
                                //else
                                //{
                                //    sendcount = 0;
                                //    break;
                                //}


                            }
                            catch (Exception ex)
                            {
                                new SysLogService(_db).AddSysLog("[" + FormId + "]" + ex.ToString(), "CloudLocationOrder-POS-Error", "", "", "");

                                //这里说明前面的请求其实成功了，但是超时了，所以直接跳出循环
                                if (ex.ToString().Contains("OnlineCachePrimaryKey"))
                                {
                                    sendcount = 0;
                                    break;
                                }
                                Thread.Sleep(2000);
                            }
                            sendcount++;

                            //如果重发错误超过6次
                            if (sendcount >= 6)
                            {
                                bool sendplivosms = true;
                                try
                                {
                                    if (newhid == "999000" || newhid == "999111" || newhid == "999112" || newhid == "999999")
                                    {
                                        sendplivosms = false;
                                    }
                                }
                                catch (Exception) { }
                                string callnumber = "";
                                try
                                {
                                    if (sendplivosms)
                                    {
                                        SmsPlivoPara sendmodel = null;
                                        if (sendmodel == null || string.IsNullOrWhiteSpace(sendmodel.Auth_id))
                                        {
                                            sendmodel = new SmsPlivoPara();
                                            sendmodel.Auth_id = _db.SysParas.Where(x => x.Code == "Default SMS-AuthID").FirstOrDefault().Value;
                                            sendmodel.Auth_token = _db.SysParas.Where(x => x.Code == "Default SMS-AuthToken").FirstOrDefault().Value;
                                            sendmodel.SendPhone = _db.SysParas.Where(x => x.Code == "Default SMS-SendPhone").FirstOrDefault().Value;
                                        }
                                        sendmodel.Hid = "999000";
                                        if (sendmodel != null && !string.IsNullOrEmpty(sendmodel.Auth_id) && !string.IsNullOrEmpty(sendmodel.Auth_token) && !string.IsNullOrEmpty(sendmodel.SendPhone))
                                        {
                                            PlivoSMSWebService soap = new PlivoSMSWebService();

                                            try
                                            {
                                                foreach (JObject orderitem in jarray)
                                                {
                                                    callnumber = orderitem["callnumber"]?.ToString();
                                                }
                                            }
                                            catch (Exception) { }

                                            soap.PlivoSmsSend(JsonConvert.SerializeObject(sendmodel), "+14163155669", "[" + hotelname + "][" + callnumber + "]WebHook & Error Receiving POS Order");
                                        }
                                        ADOHelper.ExecNonQuery(" update cloudlocationorder set issendposcomplete = 0 where formid = '" + newformid + "' ", newhidconnstr, new List<SqlParameter>()
                                        {
                                            new SqlParameter("@hid",newhid)
                                        });


                                        new SmsLogService("999000", dbconnstr, "").AddLog("14163155669", "[" + hotelname + "][" + callnumber + "]WebHook & Error Receiving POS Order", "发送成功", null, newhid, "system", "SMS999", sendmodel.SendPhone);
                                    }
                                }
                                catch (Exception) { }
                                break;
                            }
                        }
                    }

                    //这里是通知到POS成功了
                    if (sendcount == 0)
                    {
                        try
                        {
                            CenterHotel hotelmodel = _db.Hotels.FirstOrDefault(x => x.Hid == hid);
                            string grpid = "";
                            if (hotelmodel != null)
                            {
                                grpid = hotelmodel.Hid;
                                if (!string.IsNullOrWhiteSpace(hotelmodel.Grpid))
                                {
                                    grpid = hotelmodel.Grpid;
                                }
                            }
                            JObject orderobj = JObject.Parse(GetOrderInfoPublic(hotelmodel.Hid, newformid, "en", _db));
                            JObject locationobj = new JObject();
                            JObject configobj = new JObject();

                            //这里去后台拿pickup的配置
                            string settingsql = " select isnull(locationorderconfig,''),isnull(locationpickupconfig,'') from pmshotel where hid = @hid ";

                            if (orderobj["ordertype"]?.ToString() == "delivery")
                            {
                                settingsql = " select isnull(locationorderconfig,''),isnull(locationdeliveryconfig,'') from pmshotel where hid = @hid ";
                            }

                            ADOHelper.ExecNonQuery(" update cloudlocationorder set issendposcomplete = 1 where formid = '" + newformid + "' ", newhidconnstr, new List<SqlParameter>()
                            {
                                new SqlParameter("@hid",hotelmodel.Hid)
                            });

                            DataTable settingtable = ADOHelper.ExecSql(settingsql, newhidconnstr, new List<SqlParameter>()
                            {
                                new SqlParameter("@hid",hotelmodel.Hid)
                            });
                            if (settingtable != null && settingtable.Rows.Count > 0)
                            {
                                string locationconfigstr = settingtable.Rows[0][0]?.ToString();
                                string configstr = settingtable.Rows[0][1]?.ToString();

                                if (!string.IsNullOrWhiteSpace(locationconfigstr))
                                {
                                    locationobj = JObject.Parse(locationconfigstr);
                                }
                                if (!string.IsNullOrWhiteSpace(configstr))
                                {
                                    configobj = JObject.Parse(configstr);
                                }

                                //这里发送邮件通知到客户
                                try
                                {
                                    Emailconfig defaultemailconfig = _db.EmailConfig.FirstOrDefault(x => x.Hid == grpid);
                                    if (defaultemailconfig == null || string.IsNullOrWhiteSpace(defaultemailconfig.Emailsendserver))
                                    {
                                        defaultemailconfig = new Emailconfig();
                                        defaultemailconfig.Emailsendserver = _db.SysParas.Where(x => x.Code == "Default Email-SendServer").FirstOrDefault().Value;
                                        defaultemailconfig.Emailsendport = _db.SysParas.Where(x => x.Code == "Default Email-Port").FirstOrDefault().Value;
                                        defaultemailconfig.Emailsendusername = _db.SysParas.Where(x => x.Code == "Default Email-SendUserName").FirstOrDefault().Value;
                                        defaultemailconfig.Emailsendpassword = _db.SysParas.Where(x => x.Code == "Default Email-Password").FirstOrDefault().Value;
                                    }
                                    SendEmail(hid, defaultemailconfig, orderobj, locationobj, configobj);

                                    try
                                    {
                                        if (!string.IsNullOrWhiteSpace(locationobj["isforwardemail"]?.ToString()) && Convert.ToBoolean(locationobj["isforwardemail"]))
                                        {
                                            string[] forwardmethod = locationobj["forwardmethod"]?.ToString().Split(',');
                                            if (string.IsNullOrWhiteSpace(locationobj["forwardmethod"]?.ToString()) || (orderobj["ordertype"]?.ToString() == "delivery" && forwardmethod.Contains("delivery")) || (orderobj["ordertype"]?.ToString() == "pickup" && forwardmethod.Contains("pickup")) || (orderobj["ordertype"]?.ToString() == "dinein" && forwardmethod.Contains("dinein")))
                                            {
                                                JObject neworderobj = JObject.Parse(orderobj.ToString());
                                                neworderobj["customerinfo"]["email"] = locationobj["forwardemailaddress"]?.ToString();
                                                SendEmail(hid, defaultemailconfig, neworderobj, locationobj, configobj);
                                            }
                                        }
                                    }
                                    catch (Exception) { }

                                }
                                catch (Exception) { }

                                //这里发送短信通知到客户
                                try
                                {
                                    SmsPlivoPara defaultsmsconfig = _db.SmsPlivoPara.FirstOrDefault(x => x.Hid == grpid);
                                    if (defaultsmsconfig == null || string.IsNullOrWhiteSpace(defaultsmsconfig.Auth_id))
                                    {
                                        defaultsmsconfig = new SmsPlivoPara();
                                        defaultsmsconfig.Auth_id = _db.SysParas.Where(x => x.Code == "Default SMS-AuthID").FirstOrDefault().Value;
                                        defaultsmsconfig.Auth_token = _db.SysParas.Where(x => x.Code == "Default SMS-AuthToken").FirstOrDefault().Value;
                                        defaultsmsconfig.SendPhone = _db.SysParas.Where(x => x.Code == "Default SMS-SendPhone").FirstOrDefault().Value;
                                    }
                                    SendPlivoSms(hid, defaultsmsconfig, orderobj, configobj);
                                }
                                catch (Exception) { }
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
                catch (Exception ex)
                {
                    new SysLogService(_db).AddSysLog("[" + FormId + "]" + ex.ToString(), "CloudLocationOrder-POS-Error", "", "", "");
                }
            }).Start();
        }
