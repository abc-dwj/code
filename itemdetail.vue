<template>
	<view v-if="showPopup && orderitem && orderitem.iteminfo" class="uni-popup" :class="[popupstyle]"
		@touchmove.stop.prevent="clear">
		<!-- #ifdef H5 -->

		<uni-transition v-if="maskShow" :mode-class="['fade']" :styles="maskClass" :duration="duration"
			:show="showTrans" @click="onTap" />
		<uni-transition :mode-class="ani" :styles="transClass" :duration="duration" :show="showTrans" @click="onTap">
			<view class="uni-popup__wrapper-box" @click.stop="clear">
				<slot />
				<template>
					<scroll-view id="itemdetail"
						:style="'max-height: '+screenHeight+'px;display: flex;width: 650rpx;background-color: #FFFFFF;flex-direction: column;'"
						:scroll-top="0" :scroll-y="true">
						<view v-if="!freeitem && orderitem.iteminfo.menuitemimgurl"
							style="width: 650rpx;height: 487.5rpx;order-color: #000000;">
							<image :src="orderitem.iteminfo.menuitemimgurl" style="width: 650rpx;height: 487.5rpx;"
								mode="scaleToFill">
							</image>
						</view>
						
						<!--free 专用 S -->
						<view style="justify-content: center;align-items: center;" v-if="freeitem && freeitem.imgurl">
							<view class="promo-preview"
								:style="'background-image: url('+freeitem.imgurl+');'">
								<view class="promo-preview-details">
									<view class="promo-preview-details-title">
										<text class="promo-preview-details-title-span">{{freeitem.salename}}</text>
									</view>
									<view class="promo-preview-details-description">
										<text class="promo-preview-details-description-span">{{freeitem.description}}</text>
									</view>
								</view>
							</view>
						</view>
						<!--free 专用 E -->
						
						<view
							style="width: 650rpx;justify-content: center;align-items: center;border-bottom-width: 2rpx;border-color: rgb(236, 236, 236);border-style: solid;">
							<view
								style="justify-content: space-between;flex-direction: row;min-height: 50rpx;align-items: center;width: 600rpx;margin-top: 30rpx;">
								<text
									style="font-size: 32rpx;font-weight: bold;width: 470rpx;">{{GetNameByDic(orderitem.iteminfo.namedic,true)}}</text>
								<text
									v-if="!isfreesale && (orderitem.iteminfo.itemtype != 'Z' || (orderitem.iteminfo.itemtype == 'Z' && !orderitem.iteminfo.forcesize)) && !ispointsexchange"
									style="font-size: 30rpx;font-weight: bold;">${{GetPriceByDic(orderitem.iteminfo.pricedic)}}</text>
								<text
									v-if="isfreesale"
									style="font-size: 30rpx;font-weight: bold;color: rgb(0, 100, 0);">{{GetTextByLanguage('免费')}}</text>
								<text style="font-size: 30rpx;font-weight: bold;text-align: center;" v-if="ispointsexchange">{{redeempoints}} {{GetTextByLanguage('积分')}}</text>
							</view>
							<!-- <view
								style="justify-content: flex-start;flex-direction: row;height: 50rpx;align-items: center;width: 600rpx;">
								<text>Apple</text>
							</view> -->
							<view
								style="justify-content: space-between;align-items: center;width: 600rpx;margin-top: 15rpx;margin-bottom: 15rpx;">
								<text
									style="font-size: 26rpx;">{{GetNameByDic(orderitem.iteminfo.descriptiondic,true)}}</text>
							</view>
							<view v-if="taglistcodes" style="width: 95%;display: flex;flex-direction: row;align-items: center;margin-top: 5rpx;margin-bottom: 5rpx;flex-wrap: wrap;padding-left: 1%;">
								<template v-for="(tagcodeitem,tagcodeindex) in taglistcodes.split(',')">
								<view v-for="(tagitem,tagindex) in taglist" v-if="tagcodeitem == tagitem.id" style="display: flex;flex-direction: row;width: 48%;margin-top: 10rpx;justify-content: flex-start;align-items: center;">
									<image v-if="tagitem.imgurl" :src="tagitem.imgurl" mode="aspectFit" style="width:38rpx;height: 38rpx;"></image>
									<text v-if="tagitem.imgurl" style="font-size: 24rpx;font-weight: bold;width: 26rpx;color: #9b9797;text-align: center;">-</text>
									<text style="font-size: 24rpx;font-weight: bold;color: #9b9797;">{{GetNameByDic(tagitem.namedic)}}</text>
								</view>
								</template>
							</view>
						</view>
						<template>
							<view style="display: flex;width: 650rpx;justify-content: center;align-items: center;">

								<!--大小 S-->
								<template v-if="orderitem.iteminfo.itemtype == 'Z'">
									<view :class="orderitem.iteminfo.error?'listposition':''"
										style="display: flex;justify-content: flex-start;flex-direction: row;height: 55rpx;width: 600rpx;margin-top: 30rpx;">
										<text style="font-size: 30rpx;">{{GetTextByLanguage('大小')}}</text><text
											style="margin-left: 10rpx;font-size: 24rpx;">{{orderitem.iteminfo.forcesize?'(' +GetTextByLanguage('必要') + ')':''}}
										</text>
									</view>
									<!-- <view
									style="justify-content: flex-start;flex-direction: row;height: 50rpx;align-items: center;width: 600rpx;">
									<text>space-between 2-2(required)</text>
								</view> -->
									<view
										style="display: flex;width:650rpx;flex-direction: column;justify-content: center;align-items: center;">
										<radio-group>
											<view :class="orderitem.iteminfo.error?'listerror':''"
												style="border-width: 2rpx;border-radius: 10rpx;border-color: #CDCDCD;">
												<view v-for="(item,index) in orderitem.itemsizelist" v-if="redeemsizecode == null || item.sizecode == redeemsizecode"
													style="display: flex;width: 600rpx;flex-direction: row;justify-content: space-between;align-items: center;min-height: 70rpx;border-style: solid;border-bottom-width: 2rpx;border-color: #CDCDCD;padding-left: 20rpx;padding-right: 20rpx;">
													<view @tap="sizeclick" :data-id="item.id"
														style="display: flex;flex-direction: row;justify-content: flex-start;align-items: center;width: 450rpx;">
														<radio :disabled="true" :checked="item.checked">
														</radio><text
															style="word-break: break-word;font-size: 25rpx;margin-left: 10rpx;width: 370rpx;">{{GetNameByDic(item.namedic,true)}}</text>
													</view>
													<view style="width: 150rpx;"><text v-if="!ispointsexchange && redeemid == null && !isfreesale"
															style="font-size: 25rpx;">${{GetPriceByDic(item.pricedic)}}</text>
													</view>
												</view>
											</view>
										</radio-group>
									</view>
								</template>
								<!--大小 E-->
								
								<!--改餐(做法) S-->
								<template v-if="orderitem.itemmodgrouplist && orderitem.itemmodgrouplist.length > 0"
									v-for="(moditem,modindex) in orderitem.itemmodgrouplist">
									<view :class="moditem.error?'listposition':''" @tap="showcomboitem(moditem)"
										style="display: flex;justify-content: flex-start;flex-direction: row;width: 600rpx;margin-top: 30rpx;margin-bottom: 30rpx;align-items: center;">
										<image v-if="!moditem.showexitem" src="../../static/order/rightarraw.png"
											style="width: 30rpx;height: 30rpx;margin-right: 10rpx;" mode="aspectFill">
										</image>
										<image v-if="moditem.showexitem" src="../../static/order/bottomarraw.png"
											style="width: 30rpx;height: 25rpx;margin-right: 10rpx;position: relative;top: 2rpx;"
											mode="aspectFill"></image>
										<view
											style="width: 350rpx;flex-direction: row;justify-content: flex-start;align-items: center;">
											<text
												style="width: 280rpx;font-size: 30rpx;font-weight: bold;word-wrap: break-word;">{{GetNameByDic(moditem.namedic,true)}}</text>
											<view
												style="width: 50rpx;height: 40rpx;border-radius: 15rpx;background-color: #000000;justify-content: center;align-items: center;position: absolute;right: 20rpx;"
												v-if="moditem.reqcheckcount"><text
													style="color: #FFFFFF;font-size: 25rpx;">x{{moditem.reqcheckcount}}</text>
											</view>
										</view>
										<template>
											<!-- 这里是3种文字，提示改餐可以选X个-->
											<view
												v-if="moditem.minselection == moditem.maxselection && moditem.minselection != 0 && !moditem.noshowreq"
												style="position: absolute;right: 1px;background: red;border-radius: 10rpx;box-shadow: red;color: #fff;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
												<text
													style="font-size: 23rpx;">{{moditem.minselection}}&nbsp;{{GetTextByLanguage('必选')}}</text>
											</view>
											<view v-else-if="moditem.minselection <= 0 && moditem.maxselection > 0"
												style="position: absolute;right: 1px;border-radius: 10rpx;box-shadow: red;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
												<text
													style="font-size: 25rpx;color: #000000;">{{GetTextByLanguage('最多')}}&nbsp;({{moditem.maxselection}})</text>
											</view>
											<view
												v-else-if="moditem.minselection >= 1 && moditem.maxselection > moditem.minselection"
												style="position: absolute;right: 1px;border-radius: 10rpx;box-shadow: red;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
												<text
													style="font-size: 25rpx;color: red;">{{GetTextByLanguage('最少')}}&nbsp;({{moditem.minselection}})&nbsp;,&nbsp;{{GetTextByLanguage('最多')}}&nbsp;({{moditem.maxselection}})</text>
											</view>
											<view
												v-else-if="moditem.minselection > 0 && moditem.maxselection == 0"
												style="position: absolute;right: 1px;border-radius: 10rpx;box-shadow: red;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
												<text
													style="font-size: 25rpx;color: red;">{{GetTextByLanguage('最少')}}&nbsp;({{moditem.minselection}})</text>
											</view>
										</template>
									</view>
									<!-- <view
										style="justify-content: flex-start;flex-direction: row;height: 50rpx;align-items: center;width: 600rpx;">
										<text>space-between 2-2(required)</text>
									</view> -->
									<view v-if="moditem.showexitem"
										style="display: flex;width:650rpx;flex-direction: column;justify-content: center;align-items: center;">
										<view style="border-width: 2rpx;border-radius: 10rpx;border-color: #CDCDCD;"
											:class="moditem.error?'listerror':''">
											<checkbox-group>
												<view v-for="(item,index) in moditem.modifiercodeslist"
													style="display: flex;width: 600rpx;flex-direction: row;justify-content: space-between;min-height: 70rpx;border-style: solid;border-bottom-width: 2rpx;border-color: #CDCDCD;padding-left: 20rpx;padding-right: 20rpx;">
													<view
														style="display: flex;flex-direction: row;justify-content: flex-start;align-items: center;width: 420rpx;">
														<checkbox :value="item.id" :disabled="true" v-if="!item.noshowcheckbox"
															@tap="modifiercodesclick" :data-groupid="moditem.id"
															:data-codeid="item.id" :checked="item.checked">
														</checkbox>
														<text @tap="modifiercodesclick"
															v-if="!item.DisableCheck && !item.noshowcheckbox" :data-groupid="moditem.id"
															:data-codeid="item.id"
															:style="'font-size: 26rpx;margin-left: 10rpx;width: 370rpx;' +(item.DisableCheck?'opacity:0.5':'')">{{GetNameByDic(item.namedic,true)}}</text>
														<text
															v-if="!item.DisableCheck && item.noshowcheckbox" :data-groupid="moditem.id"
															:data-codeid="item.id"
															:style="'font-size: 26rpx;margin-left: 10rpx;width: 370rpx;' +(item.DisableCheck?'opacity:0.5':'')">{{GetNameByDic(item.namedic,true)}}</text>
														<text v-if="item.DisableCheck" :data-groupid="moditem.id"
															:data-codeid="item.id"
															:style="'font-size: 26rpx;margin-left: 10rpx;width: 370rpx;' +(item.DisableCheck?'opacity:0.5':'')">{{GetNameByDic(item.namedic,true)}}</text>
								
													</view>
													<view
														style="width: 205rpx;display: flex;flex-direction: column;justify-content: center;min-height: 70rpx;align-items: center;padding-right: 50rpx;">
														<view v-if="moditem.canrepeat && !item.DisableCheck"
															style="width: 200rpx;height: 20rpx;">
														</view>
														<view
															v-if="moditem.canrepeat  && !item.DisableCheck  && item.checked && (moditem.maxselection > 1 || (moditem.minselection == 0 && moditem.maxselection == 0) || (moditem.minselection > 0 && moditem.maxselection == 0))"
															style="display: flex;flex-direction: row;">
															<button @tap="modgroupcodelistrepeatcount" data-method="-"
																:data-groupid="moditem.id" :data-codeid="item.id"
																style="display: flex;width: 50rpx;height: 50rpx;justify-content: center;align-items: center;display: flex;border-radius: 0rpx;line-height: 50rpx;"><text>-</text></button>
															<button @tap="modgroupcodelistrepeatcount" data-method="+"
																:data-groupid="moditem.id" :data-codeid="item.id"
																style="margin-left: 10rpx;display: flex;width: 50rpx;height: 50rpx;justify-content: center;align-items: center;display: flex;border-radius: 0rpx;line-height: 50rpx;"><text>+</text></button>
														</view>
														<view
															style="flex-direction: row;align-items: center;display: flex;margin-top: 5rpx;"
															v-if="!item.DisableCheck">
															<text v-if="item.modprice > 0"
																:style="'font-size: 25rpx;'+ (moditem.canrepeat?'marign-top:10rpx;':'')+(item.DisableCheck?'opacity:0.5':'')">+${{returnFloat(item.modprice)}}</text>
															<text v-if="item.checked"
																style="font-size: 24rpx;margin-left: 20rpx;">x{{item.repeatcount}}</text>
														</view>
														<view v-if="moditem.canrepeat"
															style="width: 200rpx;height: 20rpx;">
														</view>
													</view>
												</view>
											</checkbox-group>
										</view>
									</view>
								</template>
								<!--改餐(做法) E-->
								<!--套餐(Combo) S-->
								<template v-if="orderitem.itemcombolist && orderitem.itemcombolist.length > 0"
									v-for="(moditem,modindex) in orderitem.itemcombolist">
									<!-- moditem.exchangeitemlist.length > 1 -->
									<view v-if="moditem.exchangeitemlist && (moditem.exchangeitemlist.length > 1 || !(moditem.exchangeitemlist.length == 1 && moditem.minqty > 0 && moditem.minqty == moditem.maxqty))"  
										@tap="showcomboitem(moditem)" :class="moditem.error?'listposition':''"
										style="display: flex;justify-content: flex-start;flex-direction: row;min-height: 55rpx;width: 600rpx;margin-top: 30rpx;align-items: center;box-sizing: border-box;padding-bottom: 10rpx;">
										<image v-if="!moditem.showexitem" src="../../static/order/rightarraw.png"
											style="width: 30rpx;height: 30rpx;margin-right: 10rpx;" mode="aspectFill">
										</image>
										<image v-if="moditem.showexitem" src="../../static/order/bottomarraw.png"
											style="width: 30rpx;height: 25rpx;margin-right: 10rpx;position: relative;top: 2rpx;"
											mode="aspectFill"></image>
										<view
											style="width: 350rpx;flex-direction: row;justify-content: flex-start;align-items: center;">
											<text
												style="width: 280rpx;font-size: 30rpx;font-weight: bold;">{{GetNameByDic(moditem.namedic,true)}}</text>
											<view
												style="width: 50rpx;height: 40rpx;border-radius: 15rpx;background-color: #000000;justify-content: center;align-items: center;position: absolute;right: 20rpx;"
												v-if="moditem.reqcheckcount"><text
													style="color: #FFFFFF;font-size: 25rpx;">x{{moditem.reqcheckcount}}</text>
											</view>
										</view>
										<template>
											<!-- 这里是3种文字，提示套餐可以选X个-->
											<view v-if="moditem.minqty == moditem.maxqty && moditem.minqty != 0"
												style="position: absolute;right: 1px;background: red;border-radius: 10rpx;box-shadow: red;color: #fff;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
												<text
													style="font-size: 23rpx;">{{moditem.minqty}}&nbsp;{{GetTextByLanguage('必选')}}</text>
											</view>
											<view v-else-if="moditem.minqty <= 0 && moditem.maxqty > 0"
												style="position: absolute;right: 1px;border-radius: 10rpx;box-shadow: red;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
												<text
													style="font-size: 25rpx;color: #000000;">{{GetTextByLanguage('最多')}}&nbsp;({{moditem.maxqty}})</text>
											</view>
											<view v-else-if="moditem.minqty >= 1 && moditem.maxqty > moditem.minqty"
												style="position: absolute;right: 1px;border-radius: 10rpx;box-shadow: red;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
												<text
													style="font-size: 25rpx;color: red;">{{GetTextByLanguage('最少')}}&nbsp;({{moditem.minqty}})&nbsp;,&nbsp;{{GetTextByLanguage('最多')}}&nbsp;({{moditem.maxqty}})</text>
											</view>
											<view
												v-else-if="moditem.minqty > 0 && moditem.maxqty == 0"
												style="position: absolute;right: 1px;border-radius: 10rpx;box-shadow: red;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
												<text
													style="font-size: 25rpx;color: red;">{{GetTextByLanguage('最少')}}&nbsp;({{moditem.minqty}})</text>
											</view>
										</template>
									</view>
									<view v-else style="margin-top: 30rpx;">
									</view>
									<!-- <view
										style="justify-content: flex-start;flex-direction: row;height: 50rpx;align-items: center;width: 600rpx;">
										<text>space-between 2-2(required)</text>
									</view> -->
									<transition name="plus-icon">
										<view
											v-if="!moditem.exchangeitemlist || (moditem.minqty == 1 && moditem.maxqty == 1 && moditem.exchangeitemlist.length <= 1) || moditem.showexitem"
											style="display: flex;width:600rpx;flex-direction: column;justify-content: center;align-items: center;">
											<view :class="moditem.error?'listerror':''">
												<checkbox-group>
													<view v-for="(item,index) in moditem.exchangeitemlist"
														:style="'width: 600rpx;flex-direction: column;display: flex;justify-content: center;align-items: center;border-width: 2rpx;border-radius: 10rpx;border-color: #'+(moditem.exchangeitemlist.length > 1 && item.checked?'962929':'CDCDCD')+';' + (index != 0?'margin-top: 20rpx;':'')"
														v-if="item.iteminfo">
														<view
															:style="'display: flex;width: 600rpx;flex-direction: row;justify-content: space-between;padding-left: 20rpx;padding-right: 20rpx;'">
															<view
																style="display: flex;flex-direction: row;justify-content: flex-start;align-items: center;width: 420rpx;">
																<checkbox v-if="moditem.exchangeitemlist.length > 1 || moditem.minqty == 0"
																	:disabled="true" @tap="comcheckbox"
																	:data-comboid="moditem.id" :data-id="item.id" style="cursor: pointer;"
																	:value="item.id" :checked="item.checked">
																</checkbox>
																<text @tap="comcheckbox"
																	v-if="!item.DisableCheck && (moditem.exchangeitemlist.length > 1  || moditem.minqty == 0)"
																	:data-comboid="moditem.id" :data-id="item.id"
																	:style="'word-break:break-word;font-size: 28.5rpx;margin-left: 10rpx;width: 370rpx;color:#962929;'+(item.DisableCheck?'opacity:0.5':'')">{{GetNameByDic(item.iteminfo.namedic,true)}}</text>
																<text
																	v-if="!item.DisableCheck && (moditem.exchangeitemlist.length <= 1 && moditem.minqty != 0)"
																	:data-comboid="moditem.id" :data-id="item.id"
																	:style="'font-size: 28.5rpx;margin-left: 10rpx;width: 370rpx;'+(item.DisableCheck?'opacity:0.5':'')">{{GetNameByDic(item.iteminfo.namedic,true)}}</text>
																<text v-if="item.DisableCheck"
																	:data-comboid="moditem.id" :data-id="item.id"
																	:style="'word-break:break-word;font-size: 28.5rpx;margin-left: 10rpx;width: 370rpx;'+(item.DisableCheck?'opacity:0.5':'')">{{GetNameByDic(item.iteminfo.namedic,true)}}</text>
															</view>
															<view
																style="width: 205rpx;display: flex;flex-direction: column;justify-content: center;min-height: 100rpx;align-items: center;padding-right: 50rpx;padding-top: 10rpx;padding-bottom: 10rpx;">
																<view
																	v-if="moditem.canrepeat && item.DisableCheck && item.checked && ((moditem.maxqty >= moditem.minqty && moditem.maxqty != 1) || (moditem.minqty > 0 && moditem.maxqty == 0))"
																	style="width: 200rpx;height: 20rpx;">
																</view>
																<view
																	v-if="moditem.canrepeat && !item.DisableCheck && item.checked && ((moditem.maxqty >= moditem.minqty && moditem.maxqty != 1) || (moditem.minqty > 0 && moditem.maxqty == 0))"
																	style="display: flex;flex-direction: row;">
																	<button @tap="exchangeitemcount" data-method="-"
																		:disabled="item.repeatcount <= 1"
																		:data-comid="moditem.id"
																		:data-exchangeid="item.id"
																		style="display: flex;width: 50rpx;height: 50rpx;justify-content: center;align-items: center;display: flex;border-radius: 0rpx;line-height: 50rpx;"><text>-</text></button>
																	<button @tap="exchangeitemcount" data-method="+"
																		:data-comid="moditem.id"
																		:data-exchangeid="item.id"
																		:disabled="item.repeatcount >= moditem.maxqty && moditem.maxqty != 0"
																		style="margin-left: 10rpx;display: flex;width: 50rpx;height: 50rpx;justify-content: center;align-items: center;display: flex;border-radius: 0rpx;line-height: 50rpx;"><text>+</text></button>
																</view>
																<text
																	v-if="(moditem.canrepeat || moditem.canrepeatshow) && item.checked && ((moditem.maxqty >= moditem.minqty && moditem.maxqty != 1) || (moditem.minqty > 0 && moditem.maxqty == 0)) && !item.DisableCheck"
																	:style="'font-size: 25rpx;'">x{{item.repeatcount}}</text>
																<!-- 暂时注释 -->
																<text v-if="item.itemprice && item.itemprice > 0"
																	style="font-size: 25rpx;">+${{returnFloat(item.itemprice)}}</text>
																<!-- <view
																	v-if="moditem.canrepeat && !item.DisableCheck && ((moditem.maxqty >= moditem.minqty && moditem.maxqty != 1) || (moditem.minqty > 0 && moditem.maxqty == 0))"
																	style="width: 200rpx;height: 20rpx;">
																</view> -->
															</view>
														</view>
														<view
															v-if="item.iteminfo.itemsizelist  && !item.DisableCheck && item.checked"
															style="display: flex;justify-content: center;align-items: center;">
															<!--大小 S-->
															<template v-if="item.iteminfo.itemtype == 'Z'">
																<view
																	style="display: flex;justify-content: flex-start;flex-direction: row;height: 55rpx;width: 550rpx;">
																	<text
																		style="margin-left: 5rpx;font-size: 27rpx;font-weight: bold;">{{GetTextByLanguage('大小')}}</text><text
																		style="margin-left: 10rpx;font-size: 24rpx;font-weight: bold;">{{item.iteminfo.forcesize?'(' +GetTextByLanguage('必要') + ')':''}}
																	</text>
																</view>
																<!-- <view
															style="justify-content: flex-start;flex-direction: row;height: 50rpx;align-items: center;width: 600rpx;">
															<text>space-between 2-2(required)</text>
														</view> -->
																<view
																	style="display: flex;width:650rpx;flex-direction: column;justify-content: center;align-items: center;">
																	<radio-group>
																		<view
																			style="border-width: 2rpx;border-radius: 10rpx;border-color: #CDCDCD;">
																			<view @tap="exchangesizeclick"
																				:data-comid="moditem.id"
																				:data-exchangeid="item.id"
																				:data-sizeid="sizeitem.id"
																				v-for="(sizeitem,sizeindex) in item.iteminfo.itemsizelist"
																				style="display: flex;width: 550rpx;flex-direction: row;justify-content: space-between;align-items: center;min-height: 70rpx;border-style: solid;border-bottom-width: 2rpx;border-color: #CDCDCD;padding-left: 20rpx;padding-right: 20rpx;">
																				<view
																					style="display: flex;flex-direction: row;justify-content: flex-start;align-items: center;width: 450rpx;min-height: 70rpx;">
																					<radio :disabled="true"
																						:value="sizeitem.id"
																						:checked="sizeitem.checked">
																					</radio><text
																						style="font-size: 25rpx;margin-left: 10rpx;width: 370rpx;word-break:break-word;">{{GetNameByDic(sizeitem.namedic,true)}}</text>
																				</view>
																				<view style="width: 150rpx;"><text v-if="!isfreesale"
																						style="font-size: 25rpx;">
																						<text v-if="sizeitem.sizeprice != null" style="font-size: 25rpx;">{{returnFloat(sizeitem.sizeprice) > 0?'$' + returnFloat(sizeitem.sizeprice):''}}</text>
																						<text style="font-size: 25rpx;" v-else>{{returnFloat(GetPriceByDic(sizeitem.pricedic)) > 0 ? '$' + GetPriceByDic(sizeitem.pricedic):''}}</text>
																						</text>
																				</view>
																			</view>
																		</view>
																	</radio-group>
																</view>
															</template>
															<!--大小 E-->
														</view>
														<view
															v-if="item.iteminfo.modlist  && !item.DisableCheck && item.checked"
															:style="'width: 550rpx;display: flex;justify-content: flex-start;align-items: center;'+(item.iteminfo.itemtype == 'Z'?'margin-top: 10rpx;':'')">
															<!--改餐(做法) S-->
															<template
																v-if="item.iteminfo.modlist && item.iteminfo.modlist.length > 0"
																v-for="(modcomitem,modcomindex) in item.iteminfo.modlist">
																<view @tap="showcomboitem(modcomitem)" :class="modcomitem.error?'listposition':''"
																	style="width: 550rpx;min-height: 55rpx;display: flex;justify-content: flex-start;flex-direction: row;margin-top: 10rpx;align-items: center;">
																	<image v-if="!modcomitem.showexitem" src="../../static/order/rightarraw.png"
																		style="width: 30rpx;height: 30rpx;margin-right: 10rpx;" mode="aspectFill">
																	</image>
																	<image v-if="modcomitem.showexitem" src="../../static/order/bottomarraw.png"
																		style="width: 30rpx;height: 25rpx;margin-right: 10rpx;"
																		mode="aspectFill"></image>
																	<view
																		style="width: 310rpx;flex-direction: row;justify-content: flex-start;align-items: center;position: relative;">
																		<text
																			style="width: 230rpx;font-size: 27rpx;font-weight: bold;overflow-wrap: break-word;">{{GetNameByDic(modcomitem.namedic,true)}}</text>
																			<view
																				style="width: 50rpx;height: 40rpx;border-radius: 15rpx;background-color: #000000;justify-content: center;align-items: center;position: absolute;right: 20rpx;"
																				v-if="modcomitem.reqcheckcount">
																				<text style="color: #FFFFFF;font-size: 25rpx;">x{{modcomitem.reqcheckcount}}</text>
																			</view>
																	</view>
																	
																	<template>
																		<!-- 这里是3种文字，提示套餐可以选X个-->
																		<view v-if="modcomitem.minselection == modcomitem.maxselection && modcomitem.minselection != 0"
																			style="position: absolute;right: 1px;background: red;border-radius: 10rpx;box-shadow: red;color: #fff;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
																			<text
																				style="font-size: 23rpx;">{{modcomitem.minselection}}&nbsp;{{GetTextByLanguage('必选')}}</text>
																		</view>
																		<view v-else-if="modcomitem.minselection <= 0 && modcomitem.maxselection > 0"
																			style="position: absolute;right: 1px;border-radius: 10rpx;box-shadow: red;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
																			<text
																				style="font-size: 25rpx;color: #000000;">{{GetTextByLanguage('最多')}}&nbsp;({{modcomitem.maxselection}})</text>
																		</view>
																		<view v-else-if="modcomitem.minselection >= 1 && modcomitem.maxselection > modcomitem.minselection"
																			style="position: absolute;right: 1px;border-radius: 10rpx;box-shadow: red;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
																			<text
																				style="font-size: 25rpx;color: red;">{{GetTextByLanguage('最少')}}&nbsp;({{modcomitem.minselection}})&nbsp;,&nbsp;{{GetTextByLanguage('最多')}}&nbsp;({{modcomitem.maxselection}})</text>
																		</view>
																		<view
																			v-else-if="modcomitem.minselection > 0 && modcomitem.maxselection == 0"
																			style="position: absolute;right: 1px;border-radius: 10rpx;box-shadow: red;box-sizing: border-box;padding: 4rpx 16rpx 4rpx 16rpx;font-weight: bold;">
																			<text
																				style="font-size: 25rpx;color: red;">{{GetTextByLanguage('最少')}}&nbsp;({{modcomitem.minselection}})</text>
																		</view>
																	</template>
																</view>
																<!-- <view
																style="justify-content: flex-start;flex-direction: row;height: 50rpx;align-items: center;width: 600rpx;">
																<text>space-between 2-2(required)</text>
															</view> -->
																<view v-if="modcomitem.showexitem"
																	style="display: flex;width:550rpx;flex-direction: column;justify-content: center;align-items: center;">
																	<view :class="modcomitem.error?'listerror':''"
																		style="border-width: 2rpx;border-radius: 10rpx;border-color: #CDCDCD;">
																		<checkbox-group :data-groupid="modcomitem.id"
																			:data-comboid="moditem.id"
																			:data-exchangeid="item.id">
																			<view
																				v-for="(codeitem,codeindex) in modcomitem.modifiercodeslist"
																				:style="'display: flex;width: 550rpx;flex-direction: row;justify-content: space-between;align-items: center;min-height: 70rpx;margin-top: 10rpx;border-style: solid;padding-left: 20rpx;padding-right: 20rpx;' + (codeindex != modcomitem.modifiercodeslist.length - 1?'border-bottom-width: 2rpx;border-color: #CDCDCD;':'')">
																				<view
																					style="display: flex;flex-direction: row;justify-content: flex-start;align-items: center;width: 420rpx;">
																					<checkbox :value="codeitem.id"
																						:disabled="true"
																						@tap="combomodifiercodesclick"
																						:data-groupid="modcomitem.id"
																						:data-comboid="moditem.id"
																						:data-exchangeid="item.id"
																						:data-codeid="codeitem.id"
																						:checked="codeitem.checked">
																					</checkbox><text
																						@tap="combomodifiercodesclick"
																						v-if="!codeitem.DisableCheck"
																						:data-groupid="modcomitem.id"
																						:data-comboid="moditem.id"
																						:data-exchangeid="item.id"
																						:data-codeid="codeitem.id"
																						:style="'font-size: 25rpx;margin-left: 10rpx;width: 300rpx;' +(codeitem.DisableCheck?'opacity:0.5':'')">{{GetNameByDic(codeitem.namedic,true)}}</text>
																					<text v-if="codeitem.DisableCheck"
																						:style="'font-size: 25rpx;margin-left: 10rpx;width: 300rpx;' +(codeitem.DisableCheck?'opacity:0.5':'')">{{GetNameByDic(codeitem.namedic,true)}}</text>
																				</view>
																				<view
																					style="width: 105rpx;display: flex;flex-direction: column;justify-content: center;min-height: 60rpx;align-items: center;padding-right: 50rpx;">
																					<view
																						v-if="modcomitem.canrepeat && !codeitem.DisableCheck "
																						style="width: 105rpx;height: 20rpx;">
																					</view>
																					<view
																						v-if="modcomitem.canrepeat  && !codeitem.DisableCheck && codeitem.checked && (modcomitem.maxselection >= modcomitem.minselection && modcomitem.maxselection != 1)"
																						style="display: flex;flex-direction: row;">
																						<button
																							@tap="combomodgroupcodelistrepeatcount"
																							data-method="-"
																							:data-groupid="modcomitem.id"
																							:data-comboid="moditem.id"
																							:data-exchangeid="item.id"
																							:data-codeid="codeitem.id"
																							style="display: flex;width: 50rpx;height: 50rpx;justify-content: center;align-items: center;display: flex;border-radius: 0rpx;line-height: 50rpx;"><text>-</text></button>
																						<button
																							@tap="combomodgroupcodelistrepeatcount"
																							data-method="+"
																							:data-groupid="modcomitem.id"
																							:data-comboid="moditem.id"
																							:data-exchangeid="item.id"
																							:data-codeid="codeitem.id"
																							style="margin-left: 10rpx;display: flex;width: 50rpx;height: 50rpx;justify-content: center;align-items: center;display: flex;border-radius: 0rpx;line-height: 50rpx;"><text>+</text></button>
																					</view>
																					<view
																						style="flex-direction: row;align-items: center;display: flex;margin-top: 5rpx;"
																						v-if="!codeitem.DisableCheck">
																						<text
																							v-if="codeitem.modprice > 0"
																							:style="'font-size: 25rpx;'+ (modcomitem.canrepeat?'marign-top:10rpx;':'')+(codeitem.DisableCheck?'opacity:0.5':'')">+${{returnFloat(codeitem.modprice)}}</text>
																						<text v-if="codeitem.checked"
																							style="font-size: 24rpx;margin-left: 20rpx;">x{{codeitem.repeatcount}}</text>
																					</view>
																					<view v-if="modcomitem.canrepeat"
																						style="width: 200rpx;height: 20rpx;">
																					</view>
																				</view>
																			</view>
																		</checkbox-group>
																	</view>
																</view>
																<view style="width: 750rpx;height: 30rpx;"></view>
															</template>
															<!--改餐(做法) E-->
														</view>
													</view>
												</checkbox-group>
											</view>
										</view>
									</transition>
								</template>
								<!--套餐(Combo) E-->

								<!-- <view style="650rpx;height: 100rpx;border-width: 2rpx;border-color: #808080;border-style: solid;padding-left: 40rpx;">
									<input type="text" value="" placeholder="Special instructions" style="width: 660rpx;height: 100rpx;" />
								</view> -->
							</view>
						</template>
						
						<view v-if="!isnoshowaddcart && showspecial" style="width: 650rpx;height: 310rpx;justify-content: center;align-items: center;display: flex;flex-direction: column;margin-top: 40rpx;">
							<view style="width: 600rpx;height: 80rpx;">
								<text style="color: #000000;font-size: 29rpx;">{{GetTextByLanguage('请注明任何过敏,食物说明等')}}</text>
							</view>
							<view style="width: 600rpx;height: 220rpx;border-width: 2rpx;border-color: #CDCDCD;border-radius: 10rpx;margin-top: 10rpx;">
								<textarea :placeholder="GetTextByLanguage('特殊要求提示文字1')" :placeholder-style="'font-size: 28rpx;'" v-model="special" maxlength="200" style="width: 600rpx;height: 220rpx;padding: 10rpx;font-size: 28rpx;" />
							</view>
						</view>
						<view style="width: 650rpx;height: 155rpx;" v-if="!isnoshowaddcart && cardfixed"></view>
						<view v-if="isnoshowaddcart" style="width: 650rpx;height: 55rpx;"></view>
						<view v-if="!isnoshowaddcart"
							:style="'width: 650rpx;height: 155rpx;flex-direction: row;justify-content: space-between;align-items: center;background-color: rgb(255, 255, 255,.9);z-index: 999;' + (cardfixed?'position:fixed;bottom:'+(offheight)+'px;':'')">
							<view  v-if="!isfreesale"
								style="flex-direction: row;justify-content: center;align-items: center;width: 300rpx;height: 155rpx;">
								<template>
								<button
									style="width: 65rpx;height: 65rpx;justify-content: center;align-items: center;display: flex;border-radius: 0rpx;"
									@tap="modcartcount('-')"><text style="font-size: 24rpx;">-</text></button>
								<view
									style="width: 75rpx;height: 65rpx;justify-content: center;align-items: center;text-align: center;border-color: #C0C0C0;border-style: solid;border-width: 2rpx;">
									<input type="number"
										style="width: 75rpx;height: 50rpx;text-align: center;font-size: 24rpx;"
										v-model="selectcount" @blur="modcartcount('blur')"></input>
								</view>
								<button
									style="width: 65rpx;height: 65rpx;justify-content: center;align-items: center;display: flex;border-radius: 0rpx"
									@tap="modcartcount('+')"><text style="font-size: 24rpx;">+</text></button>
									</template>
							</view>
							<view v-if="!isfreesale"
								style="justify-content: center;align-items: center;border-width:3rpx;border-style: solid;border-color: rgb(205, 205, 205);border-radius: 15rpx;height: 80rpx;margin-right: 30rpx;background-color: #000000;"
								@tap="AddCart">
								<text
									style="margin-left: 20rpx;margin-right: 20rpx;font-size: 24rpx;border:none !important;font-weight: bold;color: #FFFFFF;">{{GetTextByLanguage(!iseditopen?'加入购物车':'确认')}}</text>
							</view>
							<view v-if="isfreesale" style="display: flex;flex-direction: row;justify-content: space-between;width: 650rpx;">
								<view
									style="justify-content: center;align-items: center;border-width:3rpx;border-style: solid;border-color: rgb(205, 205, 205);border-radius: 15rpx;height: 80rpx;padding-left:50rpx;padding-right: 50rpx;margin-left: 30rpx;background-color: #fff;"
									@tap="close">
									<text
										style="color: #808080;font-size: 24rpx;border:none !important;font-weight: bold;">{{GetTextByLanguage('取消')}}</text>
								</view>
								<view
									style="justify-content: center;align-items: center;border-width:3rpx;border-style: solid;border-color: rgb(205, 205, 205);border-radius: 15rpx;height: 80rpx;padding-left:50rpx;padding-right: 50rpx;margin-right: 30rpx;background-color: #000000;"
									@tap="AddCart">
									<text
										style="font-size: 24rpx;border:none !important;font-weight: bold;color: #FFFFFF;">{{GetTextByLanguage('确认')}}</text>
								</view>
							</view>
						</view>
					</scroll-view>
					<image src="../../static/order/cha.png"
						style="position: absolute;right: -16rpx;top: -16rpx;width: 50rpx;height: 50rpx;" @tap="close">
					</image>
				</template>
			</view>
		</uni-transition>
		<!-- #endif -->
	</view>
</template>

<script>
	import locationorder from "@/util/locationorder.js"
	import language from "@/util/language.js"
	import uniTransition from '../uni-transition/uni-transition.vue'
	import popup from './popup.js'
	import request from "@/util/request.js"
	import api from "@/util/apiurl.js"
	/**
	 * PopUp 弹出层
	 * @description 弹出层组件，为了解决遮罩弹层的问题
	 * @tutorial https://ext.dcloud.net.cn/plugin?id=329
	 * @property {String} type = [top|center|bottom] 弹出方式
	 * 	@value top 顶部弹出
	 * 	@value center 中间弹出
	 * 	@value bottom 底部弹出
	 * 	@value message 消息提示
	 * 	@value dialog 对话框
	 * 	@value share 底部分享示例
	 * @property {Boolean} animation = [ture|false] 是否开启动画
	 * @property {Boolean} maskClick = [ture|false] 蒙版点击是否关闭弹窗
	 * @event {Function} change 打开关闭弹窗触发，e={show: false}
	 */

	export default {
		name: 'UniPopup',
		components: {
			uniTransition,
		},
		props: {
			// 开启动画
			animation: {
				type: Boolean,
				default: true
			},
			// 弹出层类型，可选值，top: 顶部弹出层；bottom：底部弹出层；center：全屏弹出层
			// message: 消息提示 ; dialog : 对话框
			type: {
				type: String,
				default: 'center'
			},
			// maskClick
			maskClick: {
				type: Boolean,
				default: false
			}
		},
		provide() {
			return {
				popup: this
			}
		},
		mixins: [popup],
		watch: {
			/**
			 * 监听type类型
			 */
			type: {
				handler: function(newVal) {
					this[this.config[newVal]]()
				},
				immediate: true
			},
			/**
			 * 监听遮罩是否可点击
			 * @param {Object} val
			 */
			maskClick(val) {
				this.mkclick = val
			}
		},
		data() {
			return {
				duration: 300,
				ani: [],
				showPopup: false,
				showTrans: false,
				maskClass: {
					'position': 'fixed',
					'bottom': 0,
					'top': 0,
					'left': 0,
					'right': 0,
					'backgroundColor': 'rgba(0, 0, 0, 0.4)'
				},
				transClass: {
					'position': 'fixed',
					'left': 0,
					'right': 0,
				},
				maskShow: true,
				mkclick: true,
				popupstyle: 'top',


				orderitem: {},
				pricelevel: '1',

				selectcount: 1, //选中的总数量
				selectsize: null, //选中的大小
				selectmodgroupcodelist: [], //选中的改餐列表
				screenHeight: 0,
				peoplemaxordercount: 0,
				perorder: 0,

				menuid: '', //菜单id
				menuclasscode: '', //菜单classcode
				saleid: '', //营业id
				cardfixed: false,
				
				
				special:'',
				
				showspecial:false,
				
				ispointsexchange:false,
				redeempoints:0,
				redeemid:null,
				redeemsizecode:null,
				isfreesale:false,
				freesaleid:null,
				cartamount:0,
				cardfixedpx:0,
				offheight:0,
				freeitem:null,
				
				
				defaultselectfirstmod:false,
				
				
				indexhotelid:'',
				taglist:[],
				
				iseditopen:false,
				cartindex:-1,
				
				isnoshowaddcart:false,
			}
		},
		created() {
			this.mkclick = this.maskClick
			if (this.animation) {
				this.duration = 300
			} else {
				this.duration = 0
			}
			var info = uni.getSystemInfoSync()
			if(info.screenHeight > info.windowHeight){
				this.screenHeight = info.windowHeight - 50
			}else{
				this.screenHeight = info.screenHeight - 50
			}
			this.offheight = uni.upx2px(47);
		},
		methods: {
			//展开套餐列表
			showcomboitem: function(item) {
				var that = this
				if (!item.showexitem) {
					item.showexitem = true;
				} else {
					item.showexitem = false;
				}
				that.$forceUpdate()
				that.checkcardfixedstatus()
				setTimeout(function(){
					that.checkcardfixedstatus()
				},250)
			},
			/*判断现在是否需要固定*/
			checkcardfixedstatus(){
				var that = this
				that.$nextTick(function(){
					var itemdetail = document.getElementById('itemdetail');
					
					
					// console.log('offsetHeight',itemdetail.offsetHeight)
					// console.log('screenHeight',that.screenHeight)
					if (itemdetail.offsetHeight >= that.screenHeight) {
						that.cardfixed = true
						that.offheight = uni.upx2px(47);
					} else {
						that.cardfixed = false
						that.offheight =  uni.upx2px(47);
					}
						itemdetail = document.getElementById('itemdetail');
						if (itemdetail.offsetHeight >= that.screenHeight) {
							that.cardfixed = true
							that.offheight = uni.upx2px(47);
						} else {
							that.cardfixed = false
							that.offheight =  uni.upx2px(47);
						}
				})
			},
			comcheckbox: function(e) {
				var that = this
				that.orderitem.itemcombolist.forEach((comitem, comindex) => {
					if (comitem.id == e.currentTarget.dataset.comboid) {
						comitem.exchangeitemlist.forEach((exitem, exindex) => {
							//如果是单选，其他都改为不选中
							if (comitem.maxqty == 1) {
								if (exitem.id == e.currentTarget.dataset.id) {
									exitem.checked = !exitem.checked
								} else {
									exitem.checked = false
								}
							} else {
								if (exitem.id == e.currentTarget.dataset.id) {
									exitem.checked = !exitem.checked
								}
							}
						})
					}
				})

				var reqcheckcount = 0

				var value = []
				that.orderitem.itemcombolist.forEach((comitem, comindex) => {
					if (comitem.id == e.currentTarget.dataset.comboid) {
						comitem.exchangeitemlist.forEach((exitem, exindex) => {
							if (exitem.checked) {
								value.push(exitem.id)
								reqcheckcount += exitem.repeatcount
							}
						})
						comitem.reqcheckcount = reqcheckcount
					}
				})



				var changejson = {
					currentTarget: {
						dataset: {
							comid: e.currentTarget.dataset.comboid,
							exchangeid: e.currentTarget.dataset.id
						}
					},
					detail: {
						value: value
					}
				}
				that.exitemcodesChange(changejson)


				that.$forceUpdate()
			},
			returnFloat: function(price) {
				return locationorder.returnFloat(price)
			},

			exchangesizeclick: function(e) {
				var that = this
				var comid = e.currentTarget.dataset.comid
				var exchangeid = e.currentTarget.dataset.exchangeid
				var sizeid = e.currentTarget.dataset.sizeid


				var changejson = {
					currentTarget: {
						dataset: {
							comid: comid,
							exchangeid: exchangeid,
						}
					},
					detail: {
						value: sizeid
					}
				}
				that.exchangeSizeChange(changejson)
			},

			/*
			 * 套餐子项大小变更
			 */
			exchangeSizeChange: function(e) {
				var that = this
				var comid = e.currentTarget.dataset.comid
				var exchangeid = e.currentTarget.dataset.exchangeid
				var sizeid = e.detail.value

				that.orderitem.itemcombolist.forEach((item, index) => {
					if (item.id == comid) {
						item.exchangeitemlist.forEach((exitem, exindex) => {
							if (exitem.id == exchangeid) {
								if (exitem.iteminfo && exitem.iteminfo.itemsizelist) {
									exitem.iteminfo.itemsizelist.forEach((sizeitem, sizeindex) => {
										sizeitem.checked = false
										if (sizeitem.id == sizeid) {
											sizeitem.checked = true
										}
									})
								}
							}
						})
					}
				})
				that.$forceUpdate()
			},
			/*
			 * Change 选中的套餐exchange列表
			 */
			exitemcodesChange: function(e) {
				var that = this
				var comid = e.currentTarget.dataset.comid
				var exchangeid = e.currentTarget.dataset.exchangeid
				var isexist = false

				var pdcount = true //判断数量是否符合group 条件



				var language = uni.getStorageSync('language')
				if (!language) {
					language = 'en'
				}

				that.orderitem.itemcombolist.forEach((item, index) => {
					if (item.id == comid) {
						item.error = false

						//如果选中数量不做限制，则进入判断
						var qty = 0
						item.exchangeitemlist.forEach((exitem, exindex) => {
							if (e.detail.value && e.detail.value.length > 0) {
								e.detail.value.forEach((detailitem, count) => {
									if (exitem.id == detailitem) {
										qty += exitem.repeatcount
									}
								})
							}
						})

						if (item.maxqty != 0) {

							//如果套餐限制的子项数量最多是 1，则点击其他切换
							if (item.maxqty == 1) {
								item.exchangeitemlist.forEach((exitem, exindex) => {
									if (exitem.id == exchangeid && exitem.checked) {
										exitem.checked = true
									} else {
										exitem.checked = false
									}
								})
								that.$forceUpdate()
								return
							}
							if (qty > item.maxqty) {
								if (language == 'en') {
									uni.showModal({
										title: 'Message',
										content: 'The number selected by combo[' + that.GetNameByDic(
												item
												.namedic,true) + '] must not be greater than ' +
											item.maxqty,
										confirmText: 'Confirm',
										showCancel: false
									})
								} else {
									uni.showModal({
										title: 'Message',
										content: '组合[' + that.GetNameByDic(item.namedic,true) + ']数量不能大于' +
											item.maxqty,
										confirmText: '确认',
										showCancel: false
									})
								}
								item.error = true
								that.$forceUpdate()
								that.ScrollErrorPosition()
							}
						}
						if (e.detail.value.length >= item.maxqty) {
							item.exchangeitemlist.forEach((moditem, modindex) => {
								if (e.detail.value.indexOf(moditem.id) <= -1) {
									//moditem.DisableCheck = true
								}
							})
						} else {
							item.exchangeitemlist.forEach((moditem, modindex) => {
								//moditem.DisableCheck = false
							})
						}

						//设置选中的状态- 方便在购物车判断
						item.exchangeitemlist.forEach((moditem, modindex) => {
							if (e.detail.value.indexOf(moditem.id) > -1) {
								moditem.checked = true
							} else {
								moditem.checked = false
							}
						})
					}
				})

				that.$forceUpdate()
				if (!pdcount) {
					return
				}

				// 	var selectcodelist = []
				// 	e.detail.value.forEach((x, index) => {
				// 		selectcodelist.push({
				// 			id: x,
				// 			qty: 0
				// 		})
				// 	})

				// that.selectmodgroupcodelist.forEach((item, index) => {
				// 	if (item.modgroupid == groupid) {
				// 		isexist = true
				// 		if (e.detail.value.length > 0) {
				// 			item.selectcodelist = selectcodelist
				// 		} else {
				// 			that.selectmodgroupcodelist.splice(index, 1)
				// 		}
				// 	}
				// })
				// if (!isexist && e.detail.value.length > 0) {
				// 	var modgroupjson = {
				// 		modgroupid: groupid,
				// 		selectcodelist: selectcodelist //选中的改餐item列表
				// 	}
				// 	that.selectmodgroupcodelist.push(modgroupjson)
				// }
			},
			/*
			 * 套餐中的exchangeitem 的数量增减
			 */
			exchangeitemcount: function(e) {
				var that = this
				var comid = e.currentTarget.dataset.comid
				var exchangeid = e.currentTarget.dataset.exchangeid
				var method = e.currentTarget.dataset.method

				if (method == '+') {
					var ischeck = true
					that.orderitem.itemcombolist.forEach((groupitem, index) => {
						if (groupitem.id == comid) {
							//如果选中数量不做限制，则进入判断
							var qty = 0
							groupitem.exchangeitemlist.forEach((exitem, exindex) => {
								if (exitem.repeatcount && exitem.checked) {
									qty += exitem.repeatcount
								}
							})
							qty++
							if (groupitem.maxqty != 0) {
								if (qty > groupitem.maxqty) {
									if (language == 'en') {
										uni.showModal({
											title: 'Message',
											content: 'The number selected by combo[' + that
												.GetNameByDic(groupitem
													.namedic,true) + '] must not be greater than ' +
												groupitem.maxqty,
											confirmText: 'Confirm',
											showCancel: false
										})
									} else {
										uni.showModal({
											title: 'Message',
											content: '组合[' + that.GetNameByDic(groupitem.namedic,true) +
												']数量不能大于' +
												groupitem.maxqty,
											confirmText: '确认',
											showCancel: false
										})
									}
									ischeck = false
									groupitem.error = true
									that.$forceUpdate()
									that.ScrollErrorPosition()
								}
							}
						}
					})
					if (!ischeck) {
						return
					}
				}


				that.orderitem.itemcombolist.forEach((item, index) => {
					if (item.id == comid) {
						item.error = false
						that.$forceUpdate()

						var reqcheckcount = 0
						item.exchangeitemlist.forEach((exitem, exindex) => {
							if (exitem.id == exchangeid) {
								if (method == '+') {
									exitem.repeatcount++
								} else if (method == '-') {
									if ((exitem.repeatcount - 1) <= 1) {
										exitem.repeatcount = 1
									} else {
										exitem.repeatcount--
									}
								} else {
									if (exitem.repeatcount < 1) {
										exitem.repeatcount = 1
									}
								}
							}
							if (exitem.checked) {
								reqcheckcount += exitem.repeatcount
							}
						})

						item.reqcheckcount = reqcheckcount
					}
				})
				that.$forceUpdate()
			},
			/*
			 * sizeChange
			 */
			sizeChange: function(e) {
				var that = this
				that.selectsize = e.detail.value
			},
			sizeclick: function(e) {
				var that = this
				//删除掉不是这个size的modgroup
				try{
					for(var i = 0;i < that.orderitem.itemmodgrouplist.length;i++){
						if(that.orderitem.itemmodgrouplist[i].fromsize && that.orderitem.itemmodgrouplist[i].fromsize != e.currentTarget.dataset.id){
							that.orderitem.itemmodgrouplist.splice(i,1)
							i--;
						}
					}
				}catch(exm){
					
				}
				
				that.orderitem.iteminfo.error = false
				that.orderitem.itemsizelist.forEach((item, index) => {
					item.checked = false
					if (item.id == e.currentTarget.dataset.id) {
						that.selectsize = item.id
						item.checked = true
						
						if(!that.orderitem.itemmodgrouplist){
							that.orderitem.itemmodgrouplist = []
						}
						if(item.itemmodgrouplist){
							var isexist = false
							that.orderitem.itemmodgrouplist.forEach((moditem,modindex)=>{
								if(moditem.fromsize && moditem.fromsize == item.id){
									isexist = true
								}
							})
							if(!isexist){
								//把这个size的modgroup 添加进去
								item.itemmodgrouplist.forEach((sizeitemmod,sizeitemmodindex) =>{
									sizeitemmod.fromsize = item.id
									
									that.orderitem.itemmodgrouplist.push(sizeitemmod)
								})
								console.log('sizeitem,item.itemmodgrouplist',item.itemmodgrouplist)
								
								//这下面用来判断 MG改餐 是否需要默认选中一项
								that.orderitem.itemmodgrouplist.forEach((moditem,modindex) =>{
									console.log('moditem.defaultmodifier',moditem.defaultmodifier)
								if(moditem.fromsize && moditem.fromsize == item.id){
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
												
												console.log('22223')
												that.modifiercodesclick(edata)
											})
										}else if(moditem.modifiercodeslist && moditem.modifiercodeslist.length > 1){
											if(moditem.defaultmodifier){
												//移到外层处理，不受minselection 的限制
												console.log('????111')
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
															console.log('进来了点击')
															that.modifiercodesclick(edata)
														}
													})
												}
											}
										}
									}
								}
							})
							
							//选中默认的modifier
							that.orderitem.itemmodgrouplist.forEach((moditem,modindex) =>{
								if(moditem.fromsize && moditem.fromsize == item.id){
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
								}
							})
							// that.orderitem.itemmodgrouplist.forEach((moditem,modindex) =>{
							// 	if(moditem.fromsize && moditem.fromsize == item.id){
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
							// 	}
							// })
								
							}
						}
					}
				})
				
				
				
				that.$forceUpdate()
			},

			//滚动到指定位置
			ScrollErrorPosition() {
				var that = this;
				setTimeout(function() {
					// #ifdef H5
					document.getElementsByClassName('listposition')[0].scrollIntoView();
					return
					// #endif
					uni.createSelectorQuery().select(".listerror").boundingClientRect(function(
						res) { //定位到你要的class的位置
						if (res) {
							that.$nextTick(() => {
								uni.pageScrollTo({
									scrollTop: res.top,
									duration: 0
								});
							});
						}
					}).exec()
				}, 200)
			},
			combomodifiercodesclick: function(e) {
				var that = this
				
				
				var reqcheckcount = 0
				var comboid = e.currentTarget.dataset.comboid
				var exchangeid = e.currentTarget.dataset.exchangeid
				var groupid = e.currentTarget.dataset.groupid
				var codeid = e.currentTarget.dataset.codeid
				that.orderitem.itemcombolist.forEach((comboitem, comboindex) => {
					if (comboitem.id == comboid) {
						comboitem.exchangeitemlist.forEach((exchangeitem, exchangeindex) => {
							if (exchangeitem.id == exchangeid) {
								exchangeitem.iteminfo.modlist.forEach((item, index) => {
									if (item.id == groupid) {
										item.modifiercodeslist.forEach((modifieritem,
											modifierindex) => {
											if (modifieritem.id == codeid) {
												modifieritem.checked = !modifieritem
													.checked
												if (!modifieritem.checked) {
													modifieritem.repeatcount = 1
												}
											}
										})
									}
								})
							}
						})
					}
				})

				var value = []
				that.orderitem.itemcombolist.forEach((comboitem, comboindex) => {
					if (comboitem.id == comboid) {
						comboitem.exchangeitemlist.forEach((exchangeitem, exchangeindex) => {
							if (exchangeitem.id == exchangeid) {
								exchangeitem.iteminfo.modlist.forEach((item, index) => {
									if (item.id == groupid) {
										item.modifiercodeslist.forEach((modifieritem,
											modifierindex) => {
											if (modifieritem.checked) {
												value.push(modifieritem.id)
											}
										})
									}
								})
							}
						})
					}
				})
				var changejson = {
					currentTarget: {
						dataset: {
							comboid: comboid,
							exchangeid: exchangeid,
							groupid: groupid,
							codeid: codeid
						}
					},
					detail: {
						value: value
					}
				}
				that.combomodifiercodesChange(changejson)

				that.$forceUpdate()
			},
			/*
			 * Change 套餐子项选中的改餐Group列表
			 */
			combomodifiercodesChange: function(e) {
				var that = this
				var comboid = e.currentTarget.dataset.comboid
				var exchangeid = e.currentTarget.dataset.exchangeid
				var groupid = e.currentTarget.dataset.groupid
				var codeid = e.currentTarget.dataset.codeid

				var pdcount = true //判断数量是否符合group 条件

				var language = uni.getStorageSync('language')
				if (!language) {
					language = 'en'
				}

				that.orderitem.itemcombolist.forEach((comboitem, comboindex) => {
					if (comboitem.id == comboid) {
						comboitem.exchangeitemlist.forEach((exchangeitem, exchangeindex) => {
							if (exchangeitem.id == exchangeid) {
								exchangeitem.iteminfo.modlist.forEach((item, index) => {
									if (item.id == groupid) {
										item.error = false
										//如果选中数量不做限制，则进入判断
										if (item.maxselection != 0) {
											//如果套餐限制的子项数量最多是 1，则点击其他切换
											if (item.maxselection == 1) {
												item.modifiercodeslist.forEach((codeitem,
													codeindex) => {
													if (codeitem.id == codeid &&
														codeitem.checked) {
														codeitem.checked = true
														e.detail.value = [codeitem.id]
													} else {
														codeitem.checked = false
													}
												})
												return
											}
											var qty = 0
											item.modifiercodeslist.forEach((codeitem,
												codeindex) => {
												if (codeitem.checked) {
													qty += codeitem.repeatcount
												}
											})
											if (qty > item.maxselection) {
												if (language == 'en') {
													uni.showModal({
														title: 'Message',
														content: 'The number of selected items cannot exceed ' +
															item.maxselection,
														confirmText: 'Confirm',
														showCancel: false
													})
												} else {
													uni.showModal({
														title: 'Message',
														content: '选中的餐项数量不能大于 ' + item
															.maxselection,
														confirmText: '确认',
														showCancel: false
													})
												}
												pdcount = false
												item.error = true
												that.$forceUpdate()
												that.ScrollErrorPosition()
												return false
											}
											if (qty >= item.maxselection) {
												item.modifiercodeslist.forEach((moditem,
													modindex) => {
													if (e.detail.value.indexOf(moditem
															.id) <= -1) {
														moditem.DisableCheck = true
													}
												})
											} else {
												item.modifiercodeslist.forEach((moditem,
													modindex) => {
													moditem.DisableCheck = false
												})
											}
										}
									}
								})

								//这里把选中的code 加入 modlist
								var selectcodelist = []
								e.detail.value.forEach((x, index) => {
									selectcodelist.push({
										id: x,
										qty: 0
									})
								})
								if (exchangeitem.iteminfo.selectmodgroupcodelist && exchangeitem
									.iteminfo.selectmodgroupcodelist.length > 0) {
									exchangeitem.iteminfo.selectmodgroupcodelist.forEach((item,
										index) => {
										if (item.modgroupid == groupid) {
											if (e.detail.value.length > 0) {
												item.selectcodelist = selectcodelist
											} else {
												exchangeitem.iteminfo.selectmodgroupcodelist
													.splice(index, 1)
											}
										}
									})
								}
								if (e.detail.value.length > 0) {
									var modgroupjson = {
										modgroupid: groupid,
										selectcodelist: selectcodelist //选中的改餐item列表
									}
									if (!exchangeitem.iteminfo.selectmodgroupcodelist) {
										exchangeitem.iteminfo.selectmodgroupcodelist = []
									}

									var isexist = false
									exchangeitem.iteminfo.selectmodgroupcodelist.forEach((moditem,
										modindex) => {
										if (moditem.modgroupid == groupid) {
											moditem.selectcodelist = modgroupjson
												.selectcodelist
											isexist = true
										}
									})
									if (!isexist) {
										exchangeitem.iteminfo.selectmodgroupcodelist.push(modgroupjson)
									}
								}
							}
						})
					}
				})
				
				//计算选中的mod S
				var reqcheckcount = 0
				that.orderitem.itemcombolist.forEach((comboitem, comboindex) => {
					if (comboitem.id == comboid) {
						comboitem.exchangeitemlist.forEach((exchangeitem, exchangeindex) => {
							if (exchangeitem.id == exchangeid) {
								exchangeitem.iteminfo.modlist.forEach((item, index) => {
									if (item.id == groupid) {
										item.modifiercodeslist.forEach((modifieritem,
											modifierindex) => {
											if(modifieritem.checked){
												reqcheckcount += modifieritem.repeatcount
											}
										})
										item.reqcheckcount = reqcheckcount
									}
								})
							}
						})
					}
				})
				
				//计算选中的mod E

				that.$forceUpdate()
			},
			/*
			 * 修改modifiercode的重复数量
			 */
			combomodgroupcodelistrepeatcount: function(e) {
				var that = this;
				var method = e.currentTarget.dataset.method
				var groupid = e.currentTarget.dataset.groupid
				var codeid = e.currentTarget.dataset.codeid
				var comboid = e.currentTarget.dataset.comboid
				var exchangeid = e.currentTarget.dataset.exchangeid



				var language = uni.getStorageSync('language')
				if (!language) {
					language = 'en'
				}

				if (method == '+') {
					var ischeck = true

					that.orderitem.itemcombolist.forEach((comboitem, comboindex) => {
						if (comboitem.id == comboid) {
							comboitem.exchangeitemlist.forEach((exchangeitem, exchangeindex) => {
								if (exchangeitem.id == exchangeid) {
									exchangeitem.iteminfo.modlist.forEach((item, modindex) => {
										if (item.id == groupid) {
											var qty = 0
											item.modifiercodeslist.forEach((exitem,
												exindex) => {
												if (exitem.repeatcount && exitem
													.checked) {
													qty += exitem.repeatcount
												}
											})
											qty++
											if (item.maxselection != 0) {
												if (qty > item.maxselection) {
													if (language == 'en') {
														uni.showModal({
															title: 'Message',
															content: 'The number of mods selected must not be greater than ' +
																item
																.maxselection,
															confirmText: 'Confirm',
															showCancel: false
														})
													} else {
														uni.showModal({
															title: 'Message',
															content: '改餐数量不能大于' + item
																.maxselection,
															confirmText: '确认',
															showCancel: false
														})
													}
													ischeck = false
													item.error = true
													that.$forceUpdate()
													that.ScrollErrorPosition()
												}
											}
										}
									})
								}
							})
						}
					})

					if (!ischeck) {
						return
					}
				}



				that.orderitem.itemcombolist.forEach((comboitem, comboindex) => {
					if (comboitem.id == comboid) {
						comboitem.exchangeitemlist.forEach((exchangeitem, exchangeindex) => {
							if (exchangeitem.id == exchangeid) {
								exchangeitem.iteminfo.modlist.forEach((item, modindex) => {
									if (item.id == groupid) {
										item.error = false
										item.modifiercodeslist.forEach((moditem, modindex) => {
											if (moditem.id == codeid) {
												if (method == '+') {
													moditem.repeatcount++
												} else if (method == '-') {
													if (moditem.repeatcount - 1 < 1) {
														moditem.repeatcount = 1
														that.combomodifiercodesclick(e)
													} else {
														moditem.repeatcount--
													}
												}
												return false
											}
										})
									}
								})
							}
						})
					}
				})
				
				//计算选中的mod S
				var reqcheckcount = 0
				that.orderitem.itemcombolist.forEach((comboitem, comboindex) => {
					if (comboitem.id == comboid) {
						comboitem.exchangeitemlist.forEach((exchangeitem, exchangeindex) => {
							if (exchangeitem.id == exchangeid) {
								exchangeitem.iteminfo.modlist.forEach((item, index) => {
									if (item.id == groupid) {
										item.modifiercodeslist.forEach((modifieritem,
											modifierindex) => {
											if(modifieritem.checked){
												reqcheckcount += modifieritem.repeatcount
											}
										})
										item.reqcheckcount = reqcheckcount
									}
								})
							}
						})
					}
				})
				
				//计算选中的mod E
				
				that.$forceUpdate()
			},
			modifiercodesclick: function(e) {
				var that = this
				var groupid = e.currentTarget.dataset.groupid
				var codeid = e.currentTarget.dataset.codeid
				that.orderitem.itemmodgrouplist.forEach((item, index) => {
					if (item.id == groupid) {
						if (item.modifiercodeslist) {
							item.modifiercodeslist.forEach((moditem, modindex) => {
								if (moditem.id == codeid) {
									moditem.checked = !moditem.checked
									if (!moditem.checked) {
										moditem.repeatcount = 1
									}
								} else {
									if ((item.minselection == 0 || item.minselection == 1) && item.maxselection == 1) {
										moditem.checked = false
									}
								}
							})
						}
					}
				})
				
				var reqcheckcount = 0

				var value = []
				that.orderitem.itemmodgrouplist.forEach((item, index) => {
					if (item.id == groupid) {
						if (item.modifiercodeslist) {
							item.modifiercodeslist.forEach((moditem, modindex) => {
								if (moditem.checked) {
									value.push(moditem.id)
									reqcheckcount += moditem.repeatcount
								}
							})
							item.reqcheckcount = reqcheckcount
						}
					}
				})

				var changejson = {
					currentTarget: {
						dataset: {
							groupid: groupid,
							codeid: codeid
						}
					},
					detail: {
						value: value
					}
				}
				that.modifiercodesChange(changejson)
				that.$forceUpdate()
			},
			/*
			 * Change 选中的改餐Group列表
			 */
			modifiercodesChange: function(e) {
				var that = this
				var groupid = e.currentTarget.dataset.groupid
				var codeid = e.currentTarget.dataset.codeid
				var isexist = false

				var pdcount = true //判断数量是否符合group 条件


				var ismaxselection = false

				var language = uni.getStorageSync('language')
				if (!language) {
					language = 'en'
				}

				that.orderitem.itemmodgrouplist.forEach((item, index) => {
					if (item.id == groupid) {
						item.error = false

						var qty = 0
						item.modifiercodeslist.forEach((exitem, exindex) => {
							if (e.detail.value) {
								e.detail.value.forEach((detailitem, detailvalue) => {
									if (exitem.id == detailitem) {
										qty += exitem.repeatcount
									}
								})

							}
						})
						if (item.maxselection != 0) {
							//如果套餐限制的子项数量最多是 1，则点击其他切换
							if (item.maxselection == 1) {
								ismaxselection = true
								item.modifiercodeslist.forEach((codeitem, codeindex) => {
									if (codeitem.id == codeid && codeitem.checked) {
										codeitem.checked = true
									} else {
										codeitem.checked = false
									}
								})
								that.$forceUpdate()
								return
							}
							if (qty > item.maxselection) {
								if (language == 'en') {
									uni.showModal({
										title: 'Message',
										content: 'The number of mods selected must not be greater than ' +
											item
											.maxselection,
										confirmText: 'Confirm',
										showCancel: false
									})
								} else {
									uni.showModal({
										title: 'Message',
										content: '改餐数量不能大于' + item
											.maxselection,
										confirmText: '确认',
										showCancel: false
									})
								}
								item.error = true
								that.$forceUpdate()
								that.ScrollErrorPosition()
							}
						}





						//如果选中数量不做限制，则进入判断
						if (item.maxselection != 0) {
							if (e.detail.value.length > item.maxselection) {
								if (language == 'en') {
									uni.showToast({
										title: 'The number of selected items cannot exceed ' + item
											.maxselection,
										icon: 'none'
									})
								} else {
									uni.showToast({
										title: '选中的餐项数量不能大于 ' + item
											.maxselection,
										icon: 'none'
									})
								}
								pdcount = false

								item.error = true
								that.$forceUpdate()
								that.ScrollErrorPosition()
								return false
							}
							if (e.detail.value.length >= item.maxselection) {
								item.modifiercodeslist.forEach((moditem, modindex) => {
									if (e.detail.value.indexOf(moditem.id) <= -1) {
										moditem.DisableCheck = true
									}
								})
							} else {
								item.modifiercodeslist.forEach((moditem, modindex) => {
									moditem.DisableCheck = false
								})
							}
						}
					}
				})
				that.$forceUpdate()
				if (!pdcount) {
					return
				}

				var selectcodelist = []
				e.detail.value.forEach((x, index) => {
					//如果只允许选择一个， 要加上判断codeid
					if (ismaxselection) {
						if (x == codeid) {
							selectcodelist.push({
								id: x,
								qty: 0
							})
						}
					} else {
						selectcodelist.push({
							id: x,
							qty: 0
						})
					}
				})

				that.selectmodgroupcodelist.forEach((item, index) => {
					if (item.modgroupid == groupid) {
						isexist = true
						if (e.detail.value.length > 0) {
							item.selectcodelist = selectcodelist
						} else {
							that.selectmodgroupcodelist.splice(index, 1)
						}
					}
				})
				if (!isexist && e.detail.value.length > 0) {
					var modgroupjson = {
						modgroupid: groupid,
						selectcodelist: selectcodelist //选中的改餐item列表
					}
					that.selectmodgroupcodelist.push(modgroupjson)
				}
			},
			/*
			 * 修改modifiercode的重复数量
			 */
			modgroupcodelistrepeatcount: function(e) {
				var that = this;
				var method = e.currentTarget.dataset.method
				var groupid = e.currentTarget.dataset.groupid
				var codeid = e.currentTarget.dataset.codeid

				var language = uni.getStorageSync('language')
				if (!language) {
					language = 'en'
				}

				if (method == '+') {
					var ischeck = true

					that.orderitem.itemmodgrouplist.forEach((item, index) => {
						if (item.id == groupid) {
							var qty = 0
							item.modifiercodeslist.forEach((exitem, exindex) => {
								if (exitem.repeatcount && exitem.checked) {
									qty += exitem.repeatcount
								}
							})
							qty++
							if (item.maxselection != 0) {
								if (qty > item.maxselection) {
									if (language == 'en') {
										uni.showModal({
											title: 'Message',
											content: 'The number of mods selected must not be greater than ' +
												item
												.maxselection,
											confirmText: 'Confirm',
											showCancel: false
										})
									} else {
										uni.showModal({
											title: 'Message',
											content: '改餐数量不能大于' + item
												.maxselection,
											confirmText: '确认',
											showCancel: false
										})
									}
									ischeck = false
									item.error = true
									that.$forceUpdate()
									that.ScrollErrorPosition()
								}
							}
						}
					})
					if (!ischeck) {
						return
					}
				}



				that.orderitem.itemmodgrouplist.forEach((item, index) => {
					if (item.id == groupid) {
						item.error = false

						var reqcheckcount = 0
						item.modifiercodeslist.forEach((moditem, modindex) => {
							if (moditem.id == codeid) {
								if (method == '+') {
									moditem.repeatcount++
								} else if (method == '-') {
									if (moditem.repeatcount - 1 < 1) {
										moditem.repeatcount = 1
										that.modifiercodesclick(e)
									} else {
										moditem.repeatcount--
									}
								}
							}
							if (moditem.checked) {
								reqcheckcount += moditem.repeatcount
							}
						})
						item.reqcheckcount = reqcheckcount
					}
				})
				that.$forceUpdate()
			},
			/*
			 * 修改item数量
			 */
			modcartcount: function(method) {
				var that = this
				if (method == '+') {
					that.selectcount++
				} else if (method == '-') {
					if ((that.selectcount - 1) <= 1) {
						that.selectcount = 1
					} else {
						that.selectcount--
					}
				} else {
					if (that.selectcount < 1) {
						that.selectcount = 1
					}
				}
				that.$forceUpdate()
			},
			/*
			 * 加入购物车
			 */
			AddCart() {
				var that = this
				if (that.selectcount <= 0 || !that.orderitem || !that.orderitem.iteminfo) {
					return
				}
				
				if(that.isnotaddcart){
					uni.showModal({
						showCancel: false,
						title: 'Message',
						content: 'QRCode is invalid, please scan valid QR-Code',
						confirmText: 'Confirm',
						success() {
						}
					})
					return
				}
				
				var isexist = false

				var language = uni.getStorageSync('language')
				if (!language) {
					language = 'en'
				}


				//这里给选中的改餐的qty赋值
				that.selectmodgroupcodelist.forEach((codeitem, codeindex) => {
					if (that.orderitem.itemmodgrouplist) {
						that.orderitem.itemmodgrouplist.forEach((moditem, modindex) => {
							if (codeitem.modgroupid == moditem.id) {
								codeitem.selectcodelist.forEach((sonitem, sonindex) => {
									moditem.modifiercodeslist.forEach((x, index) => {
										if (x.id == sonitem.id) {
											sonitem.qty = x.repeatcount
										}
									})
								})
							}
						})
					}
				})

				//给选中的套餐子项的改餐赋值 S
				if (that.orderitem.itemcombolist && that.orderitem.itemcombolist.length > 0) {
					that.orderitem.itemcombolist.forEach((comitem, comindex) => {
						comitem.exchangeitemlist.forEach((exchangeitem, exchangeindex) => {
							if (exchangeitem.iteminfo && exchangeitem.iteminfo.modlist && exchangeitem
								.iteminfo.modlist.length > 0 && exchangeitem.iteminfo
								.selectmodgroupcodelist && exchangeitem.iteminfo.selectmodgroupcodelist
								.length > 0) {
								exchangeitem.iteminfo.modlist.forEach((moditem, modindex) => {
									exchangeitem.iteminfo.selectmodgroupcodelist.forEach((
										selectitem, selectindex) => {
										if (moditem.id == selectitem.modgroupid) {
											selectitem.selectcodelist.forEach((codeitem,
												codeindex) => {
												moditem.modifiercodeslist.forEach((
													modifieritem,
													modifierindex) => {
													if (modifieritem.id ==
														codeitem.id) {

														codeitem.qty =
															modifieritem
															.repeatcount
													}
												})
											})
										}
									})
								})
							}
						})
					})
				}
				//给选中的套餐子项的改餐赋值 E

				//计划加入cart的json
				var cartitemjson = {
					menuid: that.menuid,
					menuclasscode: that.menuclasscode,
					itemcode: that.orderitem.iteminfo.itemcode,
					qty: parseInt(that.selectcount),
					size: that.selectsize,
					modgroupcodelist: ((that.selectmodgroupcodelist == null || that.selectmodgroupcodelist.length <=
						0) ? null : that.selectmodgroupcodelist),
					combolist: null,
					special:that.special.trim(),
					ispointsexchange:that.ispointsexchange,
					redeemid:that.redeemid
				}
				
				if(that.isfreesale){
					cartitemjson.issalefree = true
					cartitemjson.freesaleid = that.freesaleid
					try{
						var cartinfo = uni.getStorageSync(that.indexhotelid + 'cartinfo')
						if(cartinfo){
							var cartjson = JSON.parse(cartinfo)
							cartjson.forEach((cartitem,cartindex)=>{
								if(cartitem.issalefree && cartitem.issalefree && cartitem.freesaleid == that.freesaleid){
									cartjson.splice(cartindex,1)
								}
							})
							uni.setStorageSync(that.indexhotelid + 'cartinfo',JSON.stringify(cartjson))
						}
					}catch(e){
						
					}
					
				}
				
				if(that.redeemid){
					cartitemjson.isnotkioskshowredeem = true
					cartitemjson.redeemsumpoints = cartitemjson.qty * that.redeempoints
					
					try{
						var userinfo = uni.getStorageSync(that.indexhotelid + 'userinfo')
						if(userinfo){
							var points = userinfo.points == null?0:userinfo.points
							var cartinfo = uni.getStorageSync(that.indexhotelid + 'cartinfo')
							if(cartinfo){
								var cartjson = JSON.parse(cartinfo)
								cartjson.forEach((cartitem,cartindex)=>{
									if(cartitem.ispointsexchange){
										points -= cartitem.redeemsumpoints
									}
								})
							}
							if((points - cartitemjson.redeemsumpoints) < 0){
								uni.showModal({
									title:that.GetTextByLanguage('消息'),
									content:"Sorry, you don't have enough points to redeem the selected item.",
									showCancel:false,
									confirmText:that.GetTextByLanguage('确定'),
								})
								return
							}
						}
					}catch(e){
						
					}
				}

				if (that.saleid) {
					cartitemjson.saleid = that.saleid
				}

				if (that.peoplemaxordercount) {
					var peoplecount = uni.getStorageSync(that.indexhotelid + 'peoplecount')

					//如果状态每轮只允许下x单，当他只有一个人
					if (that.perorder == 1) {
						peoplecount = 1
					}
					if (peoplecount) {
						var cartnumber = 0
						var cartinfo = uni.getStorageSync(that.indexhotelid + 'cartinfo')
						if (cartinfo) {
							cartinfo = JSON.parse(cartinfo)
							var qty = 0
							cartinfo.forEach((item, index) => {
								qty += parseInt(item.qty)
							})
							cartnumber = qty
						} else {
							cartnumber = 0
						}
						if ((cartnumber + cartitemjson.qty) > (that.peoplemaxordercount * peoplecount)) {
							if (language == 'en') {
								uni.showModal({
									title: 'Message',
									content: 'The number of items in the shopping cart cannot be greater than ' + (
										that.peoplemaxordercount * peoplecount),
									confirmText: 'Confirm',
									showCancel: false
								})
							} else if (language == 'cn') {
								uni.showModal({
									title: '消息',
									content: '购物车餐项数量不能大于' + (that.peoplemaxordercount * peoplecount),
									confirmText: '确认',
									showCancel: false
								})
							} else if (language == 'tc') {
								uni.showModal({
									title: '消息',
									content: '購物車餐項數量不能大於' + (that.peoplemaxordercount * peoplecount),
									confirmText: '確認',
									showCancel: false
								})
							}
							return
						}
					}
				}
				//组合套餐json
				if (that.orderitem.itemcombolist != null && that.orderitem.itemcombolist.length > 0) {
					var comcodelist = []
					that.orderitem.itemcombolist.forEach((comitem, comindex) => {
						var comjson = {
							comid: comitem.id,
							exchangelist: []
						}

						//这里循环套餐子项
						comitem.exchangeitemlist.forEach((exitem, exindex) => {
							if (exitem.checked) {
								var exjson = {
									id: exitem.id,
									qty: exitem.repeatcount,
									size: null,
									modlist: null
								}
								if (exitem.iteminfo.itemtype == 'Z' && exitem.iteminfo.itemsizelist) {
									exitem.iteminfo.itemsizelist.forEach((sizeitem, sizeindex) => {
										if (sizeitem.checked) {
											exjson.size = sizeitem.id
										}
									})
								}
								if (exitem.iteminfo.selectmodgroupcodelist && exitem.iteminfo
									.selectmodgroupcodelist.length > 0) {
									exjson.modlist = exitem.iteminfo.selectmodgroupcodelist
								}
								comjson.exchangelist.push(exjson)
							}
						})
						if (comjson.exchangelist && comjson.exchangelist.length > 0) {
							comcodelist.push(comjson)
						}
					})
					if (comcodelist && comcodelist.length > 0) {
						cartitemjson.combolist = comcodelist
					}
				}

				//判断大小 S
				if (that.orderitem.iteminfo.itemtype == 'Z' && that.orderitem.iteminfo.forcesize) {
					var ischeck = true
					if (!cartitemjson.size) {
						if (language == 'en') {
							uni.showModal({
								title: 'Message',
								content: 'Please select size',
								confirmText: 'Confirm',
								showCancel: false
							})
						} else {
							uni.showModal({
								title: 'Message',
								content: '请选择大小',
								confirmText: '确认',
								showCancel: false
							})
						}
						that.orderitem.iteminfo.error = true
						that.$forceUpdate()
						that.ScrollErrorPosition()

						ischeck = false
					}
					if (!ischeck) {
						return
					}
				}
				//判断大小 E
				
				//判断改餐 S
				if (that.orderitem.itemmodgrouplist && that.orderitem.itemmodgrouplist.length > 0) {
					var ischeck = true
					that.orderitem.itemmodgrouplist.forEach((groupitem, groupindex) => {
						var qty = 0
						if (cartitemjson.modgroupcodelist) {
							cartitemjson.modgroupcodelist.forEach((codeitem, codeindex) => {
								if (groupitem.id == codeitem.modgroupid) {
									codeitem.selectcodelist.forEach((x, index) => {
										qty += x.qty
									})
								}
							})
						}
						if (qty < groupitem.minselection) {
							if(ischeck){
								if (language == 'en') {
									uni.showModal({
										title: 'Message',
										content: 'The number of mods selected must not be less than ' +
											groupitem
											.minselection,
										confirmText: 'Confirm',
										showCancel: false
									})
								} else {
									uni.showModal({
										title: 'Message',
										content: '改餐数量不能小于' + groupitem.minselection,
										confirmText: '确认',
										showCancel: false
									})
								}
								groupitem.showexitem = true
							}
							ischeck = false
				
							groupitem.error = true
							that.$forceUpdate()
							that.ScrollErrorPosition()
						}
				
						//如果选中数量不做限制，则进入判断
						if (groupitem.maxselection != 0) {
							if (qty > groupitem.maxselection) {
								if(ischeck){
									if (language == 'en') {
										uni.showModal({
											title: 'Message',
											content: 'The number of mods selected must not be greater than ' +
												groupitem
												.maxselection,
											confirmText: 'Confirm',
											showCancel: false
										})
									} else {
										uni.showModal({
											title: 'Message',
											content: '改餐数量不能大于' + groupitem.maxselection,
											confirmText: '确认',
											showCancel: false
										})
									}
									groupitem.showexitem = true
								}
								
								ischeck = false
				
								groupitem.error = true
								that.$forceUpdate()
								that.ScrollErrorPosition()
							}
						}
					})
					if (!ischeck) {
						return
					}
				}
				//判断改餐 E

				//判断套餐选择 S
				if (that.orderitem.itemcombolist && that.orderitem.itemcombolist.length > 0) {
					var ischeck = true
					that.orderitem.itemcombolist.forEach((comitem, comindex) => {
						var qty = 0
						if (cartitemjson.combolist) {
							cartitemjson.combolist.forEach((comboitem, comboindex) => {
								if (comboitem.comid == comitem.id) {
									comboitem.exchangelist.forEach((x, index) => {
										//qty += x.wqn
										qty += x.qty
									})
								}
							})
						}
						if (qty < comitem.minqty) {
							if (ischeck) {
								if (language == 'en') {
									uni.showModal({
										title: 'Message',
										content: 'The number selected by combo[' + that.GetNameByDic(
												comitem
												.namedic,true) + '] must not be less than ' +
											comitem
											.minqty,
										confirmText: 'Confirm',
										showCancel: false
									})
								} else {
									uni.showModal({
										title: 'Message',
										content: '组合[' + that.GetNameByDic(comitem.namedic,true) + ']数量不能小于' +
											comitem.minqty,
										confirmText: '确认',
										showCancel: false
									})
								}
								comitem.showexitem = true
							}
							ischeck = false

							comitem.error = true

							that.$forceUpdate()
							that.ScrollErrorPosition()
						}
						if (qty > comitem.maxqty && comitem.maxqty != 0) {
							if(ischeck){
								if (language == 'en') {
									uni.showModal({
										title: 'Message',
										content: 'The number selected by combo[' + that.GetNameByDic(comitem
												.namedic,true) + '] must not be greater than ' +
											comitem.maxqty,
										confirmText: 'Confirm',
										showCancel: false
									})
								} else {
									uni.showModal({
										title: 'Message',
										content: '组合[' + that.GetNameByDic(comitem.namedic,true) + ']数量不能大于' +
											comitem.maxqty,
										confirmText: '确认',
										showCancel: false
									})
								}
								comitem.showexitem = true
							}
							ischeck = false


							comitem.error = true
							that.$forceUpdate()
							that.ScrollErrorPosition()
						}



						//判断套餐内改餐

						comitem.exchangeitemlist.forEach((exitem, exindex) => {
							if (exitem.iteminfo && exitem.iteminfo.modlist && exitem.iteminfo.modlist
								.length > 0) {
								exitem.iteminfo.modlist.forEach((groupitem, groupindex) => {
									let exmodqty = 0
									let isexist = false
									if (cartitemjson.combolist) {
										cartitemjson.combolist.forEach((combobitem,
											comboindex) => {
											if (combobitem.comid == comitem.id) {
												combobitem.exchangelist.forEach((
													exchangeitem, exchangeindex
												) => {
													if (exchangeitem.id == exitem
														.id) {
														isexist = true
														if (exchangeitem.modlist &&
															exchangeitem.modlist
															.length > 0) {
															// console.log(
															// 	'exchangeitem',
															// 	exchangeitem)
															exchangeitem.modlist
																.forEach((x,exmodindex) => {
																	if(x.modgroupid == groupitem.id){
																		if (x.selectcodelist && x.selectcodelist.length > 0) {
																			x.selectcodelist.forEach((codeqitem,codeqindex) => {
																						//console.log(codeqitem)
																						exmodqty += codeqitem.qty
																					}
																				)
																		}
																	}
																})
														}
													}
												})
											}
										})
									}
									//console.log('exmodqty123123213', groupitem)

									//console.log('计次',isexist,groupitem,groupitem.namedic,exmodqty,groupitem.minselection,groupitem.maxselection)
									if (isexist) {
										if (exmodqty < groupitem.minselection) {
											if (language == 'en') {
												uni.showModal({
													title: 'Message',
													content: 'The number of mods selected must not be less than ' +
														groupitem
														.minselection,
													confirmText: 'Confirm',
													showCancel: false
												})
											} else {
												//console.log('弹窗了',JSON.stringify(groupitem))
												uni.showModal({
													title: 'Message',
													content: '改餐数量不能小于' + groupitem
														.minselection,
													confirmText: '确认',
													showCancel: false
												})
											}
											ischeck = false
											groupitem.showexitem = true
											groupitem.error = true
											that.$forceUpdate()
											that.ScrollErrorPosition()

											//如果选中数量不做限制，则进入判断
											if (groupitem.maxselection != 0) {
												if (exmodqty > groupitem.maxselection) {
													if (language == 'en') {
														uni.showModal({
															title: 'Message',
															content: 'The number of mods selected must not be greater than ' +
																groupitem
																.maxselection,
															confirmText: 'Confirm',
															showCancel: false
														})
													} else {
														uni.showModal({
															title: 'Message',
															content: '改餐数量不能大于' + groupitem
																.maxselection,
															confirmText: '确认',
															showCancel: false
														})
													}
													ischeck = false
													groupitem.showexitem = true
													groupitem.error = true
													that.$forceUpdate()
													that.ScrollErrorPosition()
												}
											}
										}
									}
								})
								if (!ischeck) {
									return false
								}
							}
						})
						//判断t套餐内改餐 E



						if (!ischeck) {
							return false
						}
					})

					if (!ischeck) {
						return
					}
				}

				//判断套餐选择 E



				var cartinfo = uni.getStorageSync(that.indexhotelid + 'cartinfo')
				var cartarray = []
				if (cartinfo) {
					try {
						cartarray = JSON.parse(cartinfo)
					} catch (e) {
						cartarray = []
					}
					if(that.cartindex <= -1){
						
					if(!that.special || that.special == ''){
						
					try {
						cartarray.forEach((item, index) => {

							// uni.showToast({
							// 	title: '??',
							// 	icon: 'none'
							// })
							if (item.itemcode == that.orderitem.iteminfo.itemcode && (!item.special || item.special == '')) {
								//判断 大小、改餐、套餐 一不一致
								// if (item.modgroupcodelist == null && item.size == that.selectsize && item.combolist == null && cartitemjson
								// 	.selectmodgroupcodelist == null && cartitemjson.combolist == null) {
								// 	isexist = true
								// 	item.qty += parseInt(that.selectcount)

								// }else{
								// 	if(isidentical)


								//一个个比对，所有项匹配 才增加对应的购物车item数量

								var isidentical = true
								if (item.menuid != cartitemjson.menuid) {
									isidentical = false
								}
								if (item.menuclasscode != cartitemjson.menuclasscode) {
									isidentical = false
								}
								if (item.size != cartitemjson.size) {
									isidentical = false
								}
								if (item.modgroupcodelist == null && cartitemjson.modgroupcodelist != null) {
									isidentical = false
								}
								if (item.modgroupcodelist != null && cartitemjson.modgroupcodelist == null) {
									isidentical = false
								}
								if (item.modgroupcodelist != null && cartitemjson.modgroupcodelist != null) {
									var a = 0
									item.modgroupcodelist.forEach((moditem, modindex) => {
										cartitemjson.modgroupcodelist.forEach((item, index) => {
											if (moditem.modgroupid == item.modgroupid) {
												var c = 0
												moditem.selectcodelist.forEach((selectcode,
													selectindex) => {
													item.selectcodelist.forEach((
														itemselectcode,
														itemselectindex
													) => {
														if (selectcode.id ==
															itemselectcode.id &&
															selectcode.qty ==
															itemselectcode.qty) {
															c++;
														}
													})
												})
												if (c == moditem.selectcodelist.length && c ==
													item.selectcodelist.length) {
													a++
												}
											}
										})
									})


									if (a == item.modgroupcodelist.length) {
										// item.modgroupcodelist.forEach((codeitem, codeindex) => {
										// 	cartitemjson.modgroupcodelist.forEach((itemgroupcode,
										// 		itemgroupcodeindex) => {
										// 		if (codeitem.modgroupid == itemgroupcode.modgroupid) {
										// 			codeitem.selectcodelist.forEach((selectcode,
										// 				selectindex) => {
										// 				itemgroupcode.selectcodelist.forEach((
										// 					itemselectcode,
										// 					itemselectindex) => {
										// 					if (selectcode.id ==
										// 						itemselectcode.id) {
										// 						selectcode.qty +=
										// 							itemselectcode.qty
										// 						console.log('相等',
										// 							itemselectcode
										// 							.qty)
										// 					}
										// 				})
										// 			})
										// 		}
										// 	})
										// })
									} else {
										isidentical = false
									}
								}
								if (item.combolist == null && cartitemjson.combolist != null) {
									isidentical = false
								}
								if (item.combolist != null && cartitemjson.combolist == null) {
									isidentical = false
								}
								if (item.combolist != null && cartitemjson.combolist != null) {
									var a = 0
									item.combolist.forEach((item, index) => {
										cartitemjson.combolist.forEach((comitem, comindex) => {
											if (comitem.comid == item.comid) {
												var c = 0
												item.exchangelist.forEach((selectcode,
													selectindex) => {
													comitem.exchangelist.forEach((
														itemselectcode,
														itemselectindex
													) => {
														if (selectcode.id ==
															itemselectcode.id &&
															selectcode.size ==
															itemselectcode.size &&
															selectcode.qty ==
															itemselectcode.qty) {

															if ((itemselectcode
																	.modlist ==
																	null ||
																	itemselectcode
																	.modlist
																	.length <= 0
																) && (
																	selectcode
																	.modlist ==
																	null ||
																	selectcode
																	.modlist
																	.length <= 0
																)) {
																c++;
															} else {
																var d = 0
																if (itemselectcode
																	.modlist !=
																	null &&
																	itemselectcode
																	.modlist
																	.length > 0 &&
																	selectcode
																	.modlist !=
																	null &&
																	selectcode
																	.modlist
																	.length > 0) {
																	itemselectcode
																		.modlist
																		.forEach((
																			moditem,
																			modindex
																		) => {
																			selectcode
																				.modlist
																				.forEach(
																					(codemoditem,
																						codemoindex
																					) => {
																						if (moditem
																							.modgroupid ==
																							codemoditem
																							.modgroupid
																						) {
																							moditem
																								.selectcodelist
																								.forEach(
																									(selectmoditem,
																										selectmodindex
																									) => {
																										codemoditem
																											.selectcodelist
																											.forEach(
																												(codeselectmoditem,
																													codeselectmodindex
																												) => {
																													if (codeselectmoditem
																														.id ==
																														selectmoditem
																														.id &&
																														codeselectmoditem
																														.qty ==
																														selectmoditem
																														.qty
																													) {
																														d++
																													}
																												}
																											)
																									}
																								)
																						}
																					}
																				)
																		})
																	var itemcount =
																		0
																	var selectitemcount =
																		0

																	itemselectcode
																		.modlist
																		.forEach((
																			moditem,
																			modindex
																		) => {
																			moditem
																				.selectcodelist
																				.forEach(
																					(selectmoditem,
																						selectmodindex
																					) => {
																						itemcount++
																					}
																				)
																		})
																	selectcode
																		.modlist
																		.forEach((
																			moditem,
																			modindex
																		) => {
																			moditem
																				.selectcodelist
																				.forEach(
																					(selectmoditem,
																						selectmodindex
																					) => {
																						selectitemcount++
																					}
																				)
																		})
																	if (d ==
																		itemcount &&
																		d ==
																		selectitemcount
																	) {
																		c++
																	}

																}

															}
														}
													})
												})
												if (c == item.exchangelist.length && c == comitem
													.exchangelist.length) {
													a++
												}
											}
										})
									})

									//赋值
									if (a == item.combolist.length) {
										// item.combolist.forEach((item, index) => {
										// 	cartitemjson.combolist.forEach((comitem, comindex) => {
										// 		if (comitem.comid == item.comid) {
										// 			var c = 0
										// 			item.exchangelist.forEach((selectcode,
										// 				selectindex) => {
										// 				comitem.exchangelist.forEach((
										// 					itemselectcode,
										// 					itemselectindex
										// 				) => {
										// 					if (selectcode.id ==
										// 						itemselectcode.id &&
										// 						selectcode.size ==
										// 						itemselectcode.size) {
										// 						selectcode.qty +=
										// 							itemselectcode.qty
										// 						console.log('相等',
										// 							itemselectcode
										// 							.qty)
										// 					}
										// 				})
										// 			})
										// 		}
										// 	})
										// })
									} else {
										isidentical = false
									}
								}
								
								if(item.ispointsexchange != cartitemjson.ispointsexchange){
									isidentical = false
								}
								if(item.issalefree != cartitemjson.issalefree){
									isidentical = false
								}
								if(item.freesaleid != cartitemjson.freesaleid){
									isidentical = false
								}
								if(item.redeemid != cartitemjson.redeemid){
									isidentical = false
								}
								
								//如果全部符合 累加qty和积分
								if (isidentical) {
									item.qty += cartitemjson.qty
									
									if(item.redeemsumpoints != null){
										item.redeemsumpoints += cartitemjson.redeemsumpoints
									}else{
										item.redeemsumpoints = cartitemjson.redeemsumpoints
									}
									
									isexist = true
								}
								//}
								// else if (item.modgroupcodelist != null && cartitemjson.selectmodgroupcodelist !=
								// 	null && item.size == that.selectsize && item.combolist == null) {
								// 	//先判断是否全部相等，如果相等 累加
								// 	var a = 0
								// 	item.modgroupcodelist.forEach((moditem, modindex) => {
								// 		that.selectmodgroupcodelist.forEach((item, index) => {
								// 			if (moditem.modgroupid == item.modgroupid) {
								// 				var c = 0
								// 				moditem.selectcodelist.forEach((selectcode,
								// 					selectindex) => {
								// 					item.selectcodelist.forEach((
								// 						itemselectcode, itemselectindex
								// 					) => {
								// 						if (selectcode.id ==
								// 							itemselectcode.id) {
								// 							c++;
								// 						}
								// 					})
								// 				})
								// 				if (c == moditem.selectcodelist.length) {
								// 					a++
								// 				}
								// 			}
								// 		})
								// 	})
								// 	if (a == item.modgroupcodelist.length) {
								// 		isexist = true
								// 		item.modgroupcodelist.forEach((codeitem, codeindex) => {
								// 			that.selectmodgroupcodelist.forEach((itemgroupcode,
								// 				itemgroupcodeindex) => {
								// 				if (codeitem.modgroupid == itemgroupcode.modgroupid) {
								// 					codeitem.selectcodelist.forEach((selectcode,
								// 						selectindex) => {
								// 						itemgroupcode.selectcodelist.forEach((
								// 							itemselectcode,
								// 							itemselectindex) => {
								// 							if (selectcode.id ==
								// 								itemselectcode.id) {
								// 								selectcode.qty +=
								// 									itemselectcode.qty
								// 								console.log('相等',
								// 									itemselectcode
								// 									.qty)
								// 							}
								// 						})
								// 					})
								// 				}
								// 			})
								// 		})
								// 		console.log(item)
								// 	}
								// }
							}
						})
					} catch (ex) {
						uni.removeStorageSync(that.indexhotelid + 'cartinfo')
						uni.showToast({
							title: 'Cart info Error, please rejoin the cart',
							icon: 'none',
							duration: 2000
						})
						that.close()
						return
					}
					
					}
					}else{
						
						console.log('进来了',cartitemjson)
						cartarray[that.cartindex] = cartitemjson
						console.log()
						isexist = true
					}
				}
				if (!isexist) {
					cartarray.push(cartitemjson)
				}
				
				try{
					uni.setStorageSync(that.indexhotelid + 'cartinfolastupdatetime', that.getNowFormatDate())
				}catch(ewww){
					
				}
				uni.setStorageSync(that.indexhotelid + 'cartinfo', JSON.stringify(cartarray))
				
				that.updateuuid()
				
				if(that.cartindex <= -1){
					uni.showToast({
						title: that.GetTextByLanguage('加入购物车成功'),
						icon: 'none',
						duration: 2000
					})
				}else{
					that.$emit('cartgetnewdata')
				}


				that.$emit("RefreshCart")
				that.$emit('CloseChange')
				that.close()

			},
			getNowFormatDate() {
			  var date = new Date();
			  var seperator1 = "/";
			  var year = date.getFullYear();
			  var month = date.getMonth() + 1;
			  var strDate = date.getDate();
			  var hour = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
			  var minute = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
			  var second =  date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
			  if (month >= 1 && month <= 9) {
			    month = "0" + month;
			  }
			  if (strDate >= 0 && strDate <= 9) {
			    strDate = "0" + strDate;
			  }
			  var currentdate = year + seperator1 + month + seperator1 + strDate +' ' +hour +':' + minute + ':' + second;
			  return currentdate;
			},
			clear(e) {
				// TODO nvue 取消冒泡
				e.stopPropagation()
			},
			GetTextByLanguage(text) {

				return language.GetTextByLanguage(text)
			},
			GetPriceByDic: function(pricedic) {
				var that = this
				
				return locationorder.GetPriceBydic(that.pricelevel, pricedic)
			},
			GetNameByDic: function(namedic,isitem) {
				return language.GetNameByDic(namedic,isitem)
			},
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
			open(itemid, classcode, peoplemaxordercount, perorder, menuid, saleid,itemmodifiergroupshrink,itemmodifiergroupshrinknum,defaultiteminfo,itemcode,itemdetail,showspecial,pricelevel,ispointsexchange,redeemid,redeemsizecode,redeempoints,isfreesale,freesaleid,cartamount,freeitem,defaultselectfirstmod,indexhotelid,taglist,taglistcodes,isnotaddcart,isnoshowaddcart) {
				
				this.cartindex = -1
				
				this.iseditopen = false
				
				this.isnotaddcart = isnotaddcart
				this.isnoshowaddcart = false
				 
				if(isnoshowaddcart){
					this.isnoshowaddcart = isnoshowaddcart
				}
				
				this.freeitem = null
				this.taglist = taglist
				this.taglistcodes = taglistcodes
				if(freeitem){
					this.freeitem = freeitem
				}
				
				this.indexhotelid = indexhotelid
				
				var info = uni.getSystemInfoSync()
				if(info.screenHeight > info.windowHeight){
					this.screenHeight = info.windowHeight - 50
				}else{
					this.screenHeight = info.screenHeight - 50
				}
				this.offheight = uni.upx2px(47);
				//重置
				this.selectcount = 1
				this.orderitem = {}
				this.pricelevel = '1'
				
				if(pricelevel){
					this.pricelevel = pricelevel
				}
				
				this.selectcount = 1 //选中的总数量
				this.selectsize = null //选中的大小
				this.selectmodgroupcodelist = [] //选中的改餐列表
				this.peoplemaxordercount = peoplemaxordercount

				this.perorder = perorder
				this.menuid = menuid
				this.menuclasscode = classcode

				this.showPopup = true
				this.cardfixed = false
				this.saleid = saleid
				
				this.special = ''
				this.showspecial = showspecial
				
				this.ispointsexchange = false
				this.redeempoints = 0
				this.redeemid = null
				this.redeemsizecode = null
				this.redeempoints = null
				
				
				this.cartamount = 0
				if(cartamount){
					this.cartamount = cartamount
				}
				
				console.log('isfreesale',isfreesale)
				if(isfreesale){
					this.isfreesale = isfreesale
					
					console.log('isfreesale',isfreesale)
				}else{
					this.isfreesale = false
				}
				
				if(freesaleid){
					this.freesaleid = freesaleid
				}else{
					this.freesaleid = null
				}
				
				this.defaultselectfirstmod = defaultselectfirstmod
				
				if(redeemid){
					this.redeemid = redeemid
					this.redeemsizecode = redeemsizecode
					this.redeempoints = redeempoints
					
					console.log('1redeempoints'+redeempoints)
				}
				this.getiteminfo(itemid, classcode,itemmodifiergroupshrink,itemmodifiergroupshrinknum,defaultiteminfo,itemcode,itemdetail,ispointsexchange)
				
				this.checkcardfixedstatus()
				
				
				this.showTrans = true
				// this.$nextTick(() => {
				// 	new Promise(resolve => {
				// 		clearTimeout(this.timer)
				// 		this.timer = setTimeout(() => {
				// 			this.showTrans = true
				// 			// fixed by mehaotian 兼容 app 端
				// 			this.$nextTick(() => {
				// 				resolve();
				// 			})
				// 		}, 50);
				// 	}).then(res => {
				// 		// 自定义打开事件
				// 		clearTimeout(this.msgtimer)
				// 		this.msgtimer = setTimeout(() => {
				// 			this.customOpen && this.customOpen()
				// 		}, 100)
				// 		this.$emit('change', {
				// 			show: true,
				// 			type: this.type
				// 		})
				// 	})
				// })
			},
			editopen(cartitem,itemid, classcode, peoplemaxordercount, perorder, menuid, saleid,itemmodifiergroupshrink,itemmodifiergroupshrinknum,defaultiteminfo,itemcode,itemdetail,showspecial,pricelevel,ispointsexchange,redeemid,redeemsizecode
			,redeempoints,isfreesale,freesaleid,cartamount,freeitem,defaultselectfirstmod,indexhotelid,taglist,taglistcodes){
				var that = this
				
				that.iseditopen = true
				this.isnotaddcart = false
				
				this.cartindex = cartitem.cartindex
				
				this.freeitem = null
				this.taglist = taglist
				this.taglistcodes = taglistcodes
				if(freeitem){
					this.freeitem = freeitem
				}
				
				this.indexhotelid = indexhotelid
				
				var info = uni.getSystemInfoSync()
				if(info.screenHeight > info.windowHeight){
					this.screenHeight = info.windowHeight - 50
				}else{
					this.screenHeight = info.screenHeight - 50
				}
				// this.defaultaddoffheight = this.defaultaddoffheight - 150
				//重置
				this.selectcount = 1
				this.orderitem = {}
				this.pricelevel = '1'
				
				if(pricelevel){
					this.pricelevel = pricelevel
				}
				
				this.selectcount = 1 //选中的总数量
				this.selectsize = null //选中的大小
				this.selectmodgroupcodelist = [] //选中的改餐列表
				this.peoplemaxordercount = peoplemaxordercount
				
				this.perorder = perorder
				this.menuid = menuid
				this.menuclasscode = classcode
				
				this.showPopup = true
				this.cardfixed = false
				this.saleid = saleid
				
				this.special = ''
				this.showspecial = showspecial
				
				this.ispointsexchange = false
				this.redeempoints = 0
				this.redeemid = null
				this.redeemsizecode = null
				this.redeempoints = null
				
				
				this.cartamount = 0
				if(cartamount){
					this.cartamount = cartamount
				}
				
				console.log('isfreesale',isfreesale)
				if(isfreesale){
					this.isfreesale = isfreesale
					
					console.log('isfreesale',isfreesale)
				}else{
					this.isfreesale = false
				}
				
				if(freesaleid){
					this.freesaleid = freesaleid
				}else{
					this.freesaleid = null
				}
				
				this.defaultselectfirstmod = defaultselectfirstmod
				
				if(redeemid){
					this.redeemid = redeemid
					this.redeemsizecode = redeemsizecode
					this.redeempoints = redeempoints
					
					console.log('1redeempoints'+redeempoints)
				}
				this.checkcardfixedstatus()
				
				
				this.showTrans = true
				
				
				
				//mango slush
				//cartitem = {"menuid":3,"menuclasscode":"21","itemcode":"2101","qty":1,"size":"e230125e-c0e9-4906-9245-014d31dab78a","modgroupcodelist":[{"modgroupid":"0aafd926-627d-4ce8-961d-58aa30101e30","selectcodelist":[{"id":"38d72216-edce-4a69-8ca1-1375a0dfaae9","qty":2},{"id":"d97022f7-aab0-499c-ab5d-0f88645f45c0","qty":2}]}],"combolist":null,"special":"","ispointsexchange":false,"redeemid":null}
				
				uni.showLoading({
					title:language.GetTextByLanguage('加载中')
				})
				var data = {
					hid:uni.getStorageSync('orderhotelid'),
					classcode: cartitem.menuclasscode,
					itemid: null,
					menuid: cartitem.menuid,
					saleid: null,
					itemcode:cartitem.itemcode,
					cartamount:that.cartamount
				}
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
								that.offheight = uni.upx2px(that.defaultaddoffheight);
							} else {
								that.cardfixed = false
								
								that.offheight =  uni.upx2px(that.defaultaddoffheight);
							}
				
							setTimeout(function() {
								itemdetail = document.getElementById('itemdetail');
								if (itemdetail.offsetHeight >= that.screenHeight) {
									that.cardfixed = true
									that.offheight = uni.upx2px(that.defaultaddoffheight);
								} else {
									that.cardfixed = false
									that.offheight =  uni.upx2px(that.defaultaddoffheight);
								}
							}, 500)
						}, 100)
						
						
						//4076
						//这里赋初始值完了，然后给open参数赋值 S
						
						//这里赋值数量
						that.selectcount = cartitem.qty
						
						//这里赋值大小
						if(cartitem.size){
							if(that.orderitem.itemsizelist){
								that.orderitem.itemsizelist.forEach((item,index) =>{
									if(item.id == cartitem.size){
										item.checked = true
										
										var edata = {
											currentTarget:{
												dataset:{
													id:item.id,
												}
											}
										}
										
										//这里模拟大小点击
										that.sizeclick(edata)
									}else{
										item.checked = false
									}
								})
							}
						}
						
						//这里赋值套餐
						if(cartitem.combolist){
							cartitem.combolist.forEach((superitem,superindex)=>{
								if(that.orderitem.itemcombolist){
									that.orderitem.itemcombolist.forEach((item,index) =>{
										if(superitem.comid == item.id){
											item.showexitem = false
											
											that.showcomboitem(item)
											console.log(item)
											
											
											if(superitem.exchangelist){
												if(item.exchangeitemlist){
													item.exchangeitemlist.forEach((itemexchange,itemexchangeindex)=>{
														itemexchange.checked = false
													})
													
													superitem.exchangelist.forEach((superexchangeitem,superexchangeindex)=>{
														item.exchangeitemlist.forEach((itemexchange,itemexchangeindex)=>{
															if(superexchangeitem.id == itemexchange.id){
																itemexchange.repeatcount = superexchangeitem.qty
																itemexchange.checked = true
																
																
																//这里赋值套餐里面的大小
																if(superexchangeitem.size){
																	if(itemexchange.iteminfo && itemexchange.iteminfo.itemsizelist){
																		itemexchange.iteminfo.itemsizelist.forEach((itemsize,itemindex)=>{
																			if(itemsize.id == superexchangeitem.size){
																				var sizedata = {
																					currentTarget:{
																						dataset:{
																							comid:item.id,
																							exchangeid:itemexchange.id,
																							sizeid:itemsize.id
																						}
																					}
																				}
																				that.exchangesizeclick(sizedata)
																			}else{
																				itemsize.checked = false
																			}
																		})
																	}
																}
																
																//这里赋值套餐里的改餐
																if(superexchangeitem.modlist){
																	if(itemexchange.iteminfo && itemexchange.iteminfo.modlist){
																		superexchangeitem.modlist.forEach((supermoditem,supermodindex)=>{
																			itemexchange.iteminfo.modlist.forEach((itemmoditem,itemmodindex)=>{
																				if(supermoditem.modgroupid == itemmoditem.id){
																					if(supermoditem.selectcodelist){
																						if(itemmoditem.modifiercodeslist){
																							
																							supermoditem.selectcodelist.forEach((supercodeitem,supercodeindex)=>{
																								itemmoditem.modifiercodeslist.forEach((codeitem,codeindex)=>{
																									if(supercodeitem.id == codeitem.id){
																										var codeedata = {
																											currentTarget:{
																												dataset:{
																													groupid:itemmoditem.id,
																													comboid:superitem.comid,
																													exchangeid:superexchangeitem.id,
																													codeid:supercodeitem.id,
																												}
																											}
																										}
																										codeitem.repeatcount = supercodeitem.qty
																										that.combomodifiercodesclick(codeedata)
																									}
																								})
																							})
																						}
																					}
																				}
																			})
																		})
																	}
																}
															}
														})
													})
												}
											}
										}
									})
								}
							})
							
						}
						
						//这里赋值改餐
						if(cartitem.modgroupcodelist){
								console.log('进来了这里1')
							if(that.orderitem.itemmodgrouplist){
								cartitem.modgroupcodelist.forEach((superitem,superindex)=>{
									that.orderitem.itemmodgrouplist.forEach((item,index)=>{
										if(item.id == superitem.modgroupid){
											console.log('进来了这里1')
											that.showcomboitem(item)
											
											//这里先把默认选中给清空
											item.modifiercodeslist.forEach((modcodeitem,modcodeindex) =>{
												modcodeitem.checked = false
											})
											
											if(superitem.selectcodelist){
												superitem.selectcodelist.forEach((supercodeitem,supercodeindex)=>{
													if(item.modifiercodeslist){
														item.modifiercodeslist.forEach((modcodeitem,modcodeindex)=>{
															if(supercodeitem.id == modcodeitem.id){
																
																console.log('进来了这里2')
																
																//这里模拟改餐点击
																var edata = {
																	currentTarget:{
																		dataset:{
																			groupid:item.id,
																			codeid:supercodeitem.id
																		}
																	}
																}
																that.modifiercodesclick(edata)
																modcodeitem.repeatcount = supercodeitem.qty
																
															}
														})
													}
												})
											}
										}
									})
								})
							}
						}
						
						//这里赋值特殊要求
						if(cartitem.special){
							that.special = cartitem.special
						}
						
						
						
					}
				})
			},
			close(type) {
				this.showTrans = false
				this.$nextTick(() => {
					this.$emit('change', {
						show: false,
						type: this.type
					})
					clearTimeout(this.timer)
					// 自定义关闭事件
					this.customOpen && this.customClose()
					this.timer = setTimeout(() => {
						this.showPopup = false
					}, 300)
				})
			},
			onTap() {
				if (!this.mkclick) return
				this.close()


			},
			/**
			 * 顶部弹出样式处理
			 */
			top() {
				this.popupstyle = 'top'
				this.ani = ['slide-top']
				this.transClass = {
					'position': 'fixed',
					'left': 0,
					'right': 0,
				}
			},
			/**
			 * 底部弹出样式处理
			 */
			bottom() {
				this.popupstyle = 'bottom'
				this.ani = ['slide-bottom']
				this.transClass = {
					'position': 'fixed',
					'left': 0,
					'right': 0,
					'bottom': 0
				}
			},
			/**
			 * 中间弹出样式处理
			 */
			center() {
				this.popupstyle = 'center'
				this.ani = ['zoom-out', 'fade']
				this.transClass = {
					'position': 'fixed',
					/* #ifndef APP-NVUE */
					'display': 'flex',
					'flexDirection': 'column',
					/* #endif */
					'bottom': 0,
					'left': 0,
					'right': 0,
					'top': 0,
					'justifyContent': 'center',
					'alignItems': 'center'
				}
			},
			updateuuid(){
				var orderrandomstr = uni.getStorageSync(uni.getStorageSync('orderhotelid') + 'orderrandomstr')
				
				if(!orderrandomstr){
					var s = [];
					var hexDigits = "0123456789abcdef";
					for (var i = 0; i < 36; i++) {
						s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
					}
					s[14] = "4";  // bits 12-15 of the time_hi_and_version field to 0010
					s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01
					s[8] = s[13] = s[18] = s[23] = "-";
									 
					var uuid = s.join("");
					orderrandomstr = uuid
						
					uni.setStorageSync(uni.getStorageSync('orderhotelid') + 'orderrandomstr', orderrandomstr)
				}
			},
		}
	}
</script>
<style>
	/* #ifdef H5 */
	.uni-scroll-view-content{
		transform:initial !important
	}
	/* #endif */
</style>
<style lang="scss" scoped>
	.uni-popup {
		position: fixed;
		/* #ifndef APP-NVUE */
		z-index: 99;
		/* #endif */
	}

	.uni-popup__mask {
		position: absolute;
		top: 0;
		bottom: 0;
		left: 0;
		right: 0;
		background-color: $uni-bg-color-mask;
		opacity: 0;
	}

	.mask-ani {
		transition-property: opacity;
		transition-duration: 0.2s;
	}

	.uni-top-mask {
		opacity: 1;
	}

	.uni-bottom-mask {
		opacity: 1;
	}

	.uni-center-mask {
		opacity: 1;
	}

	.uni-popup__wrapper {
		/* #ifndef APP-NVUE */
		display: block;
		/* #endif */
		position: absolute;
	}

	.top {
		/* #ifdef H5 */
		top: var(--window-top);
		/* #endif */
		/* #ifndef H5 */
		top: 0;
		/* #endif */
	}

	.bottom {
		bottom: 0;
	}

	.uni-popup__wrapper-box {
		/* #ifndef APP-NVUE */
		display: block;
		/* #endif */
		position: relative;
		/* iphonex 等安全区设置，底部安全区适配 */
		/* #ifndef APP-NVUE */
		padding-bottom: constant(safe-area-inset-bottom);
		padding-bottom: env(safe-area-inset-bottom);
		/* #endif */
	}

	.content-ani {
		// transition: transform 0.3s;
		transition-property: transform, opacity;
		transition-duration: 0.2s;
	}


	.uni-top-content {
		transform: translateY(0);
	}

	.uni-bottom-content {
		transform: translateY(0);
	}

	.uni-center-content {
		transform: scale(1);
		opacity: 1;
	}

	.listerror {
		background-color: #ffeded;
		border: 1px solid #ef4e4b;
		border-radius: 10rpx;
	}

	/* #ifdef H5 */
	uni-button:after {
		border-radius: 0rpx !important
	}

	/* #endif */

	.plus-icon-enter-active {
		transition: opacity .4s;
	}

	.plus-icon-enter {
		opacity: 0;
	}

	.plus-icon-leave-active {
		transition: opacity .2s;
	}

	.plus-icon-leave-to {
		opacity: 0;
	}
	
	
	
	
	.promo-preview {
		width: 650rpx;
		height: 405rpx;
		color: #fff;
		background-position: center center;
		background-size: cover;
		float: right;
		background-color: #999;
		font-family: Arial, Helvetica, sans-serif;
		background-size: cover;
	}
	
	.promo-preview-details {
		position: relative;
		height: 100%;
		width: 100%;
		background-image: linear-gradient(to top, transparent 45%, rgba(0, 0, 0, .4) 100%);
		background-color: rgba(0, 0, 0, .15);
		box-shadow: inset 0 0 0 1px rgba(0, 0, 0, .1);
	}
	
	.promo-preview-details-title {
		padding-left: 40rpx;
		padding-right: 40rpx;
		padding-top: 40rpx;
		width: 95%;
	}
	
	.promo-preview-details-title-span {
		font-size: 38rpx;
		font-weight: 700;
	}
	
	.promo-preview-details-description {
		padding-left: 40rpx;
		padding-right: 40rpx;
		padding-top: 30rpx;
	}
	
	.promo-preview-details-description-span {
		font-size: 28rpx;
		word-break: break-word;
	}
	
	
	.preview-conditions {
		position: absolute;
		bottom: 0;
		left: 0;
		padding: 10rpx;
		background-color: rgba(46, 46, 51, .5);
		border-radius: 0 10rpx 0 0;
		font-size: 20rpx;
	}
	
	.preview-conditions-btn {
		position: absolute;
		bottom: 20rpx;
		right: 20rpx;
		padding: 10rpx;
		background-color: rgba(255, 255, 255, .9);
		width: 200rpx;
		height: 80rpx;
		justify-content: center;
		align-items: center;
		border-radius: 5rpx;
		cursor: pointer;
	}
</style>
