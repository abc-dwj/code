getiteminfo(itemid, classcode,itemmodifiergroupshrink,itemmodifiergroupshrinknum,defaultiteminfo,itemcode,detailinfo,ispointsexchange) {
				var that = this;
				var data = {
					hid: that.indexhotelid,
					classcode: classcode,
					itemid: itemid,
					menuid: that.menuid,
					saleid: that.saleid,
					itemcode:itemcode,
					cartamount:that.cartamount
				}
				if(!data.hid){
					data.hid = uni.getStorageSync('orderhotelid')
					that.indexhotelid = data.hid
				}
				
				//这里直接使用缓存的iteminfo 不去查数据库
				if((defaultiteminfo && !that.saleid) || (detailinfo && !that.saleid)){
					uni.hideLoading()
					if(detailinfo){
						that.orderitem = JSON.parse(detailinfo)
						that.orderitem.iteminfo = that.orderitem.iteminfo[0]
					}else{
						that.orderitem.iteminfo = defaultiteminfo
					}
					
					
								
					if(ispointsexchange){
						that.ispointsexchange = true
						
						try{
							if(parseFloat(that.GetPriceByDic(that.orderitem.iteminfo.pricedic)) < 0){
								that.showspecial = false
							}
						}catch(e){
							
						}
					}
					
					var alllistlength = 0
					try{
						if(that.orderitem.itemcombolist){
							alllistlength += that.orderitem.itemcombolist.length
						}
					    if(that.orderitem.itemmodgrouplist){
					        alllistlength += that.orderitem.itemmodgrouplist.length
					    }
					}catch(liste){}
					
					
					
					//这下面的代码重复了，和去接口请求GetItemInfo处理逻辑重复， 后续要优化掉，只保留其中一个
					if (that.orderitem.itemmodgrouplist) {
						that.orderitem.itemmodgrouplist.forEach((item, index) => {
							item.modifiercodeslist.forEach((moditem, modindex) => {
								moditem.DisableCheck = false
								moditem.repeatcount = 1
							})
							//这里判断单独的改餐是否收缩 S
							// if(!that.orderitem.itemcombolist || that.orderitem.itemcombolist.length <= 0){
							// 	//如果要自动收缩
							// 	if(itemmodifiergroupshrink){
							// 		//如果modgroup大于 X 进入是否收缩判断
							// 		if(that.orderitem.itemmodgrouplist.length >= itemmodifiergroupshrinknum){
							// 			//如果子项大于1，收缩
							// 			if(item.modifiercodeslist && item.modifiercodeslist.length > 1){
							// 				item.showexitem = false
							// 			}else{
							// 				item.showexitem = true
							// 			}
							// 		}else{
							// 			item.showexitem = true
							// 		}
							// 	}else{
							// 		item.showexitem = true
							// 	}
							// }
							//这里判断单独的改餐是否收缩 E
							
							//20230419 新 合并combo判断
							if(itemmodifiergroupshrink){
								//如果modgroup大于 X 进入是否收缩判断
								if(alllistlength >= itemmodifiergroupshrinknum){
									//如果子项大于1，收缩
									if(item.modifiercodeslist && item.modifiercodeslist.length > 1){
										item.showexitem = false
									}else{
										item.showexitem = true
									}
								}else{
									item.showexitem = true
								}
							}else{
								item.showexitem = true
							}
							
							//20230419 新 合并combo判断 E
							
						})
						
						// <checkbox :value="item.id" :disabled="true"
						// 	@tap="modifiercodesclick" :data-groupid="moditem.id"
						// 	:data-codeid="item.id" :checked="item.checked"
						
						
						//这下面用来判断 MG改餐 是否需要默认选中
						that.orderitem.itemmodgrouplist.forEach((moditem,modindex) =>{
							if(moditem.minselection == 1 && moditem.maxselection == 1){
								if(moditem.modifiercodeslist && moditem.modifiercodeslist.length == 1){
									moditem.noshowreq = true  //如果只有一项 ，不显示 必选 的文字
									moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
										var edata = {
											currentTarget:{
												dataset:{
													codeid:sonitem.id,
													groupid:moditem.id
												}
											}
										}
										
										sonitem.noshowcheckbox = true
										that.modifiercodesclick(edata)
									})
								}else if(moditem.modifiercodeslist && moditem.modifiercodeslist.length > 1){
									if(moditem.defaultmodifier){
										//移到外层处理，不受minselection 的限制
									}else{
										if(that.defaultselectfirstmod){
											moditem.showexitem = true
											moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
												if(sonindex == 0){
													var edata = {
														currentTarget:{
															dataset:{
																codeid:sonitem.id,
																groupid:moditem.id
															}
														}
													}
													that.modifiercodesclick(edata)
												}
											})
										}
									}
								}
							}
						})
						//选中默认的modifier
						that.orderitem.itemmodgrouplist.forEach((moditem,modindex) =>{
							if(moditem.defaultmodifier){
								moditem.showexitem = true
								moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
									if(sonitem.modifiercode == moditem.defaultmodifier && !sonitem.checked){
										var edata = {
											currentTarget:{
												dataset:{
													codeid:sonitem.id,
													groupid:moditem.id
												}
											}
										}
										that.modifiercodesclick(edata)
									}
								})
							}
						})
						
						
						
						//20211116废除orderdefault 字段
						// that.orderitem.itemmodgrouplist.forEach((moditem,modindex) =>{
						// 	moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
						// 		if(sonitem.orderdefault && !sonitem.checked){
						// 			moditem.showexitem = true
						// 			var edata = {
						// 				currentTarget:{
						// 					dataset:{
						// 						codeid:sonitem.id,
						// 						groupid:moditem.id
						// 					}
						// 				}
						// 			}
						// 			that.modifiercodesclick(edata)
						// 		}
						// 	})
						// })
					}
					
					if(that.orderitem.itemsizelist){
						that.orderitem.iteminfo.error = false
						
						that.orderitem.itemsizelist.forEach((sizeitem,sizeindex) =>{
							if (sizeitem.itemmodgrouplist) {
								try{
									sizeitem.itemmodgrouplist = JSON.parse(sizeitem.itemmodgrouplist)
								}catch(exxx){
									
								}
								sizeitem.itemmodgrouplist.forEach((item, index) => {
									item.modifiercodeslist.forEach((moditem, modindex) => {
										moditem.DisableCheck = false
										moditem.repeatcount = 1
										
										
									})
									
									//这里判断单独的改餐是否收缩 S
										if(itemmodifiergroupshrink){
											//如果modgroup大于 X 进入是否收缩判断
											if(sizeitem.itemmodgrouplist.length >= itemmodifiergroupshrinknum){
												//如果子项大于1，收缩
												if(item.modifiercodeslist && item.modifiercodeslist.length > 1){
													item.showexitem = false
												}else{
													item.showexitem = true
												}
											}else{
												item.showexitem = true
											}
										}else{
											item.showexitem = true
										}
									
									//这里判断单独的改餐是否收缩 E
								})
							}
						})
						
						
						
						that.orderitem.itemsizelist.forEach((item, index) => {
							item.checked = false
							if (item.sizecode == that.redeemsizecode) {
								// that.selectsize = item.id
								// item.checked = true
								var edata = {
									currentTarget:{
										dataset:{
											id:item.id,
										}
									}
								}
								that.sizeclick(edata)
								
								 // @tap="sizeclick" :data-id="item.id"
							}
						})
						
						if(that.orderitem.iteminfo.forcesize){
							that.orderitem.itemsizelist.forEach((item, index) => {
								if(index == 0){
									var edata = {
										currentTarget:{
											dataset:{
												id:item.id,
											}
										}
									}
									that.sizeclick(edata)
								}
							})
						}
					}
					
					
					if (that.orderitem.itemcombolist) {
						that.orderitem.itemcombolist.forEach((item, index) => {
							//20230419 新 合并combo判断
							if(itemmodifiergroupshrink){
								//如果modgroup大于 X 进入是否收缩判断
								if(alllistlength >= itemmodifiergroupshrinknum && item.exchangeitemlist && item.exchangeitemlist.length > 1){
									//如果子项大于1，收缩
									item.showexitem = false
								}else{
									item.showexitem = true
								}
							}else{
								item.showexitem = true
							}
							
							//20230419 新 合并combo判断 E
							
							if (item.exchangeitemlist) {
								item.exchangeitemlist.forEach((exitem, exindex) => {
									exitem.repeatcount = 1
					
					
									exitem.DisableCheck = false
									exitem.checked = false
					
									if (item.exchangeitemlist.length == 1 && item.minqty !=
										0 && item.minqty == item.maxqty) {
										exitem.repeatcount = item.minqty
										item.canrepeat = false
										item.canrepeatshow = true
									}
					
									if (exitem.iteminfo) {
										exitem.iteminfo = exitem.iteminfo[0]
					
										if (exitem.iteminfo.itemtype == 'Z' && exitem.iteminfo
											.itemsizelist && exitem.iteminfo.itemsizelist
											.length > 0) {
											exitem.iteminfo.itemsizelist.forEach((sizeitem,
												sizeindex) => {
												if(sizeindex == 0){
													sizeitem.checked = true
												}else{
													sizeitem.checked = false
												}
											})
										}
										if (exitem.iteminfo.modlist && exitem.iteminfo.modlist
											.length > 0) {
											exitem.iteminfo.modlist.forEach((exmoditem,
												exmodindex) => {
												exmoditem.modifiercodeslist.forEach((
													moditem, modindex) => {
													moditem.DisableCheck =
														false
													moditem.repeatcount = 1
												})
											})
										}
					
									}
								})
								if (item.exchangeitemlist.length == 1 && item.minqty != 0) {
									item.exchangeitemlist[0].checked = true
								}
							}
						})
					}
					
					
					try{
						setTimeout(function() {
							var itemdetail = document.getElementById('itemdetail');
							if (itemdetail.offsetHeight >= that.screenHeight) {
								that.cardfixed = true
								that.offheight = uni.upx2px(47);
							} else {
								that.cardfixed = false
								that.offheight =  uni.upx2px(47);
							}
						
							setTimeout(function() {
								itemdetail = document.getElementById('itemdetail');
								if (itemdetail.offsetHeight >= that.screenHeight) {
									that.cardfixed = true
									
									that.offheight = uni.upx2px(47);
								} else {
									that.cardfixed = false
									that.offheight =  uni.upx2px(47);
								}
							}, 500)
						}, 100)
					}catch(eeee){
						
					}
					return
				}
				
				uni.showLoading({
					title: language.GetTextByLanguage('加载中')
				})
				request.requestPost(api.ApiUrl + "Order/GetMenuItemInfo", data, function(res) {
					uni.hideLoading()
					if (res.statusCode == 200) {
						res = res.data
						
									
						
						try {
							var itemstatus = res.iteminfo[0].itemstatus
							if (itemstatus) {
								if (itemstatus == 'S') {
									uni.showModal({
										title: that.GetTextByLanguage('消息'),
										content: that.GetTextByLanguage('该餐项已售罄'),
										showCancel: false,
										confirmText: that.GetTextByLanguage('确认')
									})
									that.close()
									return
								}
								if (itemstatus == 'I') {
									uni.showModal({
										title: that.GetTextByLanguage('消息'),
										content: that.GetTextByLanguage('该餐项目前不可用'),
										showCancel: false,
										confirmText: that.GetTextByLanguage('确认')
									})
									that.close()
									return
								}
							}
						} catch (e) {

						}
						that.$emit('refreshmenuitem',res)

						that.orderitem = res
						that.orderitem.iteminfo = that.orderitem.iteminfo[0]
						
						
						var alllistlength = 0
						try{
							if(that.orderitem.itemcombolist){
								alllistlength += that.orderitem.itemcombolist.length
							}
						    if(that.orderitem.itemmodgrouplist){
						        alllistlength += that.orderitem.itemmodgrouplist.length
						    }
						}catch(liste){}
						
						if(ispointsexchange){
							that.ispointsexchange = true
							//that.redeempoints = that.orderitem.iteminfo.redeempoints
							
							
							try{
								if(parseFloat(that.GetPriceByDic(that.orderitem.iteminfo.pricedic)) < 0){
									that.showspecial = false
								}
							}catch(e){
								
							}
						}

						if (that.orderitem.itemmodgrouplist) {
							that.orderitem.itemmodgrouplist.forEach((item, index) => {
								item.modifiercodeslist.forEach((moditem, modindex) => {
									moditem.DisableCheck = false
									moditem.repeatcount = 1
								})
								
								//这里判断单独的改餐是否收缩 S
								// if(!that.orderitem.itemcombolist || that.orderitem.itemcombolist.length <= 0){
								// 	//如果要自动收缩
								// 	if(itemmodifiergroupshrink){
								// 		//如果modgroup大于 X 进入是否收缩判断
								// 		if(that.orderitem.itemmodgrouplist.length >= itemmodifiergroupshrinknum){
								// 			//如果子项大于1，收缩
								// 			if(item.modifiercodeslist && item.modifiercodeslist.length > 1){
								// 				item.showexitem = false
								// 			}else{
								// 				item.showexitem = true
								// 			}
								// 		}else{
								// 			item.showexitem = true
								// 		}
								// 	}else{
								// 		item.showexitem = true
								// 	}
								// }
								//这里判断单独的改餐是否收缩 E
								
								//20230419 新 合并combo判断
								if(itemmodifiergroupshrink){
									//如果modgroup大于 X 进入是否收缩判断
									if(alllistlength >= itemmodifiergroupshrinknum){
										//如果子项大于1，收缩
										if(item.modifiercodeslist && item.modifiercodeslist.length > 1){
											item.showexitem = false
										}else{
											item.showexitem = true
										}
									}else{
										item.showexitem = true
									}
								}else{
									item.showexitem = true
								}
								
								//20230419 新 合并combo判断 E
								
								
							})
							
							//这下面用来判断 MG改餐 是否需要默认选中一项
							that.orderitem.itemmodgrouplist.forEach((moditem,modindex) =>{
								if(moditem.minselection == 1 && moditem.maxselection == 1){
									if(moditem.modifiercodeslist && moditem.modifiercodeslist.length == 1){
										moditem.noshowreq = true  //如果只有一项 ，不显示 必选 的文字
										moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
											var edata = {
												currentTarget:{
													dataset:{
														codeid:sonitem.id,
														groupid:moditem.id
													}
												}
											}
											sonitem.noshowcheckbox = true
										console.log('2224')
											that.modifiercodesclick(edata)
										})
									}else if(moditem.modifiercodeslist && moditem.modifiercodeslist.length > 1){
										// moditem.showexitem = true
										// moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
										// 	if(sonindex == 0){
										// 		var edata = {
										// 			currentTarget:{
										// 				dataset:{
										// 					codeid:sonitem.id,
										// 					groupid:moditem.id
										// 				}
										// 			}
										// 		}
										// 		that.modifiercodesclick(edata)
										// 	}
										// })
										if(moditem.defaultmodifier){
											//移到外层处理
											// moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
											// 	if(sonitem.modifiercode == moditem.defaultmodifier){
											// 		var edata = {
											// 			currentTarget:{
											// 				dataset:{
											// 					codeid:sonitem.id,
											// 					groupid:moditem.id
											// 				}
											// 			}
											// 		}
											// 		that.modifiercodesclick(edata)
											// 	}
											// })
										}else{
											if(that.defaultselectfirstmod){
												moditem.showexitem = true
												moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
													if(sonindex == 0){
														var edata = {
															currentTarget:{
																dataset:{
																	codeid:sonitem.id,
																	groupid:moditem.id
																}
															}
														}
														that.modifiercodesclick(edata)
													}
												})
											}
										}
									}
								}
							})
							
							//选中默认的modifier
							that.orderitem.itemmodgrouplist.forEach((moditem,modindex) =>{
								if(moditem.defaultmodifier){
									moditem.showexitem = true
									moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
										if(sonitem.modifiercode == moditem.defaultmodifier && !sonitem.checked){
											var edata = {
												currentTarget:{
													dataset:{
														codeid:sonitem.id,
														groupid:moditem.id
													}
												}
											}
											that.modifiercodesclick(edata)
										}
									})
								}
							})
							// that.orderitem.itemmodgrouplist.forEach((moditem,modindex) =>{
							// 	moditem.modifiercodeslist.forEach((sonitem,sonindex) =>{
							// 		if(sonitem.orderdefault && !sonitem.checked){
							// 			moditem.showexitem = true
							// 			var edata = {
							// 				currentTarget:{
							// 					dataset:{
							// 						codeid:sonitem.id,
							// 						groupid:moditem.id
							// 					}
							// 				}
							// 			}
							// 			that.modifiercodesclick(edata)
							// 		}
							// 	})
							// })
						}
						
						
						if(that.orderitem.itemsizelist){
							that.orderitem.iteminfo.error = false
							
							
							that.orderitem.itemsizelist.forEach((sizeitem,sizeindex) =>{
								if (sizeitem.itemmodgrouplist) {
									try{
										sizeitem.itemmodgrouplist = JSON.parse(sizeitem.itemmodgrouplist)
									}catch(exxx){
										
									}
									sizeitem.itemmodgrouplist.forEach((item, index) => {
										item.modifiercodeslist.forEach((moditem, modindex) => {
											moditem.DisableCheck = false
											moditem.repeatcount = 1
											
											
											console.log(moditem)
										})
										
										//这里判断单独的改餐是否收缩 S
											if(itemmodifiergroupshrink){
												//如果modgroup大于 X 进入是否收缩判断
												if(sizeitem.itemmodgrouplist.length >= itemmodifiergroupshrinknum){
													//如果子项大于1，收缩
													if(item.modifiercodeslist && item.modifiercodeslist.length > 1){
														item.showexitem = false
													}else{
														item.showexitem = true
													}
												}else{
													item.showexitem = true
												}
											}else{
												item.showexitem = true
											}
										
										//这里判断单独的改餐是否收缩 E
									})
									// <checkbox :value="item.id" :disabled="true"
									// 	@tap="modifiercodesclick" :data-groupid="moditem.id"
									// 	:data-codeid="item.id" :checked="item.checked"
								}
							})
							
							that.orderitem.itemsizelist.forEach((item, index) => {
								item.checked = false
								if (item.sizecode == that.redeemsizecode) {
									var edata = {
										currentTarget:{
											dataset:{
												id:item.id,
											}
										}
									}
									that.sizeclick(edata)
								}
							})
							
							if(that.orderitem.iteminfo.forcesize){
								that.orderitem.itemsizelist.forEach((item, index) => {
									if(index == 0){
										var edata = {
											currentTarget:{
												dataset:{
													id:item.id,
												}
											}
										}
										that.sizeclick(edata)
									}
								})
							}
						}
						if (that.orderitem.itemcombolist) {
							that.orderitem.itemcombolist.forEach((item, index) => {
								
								//20230419 新 合并combo判断
								if(itemmodifiergroupshrink){
									//如果modgroup大于 X 进入是否收缩判断
									if(alllistlength >= itemmodifiergroupshrinknum && item.exchangeitemlist && item.exchangeitemlist.length > 1){
										//如果子项大于1，收缩
										item.showexitem = false
									}else{
										item.showexitem = true
									}
								}else{
									item.showexitem = true
								}
								
								//20230419 新 合并combo判断 E
								
								if (item.exchangeitemlist) {
									item.exchangeitemlist.forEach((exitem, exindex) => {
										exitem.repeatcount = 1


										exitem.DisableCheck = false
										exitem.checked = false

										if (item.exchangeitemlist.length == 1 && item.minqty !=
											0 && item.minqty == item.maxqty) {
											exitem.repeatcount = item.minqty
											
											console.log(exitem.repeatcount)
											
											item.canrepeat = false
											item.canrepeatshow = true
										}

										if (exitem.iteminfo) {
											exitem.iteminfo = exitem.iteminfo[0]

											if (exitem.iteminfo.itemtype == 'Z' && exitem.iteminfo
												.itemsizelist && exitem.iteminfo.itemsizelist
												.length > 0) {
												exitem.iteminfo.itemsizelist.forEach((sizeitem,
													sizeindex) => {
													if(sizeindex == 0){
														sizeitem.checked = true
													}else{
														sizeitem.checked = false
													}
												})
											}
											if (exitem.iteminfo.modlist && exitem.iteminfo.modlist
												.length > 0) {
												exitem.iteminfo.modlist.forEach((exmoditem,
													exmodindex) => {
													exmoditem.modifiercodeslist.forEach((
														moditem, modindex) => {
														moditem.DisableCheck =
															false
														moditem.repeatcount = 1
													})
												})
											}

										}
									})
									if (item.exchangeitemlist.length == 1 && item.minqty != 0) {
										item.exchangeitemlist[0].checked = true
									}
								}
							})
						}


						setTimeout(function() {
							var itemdetail = document.getElementById('itemdetail');
							if (itemdetail.offsetHeight >= that.screenHeight) {
								that.cardfixed = true
								that.offheight = uni.upx2px(47);
							} else {
								that.cardfixed = false
								
								that.offheight =  uni.upx2px(47);
							}

							setTimeout(function() {
								itemdetail = document.getElementById('itemdetail');
								if (itemdetail.offsetHeight >= that.screenHeight) {
									that.cardfixed = true
									that.offheight = uni.upx2px(47);
								} else {
									that.cardfixed = false
									that.offheight =  uni.upx2px(47);
								}
							}, 500)
						}, 100)
					}
				})
			},
			
