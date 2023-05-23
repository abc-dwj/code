<template>
  <div class="directive"
    style="padding-left: 0px;padding-right: 0px;display: flex;flex-direction: row;justify-content: center;">
    <div :style="'min-width:'+leftshowrighwidth+'px;background-color: #FFFFFF;'" v-if="sectionshowcalendar && bookingsetting">
      <div style="display: flex;flex: 1;">
        <div style="">
          <div style="width:400px;height:30px;display:flex;justify-content: flex-start;align-items: center;padding-left: 20px;">
            <el-radio-group v-model="profileBookingChangeRadio">
              <el-radio label="date">
                <span style="font-size: 15px;">By Date</span>
              </el-radio>
              <el-radio label="all">
                <span style="font-size: 15px;">By All</span>
              </el-radio>
            </el-radio-group>
          </div>
          <div style="height: 50px;display: flex;justify-content: flex-start;align-items: center;">
            <div
              style="display: flex;flex-direction: row;justify-content: flex-start;align-items: center;margin-left: 20px;min-width: 400px;">
              <div
                style="display: flex;flex-direction: row;justify-content: center;align-items: center;position: relative;">
                <i class="el-icon-date" style="font-size: 1.3rem;margin-right: 5px;cursor: pointer;"
                  @click="showcalendar = !showcalendar"></i>
                <calendar v-show="showcalendar" v-model="value"
                  style="width:380px;border: 1px solid #dfe6ec;z-index: 9999;position: absolute;left: 0;top: 50px;">
                  <template slot="dateCell" slot-scope="{date, data}">
                    <div style="width: 100%;height: 100%;display: flex;justify-content: center;align-items: center;"
                      :class="data.isSelected ? 'is-selected' : ''" @click="calendarOnClick(data)">
                      <div style="display: flex;flex-direction: column;">
                        <span>{{ data.day.split('-')[2]}}</span>
                        <span style="zoom: 0.9;" v-if="data.isSelected">✔️</span>
                      </div>
                    </div>
                  </template>
                </calendar>
                <div
                  style="width: 50px;height: 50px;display: flex;justify-content: center;align-items: center;cursor: pointer;"
                  @click="changedate('-')"><i class="el-icon-caret-left" style="font-size: 22px;"></i></div>
                <div @click="showcalendar = !showcalendar"
                  style="cursor: pointer;width: 120px;height: 50px;font-size: 20px;display: flex;justify-content: center;align-items: center;">
                  {{valuestr}}
                </div>
                <div
                  style="width: 50px;height: 50px;display: flex;justify-content: center;align-items: center;cursor: pointer;"
                  @click="changedate('+')"><i class="el-icon-caret-right" style="font-size: 22px;"></i></div>
              </div>
              <el-button type="warning" style="margin-left: 30px;" @click="gotoday">Today</el-button>
              <el-button type="success" style="margin-left: 30px;" @click="createnewbooking">New Booking</el-button>

              <div v-if="bookingsetting.bookingmode == 'profile' && false" style="display: flex;flex-direction: row;justify-content: flex-start;align-items: center;margin-left: 50px;width: 98%;">
                <el-input
                  v-model="phonenumber"
                  type="number"
                  placeholder="phonenumber"
                  style="width: 200px;"
                  :min="0"
                  @keyup.enter.native="searchphonenumber"
                />
                <el-button style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="searchphonenumber">{{ GetTextByLanguage('搜索') }}</el-button>
              </div>
              <div
                style="margin-left: 50px;display: flex;flex-direction: row;justify-content: flex-start;align-items: center;">
                <div>
                  <span>{{totalparty}}</span>&nbsp;<span>{{totalparty <= 1?'Party':'Partys'}}</span>
                </div>
                <div style="margin-left: 30px;">
                  <span>{{totalpersion}}</span>&nbsp;<span>{{totalpersion <= 1?'Person':'Persons'}}</span>
                </div>
              </div>
            </div>
            <!-- <div style="display: flex;flex-direction: row;justify-content: flex-start;align-items: center;margin-left: 100px;">
            <el-input
              v-model="page.keyword"
              :placeholder="GetTextByLanguage('请输入关键词')"
              style="width: 300px;margin-left: 10px;"
              @keyup.enter.native="getList"
            />
            <el-button style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="getList">{{ GetTextByLanguage('搜索') }}</el-button>
          </div> -->
            <div v-if="timelist && timelist.length > 0 && bookingsetting.bookingmode == 'table'"
              style="width:30px;height: 50px;position: absolute;display: flex;justify-content: center;align-items: center;right: 20px;">
              <i title="Booking List" class="el-icon-message" style="color: #52b37d;font-size: 30px;cursor: pointer;"
                @click="showright = !showright;showselfbooking = false"></i>
            </div>
            <div v-if="bookingsetting.bookingmode == 'table' && selfbookingcalendar && selfbookingcalendar.length > 0"
              style="width:30px;height: 50px;position: absolute;display: flex;justify-content: center;align-items: center;right: 90px;">
              <i title="New Self-Booking" class="el-icon-bell contentanimation" style="color: #1c4cfb;font-size: 27px;cursor: pointer;"
               @click="showselfbooking = !showselfbooking;showright = false"></i>
            </div>
          </div>
          <div :style="'display: flex;flex-direction: row;width:'+(leftshowrighwidth+'px')">
            <!-- 行程表格S -->
            <div id="timelistscroll" v-if="timelist && timelist.length > 0"
              :style="'width:'+leftshowrighwidth + 'px;overflow-x: scroll;max-height: '+leftheight+'px'+';margin-top: 20px;cursor: default;position: relative;display: flex;flex-direction: row;'">
              <div v-if="sectionshowcalendar && sectionshowcalendar.length > 1"
                :style="'width: auto;height: 50px;position: absolute;left: 0px;display: flex;flex-direction: row;padding-left:30px'">
                <div @click="changeselectsectionid(sectionitem.sectionid)"
                  v-for="(sectionitem,sectionindex) in sectionshowcalendar"
                  :style="'height: 100%;display: flex;justify-content: center;align-items: center;position:relative;'+(sectionindex != 0?'margin-left:30px':'')">
                  <span
                    :style="selectsectionid == sectionitem.sectionid?'font-weight: bold;font-size:18px;':''">{{sectionitem.sectionname}}</span>
                  <div v-if="selectsectionid == sectionitem.sectionid"
                    style="width:100%;height: 1px;border-bottom: 1px solid black;position: absolute;bottom: 0px;"></div>
                </div>
              </div>
              <table class="needsclick" v-if="timeslotfromdatalist && timeslotfromdatalist.length > 0" id="timetable"
                :style="'overflow-x:scroll;width: 100%;max-height: '+leftheight+'px'+';border-collapse: collapse;table-layout: fixed;'+(sectionshowcalendar.length> 1?'margin-top: 50px;border-top: 1px solid #cdcdcd;':'')">
                <tr style="height: 30px;position: relative;z-index: 4;" id="timelistaptr" v-if="timelistap && timelistap.length > 0">
                  <th :style="'z-index:4;' + (bookingsetting.bookingmode == 'profile'?'width:155px':'')">
                    <div>
                    </div>
                  </th>
                  <th style="z-index: 4;height: 30px;" v-for="(item,index) in timelistap"
                    v-if="bookingsetting.bookingmode == 'profile' || item.sectionid == selectsectionid" colspan="4">
                    <div style="position: relative;height: 100%;"><span class="timehour"
                        style="font-size: 14px;font-style: italic;color: #1c4cfb;">{{item.peoplecount}}&nbsp;{{item.peoplecount > 1?'pers':'per'}}&nbsp;|&nbsp;{{item.partycount}}&nbsp;P</span>
                      <div class="borderrightposition" v-if="index != timelist.length - 1"
                        style="border-right: 1px solid #ffe666;height: 600px;position: absolute;top: 0px;left: 3px;z-index: -1;pointer-events: none;">
                      </div>
                    </div>
                  </th>
                </tr>
                <tr style="height: 30px;position: relative;z-index: 4;"  id="timelistmin">
                  <th :style="'z-index:4;'+ (bookingsetting.bookingmode == 'profile'?'width:155px':'')">
                    <div :style="bookingsetting.bookingmode == 'profile'?'flex:1;flex-direction: column;height: 100%;':''">
                    <div v-if="bookingsetting.bookingmode == 'profile'" :style="'display: flex;flex:1; flex-direction: column;justify-content: flex-start;align-items: center;width: 99%;' + (timelistap && timelistap.length > 0?'position:relative;bottom:20px':'')">
                      <el-input
                        v-model="phonenumber"
                        type="number"
                        placeholder="phone number"
                        style="width: 100%;"
                        :min="0"
                        @keyup.enter.native="searchphonenumber"
                      />
                      <el-button style="width: 99%;" icon="el-icon-search" type="primary"  @click="searchphonenumber">{{ GetTextByLanguage('搜索') }}</el-button>
                    </div>
                    </div>
                  </th>
                  <th style="z-index: 4;height: 100%;" v-for="(item,index) in timelist" colspan="4">
                    <div style="position: relative;"><span class="timehour">{{item}}</span>
                      <div class="borderrightposition" v-if="(!timelistap || timelistap.length <= 0) && index != timelist.length - 1"
                        style="border-right: 1px solid #ffe666;height: 600px;position: absolute;top: 0px;left: 3px;z-index: -1;pointer-events: none;">
                      </div>
                    </div>
                    <div style="display: flex;flex-direction: row;justify-content: space-between;align-items: center;width: 100%;">
                      <div style="width: 25%;text-align: center;font-size: 14.5px;position: relative;left: 10.5%;"><span>15</span></div>
                      <div style="width: 25%;text-align: center;font-size: 14.5px"><span>30</span></div>
                      <div style="width: 25%;text-align: center;font-size: 14.5px;position: relative;right: 10.5%"><span>45</span></div>
                    </div>
                  </th>
                </tr>
                <template v-for="(item,index) in timeslotfromdatalist">
                  <tr style="width: 100%;background-color: rgb(255 254 195);position: relative;z-index: 0;"
                    v-if="item.tabletypeid">
                    <td style="width: 100%;height: 30px;" :colspan="timelist.length *4 + 1">
                      <div
                        style="width: 100px;margin-left: 20px;position: sticky;left: 0px;background-color: rgb(255 254 195);font-weight: bold;">
                        {{item.typename}}
                      </div>
                    </td>
                  </tr>
                  <tr class="CodeMirror11" :style="'width: 100%;'+(item.selectthis?'border:2px solid black':'')"
                    v-if="!item.tabletypeid">
                    <td @click="selecttablename(item)"
                      :style="'width: 100%;height: 30px;border: 1px solid #cccccc;position: sticky;left: 0px;background-color: '+(item.Noshowbooking?'#f3f3f3':'rgb(255,254,195)')+';z-index: 4;'">
                      <span style="width: 100%;height: 100%;" v-if="bookingsetting.bookingmode == 'table'">{{item.Tablename}}</span>
                      <div style="width: 100%;height: 100%;" v-if="bookingsetting.bookingmode == 'profile' && item.isselfbooking">
                        <i v-if="item.isselfbooking" class="el-icon-user" style="margin-right: 2px;"></i>
                        <span>{{item.mobilephone? formatmobile(item.mobilephone,true):item.email?item.email:item.firstname}}
                        </span>
                      </div>
                      <span style="width: 100%;height: 100%;" v-if="bookingsetting.bookingmode == 'profile' && !item.isselfbooking">{{item.mobilephone? formatmobile(item.mobilephone,true):item.email?item.email:item.firstname}}</span>
                    </td>
                    <td :style="'width: 100%;height: 30px;border: 1px solid #DCDCDC;background-color:'+(bookingsetting.bookingmode == 'table' && item.Noshowbooking?'#f3f3f3':'#fff')"
                      v-for="(timeslotitem,timeslotindex) in item.timeslot" :colspan="timeslotitem.colspan" >
                      <template v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.status != 1">
                          <div v-if="timeslotitem.bookingdetail.iswalkin"
                            :style="'overflow: hidden;display:flex;flex;flex-direction: row;justify-content: flex-start;align-items: center;padding-left: 5px;width: 100%;height: 30px;background-color:'+(bookingsetting.bookingmode == 'table' && (timeslotitem.bookingdetail.status == 4 || timeslotitem.bookingdetail.isnoshow)?'#ff8787':timeslotitem.bookingdetail.status == 3?(timeslotitem.bookingdetail.iswalkin?'#C99A3E':'#7a93ff'):'#fffe48')+';position: relative;z-index:2;'+(item.timeslot[timeslotindex + 1] && item.timeslot[timeslotindex + 1].bookingdetail?'border-right:2px solid #000':'')">
                            <i class="el-icon-star-off" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.customerrequest" style="font-size: 19px;margin-right: 3px;opacity: 0.8;"></i>
                            <i class="iconfont icon-cake" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.specialoccasion == 1" style="font-size: 19px;margin-right: 3px;"></i>
                            <span
                              style="color:#1e1aff;font-weight: bold;overflow: hidden;font-style: italic;font-size: 14px;text-align: left;z-index: 2 !important;">{{timeslotitem.bookingdetail.firstname?timeslotitem.bookingdetail.firstname+' , ':''}}{{timeslotitem.bookingdetail.peoplecount > 1 ?timeslotitem.bookingdetail.peoplecount + ' pers.':timeslotitem.bookingdetail.peoplecount + ' per.'}}</span>
                          </div>
                          <div v-else-if="!timeslotitem.bookingdetail.iscompleted && timeslotitem.bookingdetail.iscanunseat" style="position: relative;">
                          <el-popover v-if='showPop && activeId == timeslotitem' :style="'position: absolute;top: -100px;background-color: #fff;left: -'+(referencewidth / 4)+'px;'"
                            ref='pop'
                            :reference='reference'
                            placement="top"
                            :width="referencewidth"
                            trigger="click" :value="true">
                            <div style="display: flex;justify-content: center;align-items: center;width: 100%;flex-direction: column;">
                              <div style="display: flex;justify-content: center;align-items: center;width: 100%;flex-direction: row;justify-content: center;align-items: center;height: 40px;">
                                <span style="font-size: 18px;">Table {{item.Tablename}}</span>
                                <span v-if="timeslotitem && timeslotitem.bookingdetail && timeslotitem.bookingdetail.timeslot"
                                style="margin-left: 30px;font-size: 18px;">{{timeslotitem.bookingdetail.timeslot}}</span>
                                <span v-else style="margin-left: 30px;font-size: 18px;">{{timeslotitem.subtimeslot}}</span>
                              </div>
                              <div style="display: flex;justify-content: center;align-items: center;width: 100%;">
                              <el-button @click="editbooking(timeslotitem.bookingdetail)">Edit</el-button>
                              <el-button type="danger" @click="closetranspose(timeslotitem);unseat(timeslotitem)">UnSeat</el-button>
                              <el-button @click="closetranspose(timeslotitem)">Close</el-button>
                              </div>
                            </div>
                            <!-- <div
                              :style="'overflow: hidden;display:flex;flex;flex-direction: row;justify-content: flex-start;align-items: center;padding-left: 5px;width: 100%;height: 30px;background-color:'+(timeslotitem.bookingdetail.isnoshow?'#ff8787':timeslotitem.bookingdetail.status == 3?(timeslotitem.bookingdetail.iswalkin?'#C99A3E':'#7a93ff'):'#fffe48')+';cursor: pointer;position: relative;z-index:2;'+(item.timeslot[timeslotindex + 1] && item.timeslot[timeslotindex + 1].bookingdetail?'border-right:2px solid #000':'')">
                              <i class="el-icon-star-off" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.customerrequest" style="font-size: 19px;margin-right: 3px;opacity: 0.8;"></i>
                              <i class="iconfont icon-cake" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.specialoccasion == 1" style="font-size: 19px;margin-right: 3px;"></i>
                              <span
                                style="color:#1e1aff;font-weight: bold;overflow: hidden;font-style: italic;font-size: 14px;text-align: left;z-index: 2 !important;">{{timeslotitem.bookingdetail.firstname?timeslotitem.bookingdetail.firstname+' , ':''}}{{timeslotitem.bookingdetail.peoplecount > 1 ?timeslotitem.bookingdetail.peoplecount + ' pers.':timeslotitem.bookingdetail.peoplecount + ' per.'}}</span>
                            </div> -->
                          </el-popover>
                            <div @click="showpopovertranspose(timeslotitem);"
                              :style="'cursor:pointer;overflow: hidden;display:flex;flex;flex-direction: row;justify-content: flex-start;align-items: center;padding-left: 5px;width: 100%;height: 30px;background-color:'+(bookingsetting.bookingmode == 'table' && (timeslotitem.bookingdetail.status == 4 || timeslotitem.bookingdetail.isnoshow)?'#ff8787':timeslotitem.bookingdetail.status == 3?(timeslotitem.bookingdetail.iswalkin?'#C99A3E':'#7a93ff'):'#fffe48')+';position: relative;z-index:2;'+(item.timeslot[timeslotindex + 1] && item.timeslot[timeslotindex + 1].bookingdetail?'border-right:2px solid #000':'')">
                              <i class="el-icon-star-off" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.customerrequest" style="font-size: 19px;margin-right: 3px;opacity: 0.8;"></i>
                              <i class="iconfont icon-cake" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.specialoccasion == 1" style="font-size: 19px;margin-right: 3px;"></i>
                              <span
                                style="color:#1e1aff;font-weight: bold;overflow: hidden;font-style: italic;font-size: 14px;text-align: left;z-index: 2 !important;">{{timeslotitem.bookingdetail.firstname?timeslotitem.bookingdetail.firstname+' , ':''}}{{timeslotitem.bookingdetail.peoplecount > 1 ?timeslotitem.bookingdetail.peoplecount + ' pers.':timeslotitem.bookingdetail.peoplecount + ' per.'}}</span>
                            </div>
                          </div>
                          <div v-else
                            :style="'overflow: hidden;display:flex;flex;flex-direction: row;justify-content: flex-start;align-items: center;padding-left: 5px;width: 100%;height: 30px;background-color:'+((timeslotitem.bookingdetail.status == 4 || timeslotitem.bookingdetail.isnoshow)?'#ff8787':timeslotitem.bookingdetail.status == 3?(timeslotitem.bookingdetail.iswalkin?'#C99A3E':'#7a93ff'):'#fffe48')+';position: relative;z-index:2;'+(item.timeslot[timeslotindex + 1] && item.timeslot[timeslotindex + 1].bookingdetail?'border-right:2px solid #000':'')">
                            <i class="el-icon-star-off" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.customerrequest" style="font-size: 19px;margin-right: 3px;opacity: 0.8;"></i>
                            <i class="iconfont icon-cake" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.specialoccasion == 1" style="font-size: 19px;margin-right: 3px;"></i>
                            <span
                              style="color:#1e1aff;font-weight: bold;overflow: hidden;font-style: italic;font-size: 14px;text-align: left;z-index: 2 !important;">{{timeslotitem.bookingdetail.firstname?timeslotitem.bookingdetail.firstname+' , ':''}}{{timeslotitem.bookingdetail.peoplecount > 1 ?timeslotitem.bookingdetail.peoplecount + ' pers.':timeslotitem.bookingdetail.peoplecount + ' per.'}}</span>
                          </div>
                      </template>
                      <template v-if="timeslotitem.bookingdetail && (timeslotitem.bookingdetail.status == 1 || timeslotitem.bookingdetail.status == 2)">
                        <div style="position: relative;">
                        <el-popover v-if='showPop && activeId == timeslotitem' :style="'position: absolute;top: -100px;background-color: #fff;left: -'+(referencewidth / 4)+'px;'"
                          ref='pop'
                          :reference='reference'
                          placement="top"
                          :width="referencewidth"
                          trigger="click" :value="true">
                          <div style="display: flex;justify-content: center;align-items: center;width: 100%;flex-direction: column;">
                            <div style="display: flex;justify-content: center;align-items: center;width: 100%;flex-direction: row;justify-content: center;align-items: center;height: 40px;">
                              <span style="font-size: 18px;">Table {{item.Tablename}}</span>
                              <span v-if="timeslotitem && timeslotitem.bookingdetail && timeslotitem.bookingdetail.timeslot"
                              style="margin-left: 30px;font-size: 18px;">{{timeslotitem.bookingdetail.timeslot}}</span>
                              <span v-else style="margin-left: 30px;font-size: 18px;">{{timeslotitem.subtimeslot}}</span>
                            </div>
                            <div style="display: flex;justify-content: center;align-items: center;width: 100%;">
                            <el-button @click="editbooking(timeslotitem.bookingdetail)">Edit</el-button>
                            <template v-if="!timeslotitem.bookingdetail.tableidlist || timeslotitem.bookingdetail.tableidlist.indexOf(',') <= -1">
                                <el-button type="warning" @click="transposebooking(timeslotitem)">Transfer</el-button>
                            </template>
                            <template v-else>
                                <el-button type="warning" @click="transposebooking(timeslotitem,item)">Transfer</el-button>
                                <!-- <el-button type="warning" @click="transposebooking(timeslotitem)">All Transfer</el-button> -->
                            </template>
                            <el-button type="success" @click="closetranspose(timeslotitem);seatbooking(timeslotitem.bookingdetail)">Seat</el-button>
                            <el-button @click="closetranspose(timeslotitem)">Close</el-button>
                            </div>
                          </div>
                          <!-- <div
                            :style="'overflow: hidden;display:flex;flex;flex-direction: row;justify-content: flex-start;align-items: center;padding-left: 5px;width: 100%;height: 30px;background-color:'+(timeslotitem.bookingdetail.isnoshow?'#ff8787':timeslotitem.bookingdetail.status == 3?(timeslotitem.bookingdetail.iswalkin?'#C99A3E':'#7a93ff'):'#fffe48')+';cursor: pointer;position: relative;z-index:2;'+(item.timeslot[timeslotindex + 1] && item.timeslot[timeslotindex + 1].bookingdetail?'border-right:2px solid #000':'')">
                            <i class="el-icon-star-off" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.customerrequest" style="font-size: 19px;margin-right: 3px;opacity: 0.8;"></i>
                            <i class="iconfont icon-cake" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.specialoccasion == 1" style="font-size: 19px;margin-right: 3px;"></i>
                            <span
                              style="color:#1e1aff;font-weight: bold;overflow: hidden;font-style: italic;font-size: 14px;text-align: left;z-index: 2 !important;">{{timeslotitem.bookingdetail.firstname?timeslotitem.bookingdetail.firstname+' , ':''}}{{timeslotitem.bookingdetail.peoplecount > 1 ?timeslotitem.bookingdetail.peoplecount + ' pers.':timeslotitem.bookingdetail.peoplecount + ' per.'}}</span>
                          </div> -->
                        </el-popover>
                        <div :class="(timeslotitem.bookingdetail && timeslotitem.bookingdetail.id == transposeid && (!transposetableid || transposetableid == item.Id)) ?'fadeanimation':''" :ref="'bt'+item" @click="showpopovertranspose(timeslotitem);"
                          :style="'overflow: hidden;display:flex;flex;flex-direction: row;justify-content: flex-start;align-items: center;padding-left: 5px;width: 100%;height: 30px;background-color:'+(bookingsetting.bookingmode == 'table' && (timeslotitem.bookingdetail.status == 4 || timeslotitem.bookingdetail.isnoshow)?'#ff8787':timeslotitem.bookingdetail.status == 3?(timeslotitem.bookingdetail.iswalkin?'#C99A3E':'#7a93ff'):'#fffe48')+';cursor: pointer;position: relative;z-index:2;'+(item.timeslot[timeslotindex + 1] && item.timeslot[timeslotindex + 1].bookingdetail?'border-right:2px solid #000':'')">
                          <i class="el-icon-star-off" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.customerrequest" style="font-size: 19px;margin-right: 3px;opacity: 0.8;"></i>
                          <i class="iconfont icon-cake" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.specialoccasion == 1" style="font-size: 19px;margin-right: 3px;"></i>
                          <i class="el-icon-date" v-if="timeslotitem.bookingdetail && timeslotitem.bookingdetail.specialoccasion == 2" style="font-size: 17px;margin-right: 3px;color: #565252;"></i>
                          <span
                            style="color:#1e1aff;font-weight: bold;overflow: hidden;font-style: italic;font-size: 14px;text-align: left;z-index: 2 !important;">{{timeslotitem.bookingdetail.firstname?timeslotitem.bookingdetail.firstname+' , ':''}}{{timeslotitem.bookingdetail.peoplecount > 1 ?timeslotitem.bookingdetail.peoplecount + ' pers.':timeslotitem.bookingdetail.peoplecount + ' per.'}}</span>
                        </div>
                        </div>
                      </template>
                      <template v-if="bookingsetting.bookingmode == 'table'">
                        <div @click="addbookingbytimeslot(item.Id,timeslotitem.subtimeslot)" v-if="!timeslotitem.bookingdetail"
                          style="width: 100%;height: 30px;cursor: pointer;">
                        </div>
                      </template>
                      <template v-else>
                        <div @click="editprofilebookingbytimeslot(item.timeslot)" v-if="!timeslotitem.bookingdetail"
                          style="width: 100%;height: 30px;cursor: pointer;">
                        </div>
                      </template>
                    </td>
                  </tr>
                </template>
              </table>
              <div v-else style="width: 100%;height:300px;font-size: 2em;text-align: center;line-height: 300px;"><span v-if="bookingsetting.bookingmode == 'profile'">There was no record of a reservation.</span></div>
            </div>

            <!-- 行程表格E -->
            <!-- <div v-if="timelist && timelist.length > 0"
                style="width:30px;height: 600px;margin-top: 20px;position: relative;display: flex;justify-content: center;align-items: center;">
                <i class="el-icon-chat-line-square" style="font-size: 28px;cursor: pointer;position: fixed;right: 0px;z-index: 999;"
                  @click="showright = !showright"></i>
              </div> -->
            <div v-dialogDrag id="divscroll" v-if="showselfbooking && selfbookingcalendar && selfbookingcalendar.length > 0"
                :style="'width:290px;height: '+rightheight + 'px'+';border: 1px solid #cdcdcd;position: absolute;background-color: #FFFFFF;top:70px;right:60px;z-index:9'">
                <div
                  style="width: 290px;height: 25px;display: flex;justify-content: center;align-items: center;position: relative;"
                  class="el-dragheader">
                  <i style="width: 15px;height: 1px;border-bottom: 1px solid black;"></i>
                  <i class="el-icon-close" style="cursor: pointer;position: absolute;right: 10px;top: 5px;"
                    @click="showselfbooking = false"></i>
                </div>
                <div class="el-dragcontent"
                  :style="'overflow: scroll;display: flex;flex-direction: column;justify-content: flex-start;align-items: center;height: '+(rightheight - 30) + 'px'">
                  <div style="margin-top:10px;">
                    <div style="width: 260px;height: 30px;text-align: center;">
                      <span>New Self-Booking</span>
                    </div>
                    <div v-for="(item,index) in selfbookingcalendar"
                      :style="'width: 270px;display: flex;flex-direction: column;min-height: '+(true?'155px':'135px')+';justify-content: center;align-items: center;margin-top:'+(index == 0?'10px':'10px')+';border: 1px solid #DCDCDC;border-radius: 10px;padding-top:10px;padding-bottom: 10px;background-color:#ffffdb'">
                      <div
                        style="font-size: 14px;width: 260px;height: 30px;border-bottom: 1px solid #f3f3f3;display: flex;justify-content: center;align-items: center;margin-bottom: 2px;">
                        <span style="position: relative;bottom: 3px;">{{item.date}}</span>
                      </div>
                      <div style="display: flex;flex-direction: row;justify-content: flex-start;width: 260px;">
                        <div
                          style="position: relative;width: 50px;display: flex;justify-content: center;align-items: center;margin-left: 10px;font-size: 14px;flex-direction: column;">
                          <span>{{item.timeslot}}</span>
                          <div v-if="item.customerrequest || item.specialoccasion || item.customerspecial || item.isselfbooking"
                            style="position: absolute;bottom:30px;display: flex;flex-direction: row;font-weight: bold;justify-content: center;align-items: center;">
                            <i v-if="item.isselfbooking" class="el-icon-user" style="margin-right: 2px;"></i>
                            <i v-if="item.customerrequest" class="el-icon-star-off" style="margin-right: 2px;"></i>
                            <i v-if="item.specialoccasion && item.specialoccasion == 1" class="iconfont icon-cake"
                                style="margin-right: 2px;font-size: 14px;color: #686868;" :title="'birthday'"></i>
                            <i v-if="item.specialoccasion && item.specialoccasion == 2" class="el-icon-date"
                                style="margin-right: 2px;font-size: 14px;color: #686868;" :title="'anniversary'"></i>
                            <i v-if="(item.specialoccasion && item.specialoccasion != 1 && item.specialoccasion != 2) || item.customerspecial" class="el-icon-chat-dot-round"
                                style="margin-right: 2px;" :title="item.customerspecial"></i>
                          </div>
                        </div>
                        <div style="display: flex;flex-direction: column;width: 120px;">
                          <div style="height: 25px;display: flex;align-items: center;font-size: 14px;"
                            v-if="(item.firstname && item.firstname.length > 0)">
                            {{item.firstname?item.firstname + (item.lastname? ' ' + item.lastname:''):''}}
                          </div>
                          <div style="height: 25px;display: flex;align-items: center;font-size: 14px;">
                            {{item.mobilephone}}
                          </div>
                          <div style="height: 25px;display: flex;align-items: center;font-size: 14px;">
                            {{item.peoplecount}}
                            people</div>
                          <div v-if="item.sectionname && sectionshowcalendar && sectionshowcalendar.length > 1"
                            style="font-weight: bold;height: 25px;display: flex;align-items: center;font-size: 14px;">
                            {{item.sectionname}}</div>
                          <div
                            style="height: 25px;display: flex;align-items: center;font-size: 13px;color: red;font-weight: bold;"
                            v-if="bookingsetting.bookingmode == 'table' && (!item.tablelist || item.tablelist.length <= 0)">No table assigned</div>
                          <div v-if="bookingsetting.bookingmode == 'table' && item.tablelist"
                            :style="'position:relative;min-height: 25px;display: flex;align-items: center;font-size: 13px;line-height:20px;color:'+(item.status == 4 || item.isnoshow?'#000':item.status == 3?'#000':'#42d885')+';font-weight: bold;word-break:break-all;'">
                            Table&nbsp;{{item.tablelist}}</div>
                          <div
                            :style="'min-height: 25px;display: flex;align-items: center;font-size: 13px;color:'+(bookingsetting.bookingmode == 'table' && (item.status == 4 || item.isnoshow)?'#000':item.status == 3?'#000':'#42d885')+';font-weight: bold;flex-direction: column;justify-content: flex-start;align-items: flex-start;'">
                            {{item.status == 3?(item.iscompleted?'Completed':'Seated'):''}}
                            <span v-if="bookingsetting.bookingmode == 'table' && (item.status == 4 || item.isnoshow)"
                              :style="'color: #000;margin-left: ' + (item.status==3?'5px':'0px') + ';font-size: 13px;'">No-Show</span>
                            <span v-if="item.iscancel"
                              style="margin-top: 5px;color: red;font-weight: bold;font-size: 14.5px;">Canceled</span>
                            <!-- <span v-if="item.iswalkin" style="margin-top: 5px;color: #000;font-weight: bold;font-size: 14.5px;">Walk-in</span> -->
                          </div>
                        </div>
                        <div
                          style="width: 70px;display: flex;flex-direction: column;align-items: center;font-size: 14px;justify-content: center;">
                          <div>
                            <el-button type="infor" @click="editbooking(item)" style="width: 90px;">Detail</el-button>
                          </div>
                          <div style="margin-top: 10px;">
                            <el-button :type="(!item.isnoshow && !item.status != 4)?'success':'danger'" @click="confirmbooking(item.id)">Confirm
                            </el-button>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div v-if="!selfbookingcalendar || selfbookingcalendar.length <= 0"
                      style="width: 260px;height: 500px;display: flex;justify-content: center;align-items: center;">
                      <span style="color: #808080">no data</span>
                    </div>
                  </div>
                </div>
              </div>

            <div v-dialogDrag id="divscroll" v-show="showright && timelist && timelist.length > 0"
              :style="'width:290px;height: '+rightheight + 'px'+';border: 1px solid #cdcdcd;position: absolute;background-color: #FFFFFF;top:70px;right:60px;z-index:9'">
              <div
                style="width: 290px;height: 25px;display: flex;justify-content: center;align-items: center;position: relative;"
                class="el-dragheader">
                <i style="width: 15px;height: 1px;border-bottom: 1px solid black;"></i>
                <i class="el-icon-close" style="cursor: pointer;position: absolute;right: 10px;top: 5px;"
                  @click="showright = false"></i>
              </div>
              <div class="el-dragcontent"
                :style="'overflow: scroll;display: flex;flex-direction: column;justify-content: flex-start;align-items: center;height: '+(rightheight - 30) + 'px'">
                <div style="width: 260px;height: 60px;margin-bottom:3px;">
                  <!-- <div style="width: 280px;height: 30px;border: 1px solid #cdcdcd;"></div> -->
                  <div style="width: 260px;height: 30px;">
                    <el-checkbox v-model="showallbooking" style="margin-left: 3px;margin-top: 3px;height: 30px;"
                      @change="getnewcalbookinglist">Show All</el-checkbox>
                  </div>
                  <el-input type="text" style="width: 260px;margin-top: 5px;height: 30px;" @input="searchbooking"
                    v-model="keyword" placeholder="Please enter keywords" prefix-icon="el-icon-search" clearable>
                  </el-input>
                </div>
                <div v-loading="loadsearch" style="margin-top:10px;">
                  <div v-for="(item,index) in showcaledatalist"
                    :style="'width: 260px;display: flex;flex-direction: column;min-height: '+(keyword && keyword.trim().length > 0?'155px':'135px')+';justify-content: center;align-items: center;margin-top:'+(index == 0?'10px':'10px')+';border: 1px solid #DCDCDC;border-radius: 10px;padding-top:10px;padding-bottom: 10px;background-color: '+ (bookingsetting.bookingmode == 'table' && (!item.tablelist || item.tablelist.length <= 0)?'#5ddadd':bookingsetting.bookingmode == 'table' && (item.status == 4 || item.isnoshow)?'#ff8787':item.isnottoday && item.status != 3?'#fff':item.status == 3?(item.iswalkin?'#C99A3E':'#7a93ff'):'#ffffdb')">
                    <div v-if="keyword && keyword.trim().length > 0"
                      style="font-size: 14px;width: 260px;height: 30px;border-bottom: 1px solid #f3f3f3;display: flex;justify-content: center;align-items: center;margin-bottom: 2px;">
                      <span style="position: relative;bottom: 3px;">{{item.date}}</span>
                    </div>
                    <div style="display: flex;flex-direction: row;justify-content: flex-start;width: 260px;">
                      <div
                        style="position: relative;width: 50px;display: flex;justify-content: center;align-items: center;margin-left: 10px;font-size: 14px;flex-direction: column;">
                        <span>{{item.timeslot}}</span>
                        <div v-if="item.customerrequest || item.specialoccasion || item.customerspecial || item.isselfbooking"
                          style="position: absolute;bottom:30px;display: flex;flex-direction: row;font-weight: bold;justify-content: center;align-items: center;">
                          <i v-if="item.isselfbooking" class="el-icon-user" style="margin-right: 2px;"></i>
                          <i v-if="item.customerrequest" class="el-icon-star-off" style="margin-right: 2px;"></i>
                          <i v-if="item.specialoccasion && item.specialoccasion == 1" class="iconfont icon-cake"
                            style="margin-right: 2px;font-size: 14px;color: #686868;" :title="'birthday'"></i>
                            <i v-if="item.specialoccasion && item.specialoccasion == 2" class="el-icon-date"
                                style="margin-right: 2px;font-size: 14px;color: #686868;" :title="'anniversary'"></i>
                          <i v-if="(item.specialoccasion && item.specialoccasion != 1 && item.specialoccasion != 2) || item.customerspecial" class="el-icon-chat-dot-round"
                            style="margin-right: 2px;" :title="item.customerspecial"></i>
                        </div>
                      </div>
                      <div style="display: flex;flex-direction: column;width: 120px;">
                        <div style="height: 25px;display: flex;align-items: center;font-size: 14px;"
                          v-if="(item.firstname && item.firstname.length > 0)">
                          {{item.firstname?item.firstname + (item.lastname? ' ' + item.lastname:''):''}}
                        </div>
                        <div style="height: 25px;display: flex;align-items: center;font-size: 14px;">
                          {{item.mobilephone}}
                        </div>
                        <div style="height: 25px;display: flex;align-items: center;font-size: 14px;">
                          {{item.peoplecount}}
                          people</div>
                        <div v-if="item.sectionname && sectionshowcalendar && sectionshowcalendar.length > 1"
                          style="font-weight: bold;height: 25px;display: flex;align-items: center;font-size: 14px;">
                          {{item.sectionname}}</div>
                        <div
                          style="height: 25px;display: flex;align-items: center;font-size: 13px;color: red;font-weight: bold;"
                          v-if="bookingsetting.bookingmode == 'table' && (!item.tablelist || item.tablelist.length <= 0)">No table assigned</div>
                        <div v-if="bookingsetting.bookingmode == 'table' && item.tablelist"
                          :style="'position:relative;min-height: 25px;display: flex;align-items: center;font-size: 13px;line-height:20px;color:'+(bookingsetting.bookingmode == 'table' && (item.status == 4 || item.isnoshow)?'#000':item.status == 3?'#000':'#42d885')+';font-weight: bold;word-break:break-all;'">
                          Table&nbsp;{{item.tablelist}}</div>
                        <div
                          :style="'min-height: 25px;display: flex;align-items: center;font-size: 13px;color:'+(bookingsetting.bookingmode == 'table' && (item.status == 4 || item.isnoshow)?'#000':item.status == 3?'#000':'#42d885')+';font-weight: bold;flex-direction: column;justify-content: flex-start;align-items: flex-start;'">
                          {{item.status == 3?(item.iscompleted?'Completed':'Seated'):''}}
                          <span v-if="bookingsetting.bookingmode == 'table' && (item.status == 4 || item.isnoshow)"
                            :style="'color: #000;margin-left: ' + (item.status==3?'5px':'0px') + ';font-size: 13px;'">No-Show</span>
                          <span v-if="item.iscancel"
                            style="margin-top: 5px;color: red;font-weight: bold;font-size: 14.5px;">Canceled</span>
                          <!-- <span v-if="item.iswalkin" style="margin-top: 5px;color: #000;font-weight: bold;font-size: 14.5px;">Walk-in</span> -->
                        </div>
                      </div>
                      <div
                        style="width: 70px;display: flex;flex-direction: column;align-items: center;font-size: 14px;justify-content: center;">
                        <div v-if="!item.iscancel && item.status != 3 && item.status != 4">
                          <el-button type="infor" @click="editbooking(item)">Edit</el-button>
                        </div>
                        <div v-if="!item.iscancel && (item.status == 3 || item.status == 4)">
                          <el-button type="infor" @click="editbooking(item)">Detail</el-button>
                        </div>
                        <div v-if="bookingsetting.bookingmode == 'table' && item.status == 1 && !item.isnottoday && item.tablelist && item.tablelist.length > 0" style="margin-top: 10px;">
                          <el-button :type="(!item.isnoshow && !item.status != 4)?'success':'danger'" @click="seatbooking(item)">Seat</el-button>
                        </div>
                        <div v-if="item.status == 1 && (item.isnoshow || item.status == 4)" style="margin-top: 10px;">
                          <div data-v-48b869f2="" @click="cancelbooking(item.id)" class="el-button" style="
                                    width: 72px;
                                    text-align: center;
                                    height: 35px;
                                    padding: 0px;
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                    background: #42d885;
                                    border-color: #42d885;
                                    color: #fff;
                                ">Cancel</div>
                        </div>
                        <div v-if="bookingsetting.bookingmode == 'table' && item.status == 3 && !item.iscompleted" style="margin-top: 10px;">
                          <el-button type="warning" @click="completebooking(item)">Finish</el-button>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div v-if="!showcaledatalist || showcaledatalist.length <= 0"
                    style="width: 260px;height: 500px;display: flex;justify-content: center;align-items: center;">
                    <span style="color: #808080">no data</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>


    <el-dialog :visible.sync="showbookinginfovisible" v-if="selectbookinginfo" :width="'450px'">
      <div
        style="text-align: center;width: 100%;justify-content: center;align-items: center;font-size: 21px;margin-bottom: 20px;">
        Booking Detail</div>
      <div class="detailrow">
        <span style="font-size: 14px;color: rgb(50, 50, 50);">Name</span><span
          style="font-size: 14px;color: rgb(50, 50, 50);">{{selectbookinginfo.firstname}}</span>
      </div>
      <div class="detailrow" v-if="selectbookinginfo.mobilephone">
        <span style="font-size: 14px;color: rgb(50, 50, 50);">Mobile</span><span
          style="font-size: 14px;color: rgb(50, 50, 50);">{{selectbookinginfo.mobilephone}}</span>
      </div>
      <div class="detailrow" v-if="selectbookinginfo.email">
        <span style="font-size: 14px;color: rgb(50, 50, 50);">Email</span><span
          style="font-size: 14px;color: rgb(50, 50, 50);">{{selectbookinginfo.email}}</span>
      </div>
      <div class="detailrow">
        <span style="font-size: 14px;color: rgb(50, 50, 50);">People</span><span
          style="font-size: 14px;color: rgb(50, 50, 50);">{{selectbookinginfo.peoplecount}}</span>
      </div>
      <div class="detailrow">
        <span style="font-size: 14px;color: rgb(50, 50, 50);">Time</span><span
          style="font-size: 14px;color: rgb(50, 50, 50);">{{!selectbookinginfo.utcebookingtime?selectbookinginfo.utcbookingtime:selectbookinginfo.utcebookingtime}}</span>
      </div>
      <div class="detailrow"
        style="display: flex;justify-content: center;align-items: center;border: 0px;margin-top: 20px;">
        <el-button style="width: 150px;height: 50px;font-size:15px" @click="seatconfirmbooking(selectbookinginfo.id)"
          type="primary">Seat Booking</el-button>
        <el-button style="width: 150px;height: 50px;margin-left: 30px;font-size:15px"
          @click="showbookinginfovisible = false">Exit</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
  import request from '@/api/request.js'
  import calendar from './main.vue'
  import signalrhelper from '@/utils/signalrglobal.js'

  var timer = null
  var timeslotline = null

  export default {
    name: 'BookingKeep',
    components: {
      calendar
    },
    data() {
      return {
        value: '',
        valuestr: '',
        ontouchstart: false,
        showcalendar: true,

        showcaledatalist: [],


        selfbookingcalendar:[],
        showselfbooking:false,

        caledatalist: [],

        page: {},
        timelist: [],
        timelistap: [],

        timeslotlist: [],

        timeslotfromdatalist: [],
        showright: false,
        keyword: '',
        loadsearch: false,
        searchcaledatalist: [],

        leftwidth: 0,
        leftshowrighwidth: 0,
        leftheight: 0,
        rightheight: 0,

        sectionshowcalendar: null,
        selectsectionid: '',
        showallbooking: false,

        totalparty: 0,
        totalpersion: 0,

        tableshowcalendar:null,
        dqlinecalendar:null,

        transposeid:null,
        transposebookdate:null,
        transposetimeslot:null,
        transposetableid:null,

        scrolltop:0,
        scrollleft:0,
        bookingsetting:null,
        showbookinginfovisible:false,
        selectbookinginfo:null,


        reference:{},
        // 控制渲染条件 如果不用v-if会报错 具体可自行尝试
        showPop: false,
        referencewidth:0,
        activeId:'',
        isenablebooking:true,
        phonenumber:'',

        profileBookingChangeRadio:'date', // 'date' | 'all' 指定页面是查看某一天还是查看一个日期范围，默认是date
        profileBookingSelectDate:[], // all显示的 一个日期范围
      }
    },
    activated() {
      var that = this
      that.getbookingsetting()

      if(document.getElementById("timelistscroll")){
        document.getElementById("timelistscroll").scrollTop = that.scrolltop
        document.getElementById("timelistscroll").scrollLeft = that.scrollleft
      }


      that.transposeid = null
      if (that.keyword && that.keyword.trim() != '') {
        that.searchbooking()
      } else {
        if (signalrhelper.GetStatus() == 1) {
          that.getbookinglistbycalendar(true)
        }
      }
      that.gettimelineslot()
    },
    created() {
      var that = this
      that.getbookingsetting()
      timeslotline = setInterval(function() {
        that.gettimelineslot()
      }, 2000)


      signalrhelper.GetHubProxy().on("getBookingNewData", function() {
        that.getbookinglistbycalendar(true)
      })

      that.$nextTick(() => {
        // 点击前一个月
        let prevBtn = document.querySelector(
          ".el-calendar__button-group .el-button-group>button:nth-child(1)"
        );
        prevBtn.addEventListener("click", e => {
          var nowDate = new Date(that.value);
          var year = nowDate.getFullYear();
          var month = nowDate.getMonth() + 1 < 10 ? "0" + (nowDate.getMonth() + 1) :
            nowDate.getMonth() + 1;
          var day = nowDate.getDate() < 10 ? "0" + nowDate.getDate() : nowDate
            .getDate();
          that.value = (year + "/" + month + "/" + day);
          that.valuestr = (year + "/" + month + "/" + day);
          that.getbookinglistbycalendar(false)
        });

        //点击下一个月
        let nextBtn = document.querySelector(
          ".el-calendar__button-group .el-button-group>button:nth-child(3)"
        );
        nextBtn.addEventListener("click", () => {
          var nowDate = new Date(that.value);
          var year = nowDate.getFullYear();
          var month = nowDate.getMonth() + 1 < 10 ? "0" + (nowDate.getMonth() + 1) :
            nowDate.getMonth() + 1;
          var day = nowDate.getDate() < 10 ? "0" + nowDate.getDate() : nowDate
            .getDate();
          that.value = (year + "/" + month + "/" + day);
          that.valuestr = (year + "/" + month + "/" + day);
          that.getbookinglistbycalendar(false)
        });
      })


      var nowDate = new Date();
      var year = nowDate.getFullYear();
      var month = nowDate.getMonth() + 1 < 10 ? "0" + (nowDate.getMonth() + 1) :
        nowDate.getMonth() + 1;
      var day = nowDate.getDate() < 10 ? "0" + nowDate.getDate() : nowDate
        .getDate();
      that.value = (year + "-" + month + "-" + day).replace(/-/g, "/");
      that.valuestr = (year + "-" + month + "-" + day).replace(/-/g, "/");

      // that.getbookinglistbycalendar()

      var inittimer = setInterval(function() {
        if (signalrhelper.GetStatus() == 1) {
          that.getbookinglistbycalendar()
          clearInterval(inittimer)
          inittimer = null
        }
      }, 500)

      // timer = setInterval(function() {
      //   if(!that.keyword || that.keyword.trim() == ''){
      //       that.getbookinglistbycalendar(true)
      //   }
      // }, 15000)


      setInterval(function() {
        //这里定期更新一下分钟
        var myDate = new Date()
        var hour = myDate.getHours();       // 获取当前小时数(0-23)
        var min = myDate.getMinutes();     // 获取当前分钟数(0-59)
        if(hour == 0 && (min == 0 || min == 1 || min == 15)){
          that.getbookinglistbycalendar(true)
        }
      }, 60000)



      setInterval(function(){
        that.autotimelineextension()
      },30000)
    },
    mounted() {
      var that = this

      that.scrolltop = 0
      that.scrollleft = 0

      for (var i = 0; i < 24; i++) {
        var timestr = i
        if (i.toString().length == 1) {
          timestr = '0' + i
        }
        that.timelist.push(timestr + ':00')
      }
      var screenwidth = window.screen.availWidth
      var screenheight = window.innerHeight

      var leftwidth = screenwidth;
      var leftshowrighwidth = screenwidth;
      that.leftwidth = leftwidth
      that.leftshowrighwidth = leftshowrighwidth
      that.leftheight = screenheight - 60 - 50 - 50
      that.rightheight = screenheight - 60 - 45
    },
    destroyed() {
      clearInterval(timer)
    },
    methods: {
      editprofilebookingbytimeslot(timeslot){
        var that = this
        if(timeslot){
          timeslot.forEach((timeslotitem,timeslotindex) =>{
             if(timeslotitem.bookingdetail){
               that.editbooking(timeslotitem.bookingdetail)
               return false
             }
          })
        }
        //(timeslotitem,timeslotindex) in timeslot
      },
      /*
       * 格式化手機號，转换成北美格式
       */
      formatmobile(mobile,ishavemobile){
      	var that = this
      	if(mobile){
      		if(!ishavemobile){
      			if(mobile[0] && mobile[0] != '(' && mobile.indexOf('(') <= -1){
      				mobile = '(' + mobile
      			}
      			if(mobile[3] && mobile.length == 4 && mobile.indexOf(')') <= -1){
      				mobile = that.insert_flg(mobile,')',4)
      			}
      			if(mobile[7] && mobile.length == 8 && mobile.indexOf('-') <= -1){
      				mobile = that.insert_flg(mobile,'-',8)
      			}
      		}else{
      			if(mobile.indexOf('(') <= -1){
      				mobile = '(' + mobile
      			}
      			if(mobile.length >= 4 && mobile.indexOf(')') <= -1){
      				mobile = mobile.substring(0,4) +')' + mobile.substring(4,mobile.length)
      			}
      			if(mobile.length >= 8 && mobile.indexOf('-') <= -1){
      				mobile = mobile.substring(0,8) +'-' + mobile.substring(8,mobile.length)
      			}
      		}
      	}
      	return mobile
      },
      insert_flg(str,flg,sn){
          var newstr="";
          for(var i=0;i<str.length;i+=sn){
              var tmp=str.substring(i, i+sn);
              newstr+=tmp+flg;
          }
          return newstr;
      },
      getNowFormatDate() {
        var date = new Date();
        var seperator1 = "/";
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var strDate = date.getDate();
        if (month >= 1 && month <= 9) {
          month = "0" + month;
        }
        if (strDate >= 0 && strDate <= 9) {
          strDate = "0" + strDate;
        }
        var currentdate = year + seperator1 + month + seperator1 + strDate;
        return currentdate;
      },
      //时间轴延长
      autotimelineextension(){
        var that = this
        if(that.getNowFormatDate() != that.valuestr){
          return
        }
        try{
          if(that.timeslotfromdatalist){
            that.timeslotfromdatalist.forEach((item,index)=>{
              if(item.timeslot){
                var timeslot = JSON.parse(JSON.stringify(item.timeslot))
                for(var i = 0;i< timeslot.length;i++){
                  if(timeslot[i] && timeslot[i].bookingdetail){
                    if(timeslot[i].bookingdetail.status == 3 && !timeslot[i].bookingdetail.iscompleted){
                      var pd = false //查询一下前面有没有seat的
                      for(var j = i + 1;j < timeslot.length;j++){
                         if(timeslot[j] && timeslot[j].bookingdetail && timeslot[j].bookingdetail.status == 3){
                           pd = true
                           break
                         }
                      }
                      if(pd){
                        continue
                      }
                      // console.log(item)
                      // console.log(timeslot[i])
                      // console.log(timeslot[i].bookingdetail)
                      // console.log(timeslot[i].bookingdetail.utceturntime.replace(/-/g, "/"))

                      if(new Date().getTime() > new Date(timeslot[i].bookingdetail.utceturntime.replace(/-/g, "/")).getTime()){
                        for(var j = i + 1;j < timeslot.length;j++){
                           if(timeslot[j] && !timeslot[j].bookingdetail && new Date().getTime() > new Date(that.valuestr.replace(/-/g, "/") + ' ' + timeslot[j].subtimeslot + ':00').getTime()){
                             timeslot[i].colspan = timeslot[i].colspan + timeslot[j].colspan
                             timeslot.splice(j,1)
                             j--;
                           }else{
                             break
                           }
                        }
                      }
                    }
                  }
                }
                item.timeslot = timeslot
              }
            })
          }
          that.$forceUpdate()
        }catch(e){

        }
      },
      // showbookinginfo(bookingid) {
      //   var that = this
      //   that.selectbookinginfo = null
      //   that.tablelayout.forEach((item, index) => {
      //     if (item.bookingdetail && item.bookingdetail.id.toLowerCase() == bookingid.toLowerCase()) {
      //       that.selectbookinginfo = item.bookingdetail
      //       that.showbookinginfovisible = true
      //       return false
      //     }
      //   })
      // },
      getbookingsetting(){
        var that = this
        request.requestPost('/RestaurantBooking/BookingGetAllSetting', {
          hid: that.hid
        }, function(res) {
          try {
            if (res.data.locationbookingconfig && res.data.locationbookingconfig.length > 0) {
              //利用  Object.assign 把数据库可能没有的属性给增加上
              that.bookingsetting = Object.assign(that.bookingconfig(), JSON.parse(res.data.locationbookingconfig));
            } else {
              that.bookingsetting = that.bookingconfig()
            }
          } catch (e) {
            that.bookingsetting = that.bookingconfig()
          }
          that.tabletypelist = res.data.tabletypelist


          that.isenablebooking = res.data.isenablebooking
          if(!that.isenablebooking){
            that.$router.push({
              path: '/booking/tablelayout'
            })
          }
        })
      },
      bookingconfig() {
        return {
          enable: false,
          submitverifymobile: false,
          timelist: null,
          leastminutes: 60,
          searchlimittimeminute: 120,
          noshowmminute: 0,
          orderspecialoccasions: true,
          orderspecialentered: true,

          ordersendsms: false,
          ordersendsmscontent: 'Hi ${customername},your booking at ${storesignature} confirmed. Details:Table for ${peoplecount} on ${bookingtime}. We look forward to seeing you!',

          cancelbookingordersendsms: false,
          cancelbookingordersendsmscontent: '',

          editbookingordersendsms: false,
          editbookingordersendsmscontent: '',

          inadvancesendsms: false,
          inadvanceleasthour: 1,
          inadvancesendsmscontent: '',


          onlyshowwalkinminute: 120,
          tipscontinuewalkinminute: 30,
          showhavebooktableredminute:60,
        }
      },
      clickPop(item){
      },
      showpopovertranspose(timeslot){
        var that = this;
        console.log(timeslot.bookingdetail)
        if(timeslot.bookingdetail.issimple){
          that.editbooking(timeslot.bookingdetail)
          return
        }
        that.showPop = true
        that.referencewidth = !timeslot.bookingdetail.tableidlist || timeslot.bookingdetail.tableidlist.indexOf(',') <= -1?380:380

                  // 这个操作是为了避免与源码中的点击reference doToggle方法冲突
                  // if (this.activeId === timeslot && this.showPop) return
                  // this.showPop = false
                  this.activeId = timeslot
                  // 因为reference是需要获取dom的引用 所以需要是$el
                  this.reference = this.$refs['bt'+timeslot][0].$el
                  this.$nextTick(() => {
                      // 等待显示的popover销毁后再 重新渲染新的popover
                      this.showPop = true
                      this.$nextTick(() => {
                          // 此时才能获取refs引用
                          setTimeout(function(){
                            // this.$refs.pop.doShow()
                          },500)

                      })
                  })

        that.$forceUpdate()
      },
      unseat(timeslot){
        var that = this
        that.$confirm('Are you sure to unseat?')
          .then(_ => {
            var data = {
              bookingid: timeslot.bookingdetail.id
            }
            that.startLoading()
            request.requestPost('/RestaurantBooking/UnSeatBooking', data, function(res) {
              that.endLoading()
              if (res.statuscode == 200) {
                that.$message({
                  message: 'Successfully',
                  type: 'success'
                })
                if (!that.keyword || that.keyword.trim() == '') {
                  that.startLoading()
                  that.getbookinglistbycalendar(true)
                } else {
                  that.getbookinglistbycalendar(true, true)
                  that.searchbooking()
                }
              }
            })
          })
          .catch(_ => {});
        return
      },
      closetranspose(timeslot){
        var that = this
        timeslot.showpopover = false
        timeslot.flicker = false
        that.transposeid = null
        that.showPop = false
        that.activeId = null
        that.$forceUpdate()
      },
      selecttablename(item) {
        var that = this

        that.timeslotfromdatalist.forEach((ditem, dindex) => {
          if (ditem.Id != item.Id) {
            ditem.selectthis = false
          }
        })
        that.$forceUpdate()
        that.$nextTick(() => {
          item.selectthis = !item.selectthis
          that.$forceUpdate()
        })
      },
      getnewcalbookinglist() {
        var that = this
        if (!that.keyword || that.keyword.trim() == '') {
          that.getbookinglistbycalendar(true)
        } else {
          that.searchbooking()
        }
      },
      completebooking(item) {
        var that = this
        that.$confirm('Whether to change the state of the booking to completed?')
          .then(_ => {
            var data = {
              bookingid: item.id
            }
            that.startLoading()
            request.requestPost('/RestaurantBooking/Completebooking', data, function(res) {
              that.endLoading()
              if (res.statuscode == 200) {
                that.$message({
                  message: 'Successfully',
                  type: 'success'
                })
                that.getbookinglistbycalendar(false)
              }
            })
          })
          .catch(_ => {});
      },
      searchphonenumber(){
        var that = this
        if(that.timeslotfromdatalist){
          that.timeslotfromdatalist.forEach((ditem, dindex) => {
            if (that.phonenumber && that.phonenumber.length > 0 &&  ditem.mobilephone && ditem.mobilephone.indexOf(that.phonenumber) >= 0) {
              ditem.selectthis = true
            }else{
              ditem.selectthis = false
            }
          })
          that.$forceUpdate()
        }
      },
      //时间轴红线
      gettimelineslot(isremoveel) {
        var that = this
        var dqtime = ''
        let yy = new Date().getFullYear()
        let mm = new Date().getMonth() + 1
        let dd = new Date().getDate() < 10 ? '0' + new Date().getDate() : new Date().getDate()
        let hh = new Date().getHours() < 10 ? '0' + new Date().getHours() : new Date().getHours()
        let mf = new Date().getMinutes() < 10 ? '0' + new Date().getMinutes() : new Date().getMinutes()
        let ss = new Date().getSeconds() < 10 ? '0' + new Date().getSeconds() : new Date().getSeconds()
        dqtime = hh + ':' + mf

        if(mm < 10){
          mm = '0' + mm
        }
        if ((yy + '/' + mm + '/' + dd) == that.valuestr) {
          var array = document.getElementsByClassName('timehour')
          for (var i = 0; i < array.length; i++) {
            var hour = array[i].innerHTML.split(':')[0]
            if (hour == hh) {
              var timetable = document.getElementById('timetable')
              var timelistaptr = document.getElementById('timelistaptr')
              var offsetTop = array[i].parentNode.offsetTop


              var offsetheight = timetable.offsetHeight
              if(timelistaptr){
                offsetheight = offsetheight - timelistaptr.offsetHeight
              }
              if(offsetTop){
                offsetheight = offsetheight - offsetTop
              }

              var thwidth = array[i].parentNode.offsetWidth
              var theverymin = thwidth / 60
              var el = document.getElementById('nowtimeslot')
              if (el) {
                if (el.classList == (yy + mm + dd + hh + mf) && !isremoveel) {
                  return
                } else {
                  el.remove()
                }
              }
              var dqpercount = that.getdqpercount()

              var afterhtml = document.createElement('div')
              afterhtml.id = 'nowtimeslot'
              afterhtml.classList = yy + mm + dd + hh + mf
              afterhtml.style = 'width:1px;border-right: 1px solid red;height: ' + (offsetheight) +
                'px;position: absolute;top: 0px;left:' + (theverymin * parseInt(mf)) +
                'px;z-index: -1;pointer-events: none;'
                if(that.timelistap && that.timelistap.length > 0){
                  afterhtml.innerHTML = '<span style="z-index:999;position: absolute;top:-13px;color:red;font-weight: bold;width: 100px;text-align: center;">' + dqpercount + (dqpercount > 1?' pers':' per') + '<span>'
                }

              array[i].parentNode.insertBefore(afterhtml, array[i])
            }
          }
        }
      },
      getdqpercount(){
        var that = this
        var peoplecount = 0
        var newdate = new Date()
        that.dqlinecalendar.forEach((item,index)=>{
            if(newdate >= new Date(item.utcbookingtime.replace(/-/g, "/")) && (newdate <= new Date(item.utcturntime.replace(/-/g, "/")) || (!item.iscompleted && item.status == 3))){
              peoplecount += item.peoplecount
            }
        })
        return peoplecount
      },
      changeselectsectionid(sectionid) {
        var that = this
        that.timeslotfromdatalist = null
        that.selectsectionid = sectionid


        that.sectionshowcalendar.forEach((item, index) => {
          if (that.bookingsetting.bookingmode == 'profile' || item.sectionid == that.selectsectionid) {
            console.log('进来了这里1')
            that.timeslotfromdatalist = JSON.parse(JSON.stringify(item.tablelist))

            var el = document.getElementById('nowtimeslot')
            if (el) {
              el.remove()
            }
            that.$nextTick(() => {
              try {

                setTimeout(function() {
                  var timetable = document.getElementById('timetable')
                  var timelistaptr = document.getElementById('timelistaptr')
                  var classlist = document.getElementsByClassName('borderrightposition')
                  for (var item in classlist) {
                    if (classlist[item] && classlist[item].style && classlist[item].style.height) {
                      if(timelistaptr && timelistaptr.offsetHeight){
                        classlist[item].style.height = (timetable.offsetHeight - 5) + 'px'
                      }else{
                        classlist[item].style.height = (timetable.offsetHeight - 5) + 'px'
                      }
                    }
                  }
                }, 500)
              } catch (e) {

              }
            })
          }
        })
        that.$forceUpdate()
      },
      cancelbooking(id) {
        var that = this
        that.$confirm('Are you sure to cancel?')
          .then(_ => {
            var data = {
              bookingid: id,
              iscancel: true
            }
            that.startLoading()
            request.requestPost('/RestaurantBooking/CancelBooking', data, function(res) {
              that.endLoading()
              if (res.statuscode == 200) {
                that.$message({
                  message: 'Successfully',
                  type: 'success'
                })
                that.getbookinglistbycalendar(false)
              }
            })
          })
          .catch(_ => {});
        return
      },
      changedate(type) {
        var that = this
        var day1 = new Date(that.valuestr.replace(/-/g, "/") + ' 00:00:00');

        if (type == '-') {
          day1.setTime(day1.getTime() - 24 * 60 * 60 * 1000);
        } else {
          day1.setTime(day1.getTime() + 24 * 60 * 60 * 1000);
        }
        var year = day1.getFullYear()
        var month = day1.getMonth() + 1
        if (month.toString().length == 1) {
          month = '0' + month
        }
        var day = day1.getDate()
        if (day.toString().length == 1) {
          day = '0' + day
        }
        var s1 = year + '/' + month + '/' + day;
        that.value = s1
        that.valuestr = s1

        that.transposeid = null

        that.getbookinglistbycalendar(false)
        that.showcalendar = false
      },
      calendarOnClick(e) {
        var that = this
        that.value = e.day.replace(/-/g, "/")
        that.valuestr = e.day.replace(/-/g, "/")
        that.transposeid = null
        that.getbookinglistbycalendar()
        that.showcalendar = false
      },
      gotoday() {
        var that = this
        that.transposeid = null
        var nowDate = new Date();
        var year = nowDate.getFullYear();
        var month = nowDate.getMonth() + 1 < 10 ? "0" + (nowDate.getMonth() + 1) :
          nowDate.getMonth() + 1;
        var day = nowDate.getDate() < 10 ? "0" + nowDate.getDate() : nowDate
          .getDate();
        that.value = (year + "-" + month + "-" + day).replace(/-/g, "/");
        that.valuestr = (year + "-" + month + "-" + day).replace(/-/g, "/");

        // that.getbookinglistbycalendar()

        that.getbookinglistbycalendar()
      },
      createnewbooking() {
        var that = this
        var data = {
          bookingdate: that.valuestr,
          type: 'addnew',
          sectionid: that.selectsectionid
        }
        that.showcalendar = false
        that.scrolltop = document.getElementById("timelistscroll").scrollTop
        that.scrollleft = document.getElementById("timelistscroll").scrollLeft
        that.$router.push('/booking/edit/' + encodeURIComponent('tb_' + JSON.stringify(data)))
      },
      getbookinglistbycalendar(isnotshowloading, issetrightcaledatalist) {
        var that = this

        if(!that.isenablebooking){
          return
        }

        var data = {
          date: that.valuestr,
          showallbooking: that.showallbooking
        }
        if (!isnotshowloading) {
          that.caledatalist = null
          that.timeslotfromdatalist = null
          that.showcalendar = false
          that.startLoading()
        }
        request.requestPost("RestaurantBooking/GetBookingListBycalendar", data, function(res) {
          console.log('获取了')
          that.totalparty = res.data.totalparty
          that.totalpersion = res.data.totalpersion
          that.selfbookingcalendar =  res.data.selfbookingcalendar


          if(that.selfbookingcalendar && that.selfbookingcalendar.length <= 0){
            that.showselfbooking = false
          }else{
            // var audio = new Audio("https://cloud.quickposhub.com/Content/newmessage.mp3");
            // audio.play();
          }
          console.log('res.data.dqlinecalendar',res.data.dqlinecalendar)

          if(res.data.dqlinecalendar){
            for(var i = 0;i < res.data.dqlinecalendar.length;i++){
              res.data.dqlinecalendar[i] = {
                utcbookingtime:res.data.dqlinecalendar[i].utcbookingtime,
                utcturntime:res.data.dqlinecalendar[i].utcturntime,
                iscompleted:res.data.dqlinecalendar[i].iscompleted,
                status:res.data.dqlinecalendar[i].status,
                peoplecount:res.data.dqlinecalendar[i].peoplecount,
              }
            }
          }
          that.dqlinecalendar = res.data.dqlinecalendar

          if (issetrightcaledatalist || (!that.keyword || that.keyword.trim() == '')) {
            console.log('获取了2')
            if (!issetrightcaledatalist) {
              that.caledatalist = res.data.calendar
              that.showcaledatalist = JSON.parse(JSON.stringify(res.data.calendar))
            }
            res.data.sectionshowcalendar.forEach((item,caleindex) =>{
              if(item.tablelist){
                var oldtablelsit = JSON.parse(JSON.stringify(item.tablelist))
                // for(var i = 0;i< oldtablelsit.length;i++){
                //   oldtablelsit[i] = {'Id': oldtablelsit[i].Id,'Tablename':oldtablelsit[i].Tablename,'typename':oldtablelsit[i].typename,'tabletypeid':oldtablelsit[i].tabletypeid, 'timeslot':oldtablelsit[i].timeslot}
                // }
                item.tablelist = oldtablelsit
              }
            })
            // that.tableshowcalendar = res.data.tableshowcalendar

            if (!that.selectsectionid) {
              try{
                that.selectsectionid = res.data.sectionshowcalendar[0].sectionid
              }catch(eaaa){

              }

            }
            console.log('到这里了2')
            res.data.sectionshowcalendar.forEach((item, index) => {
              if (that.bookingsetting.bookingmode == 'profile' || item.sectionid == that.selectsectionid) {
                var oldtimeslotfromdatalist = null
                console.log('进来了这里2')
                try {
                  if (that.timeslotfromdatalist) {
                    oldtimeslotfromdatalist = JSON.parse(JSON.stringify(that.timeslotfromdatalist))
                  }
                } catch (ccc) {

                }


                that.timeslotfromdatalist = item.tablelist
                console.log('that.timeslotfromdatalist',that.timeslotfromdatalist)
                try {
                  if (oldtimeslotfromdatalist) {
                    that.timeslotfromdatalist.forEach((item, index) => {
                      oldtimeslotfromdatalist.forEach((sonitem, sonindex) => {
                        if (item.Id == sonitem.Id) {
                          item.selectthis = sonitem.selectthis
                        }
                      })
                    })
                  }
                } catch (ccc) {

                }


              }
            })
            that.timelist = res.data.timelist
            that.timelistap = res.data.timelistap
            that.sectionshowcalendar = res.data.sectionshowcalendar

            that.autotimelineextension()

            that.gettimelineslot(true)
            that.$nextTick(() => {
              try {

                setTimeout(function() {
                  var timetable = document.getElementById('timetable')
                  var timelistaptr = document.getElementById('timelistaptr')
                  var classlist = document.getElementsByClassName('borderrightposition')
                  for (var item in classlist) {
                    if (classlist[item] && classlist[item].style && classlist[item].style.height) {
                      if(timelistaptr && timelistaptr.offsetHeight){
                        classlist[item].style.height = (timetable.offsetHeight - 5) + 'px'
                      }else{
                        classlist[item].style.height = (timetable.offsetHeight - 5) + 'px'
                      }
                    }
                  }
                }, 500)
              } catch (e) {

              }
            })
            that.endLoading()
          }
        })
      },
      addbookingbytimeslot(tableid, item) {
        var that = this
        var data = {
          tableid: tableid,
          bookingdate: that.valuestr,
          bookingtimeslot: item,
          type: 'addbytimeslot',
          sectionid: that.selectsectionid
        }
        try {
          var nowdate = new Date()
          var selecttimeslot = new Date(that.valuestr.replace(/-/g, "/") + ' ' + item)
          if (selecttimeslot.getTime() >= nowdate.getTime()) {} else {
            that.$message({
              message: 'This time period has passed',
              type: 'error'
            })
            return
          }
        } catch (ex) {

        }
        if(that.transposeid){
          if(that.bookingsetting.transferbookingonlyadjusttable == null || that.bookingsetting.transferbookingonlyadjusttable){
            data.bookingtimeslot = that.transposetimeslot + ''
          }
          if(that.transposetableid){
            data.transposetableid = that.transposetableid + ''
          }
          data.transposeid = that.transposeid + ''
          that.transposeid = null
          that.transposetimeslot = null
          that.transposetableid = null


          that.startLoading()
          request.requestPost('/RestaurantBooking/TransposeBooking', data, function(res) {
            that.endLoading()
            if (res.statuscode == 200) {
              that.$message({
                message: 'Successfully',
                type: 'success'
              })
              that.startLoading()
              that.getbookinglistbycalendar(true)
            }
          })
          return
        }
        that.scrolltop = document.getElementById("timelistscroll").scrollTop
        that.scrollleft = document.getElementById("timelistscroll").scrollLeft
        that.$router.push('/booking/edit/' + encodeURIComponent('tb_' + JSON.stringify(data)))
      },
      searchbooking(e) {
        var that = this
        var data = {
          date: that.valuestr,
          keyword: that.keyword,
          showallbooking: that.showallbooking
        }
        if (data.keyword && data.keyword.trim() != '') {
          // if(that.loadsearch){
          //   return
          // }
          that.loadsearch = true
          request.requestPost("RestaurantBooking/GetBookingListBySearch", data, function(res) {
            that.searchcaledatalist = res.data.calendar
            that.showcaledatalist = JSON.parse(JSON.stringify(that.searchcaledatalist))


            that.searchcaledatalist.forEach((item, index) => {
              that.caledatalist.forEach((caitem, caindex) => {
                if (item.id == caitem.id) {
                  that.caledatalist[caindex] = item
                }
              })
            })
            that.loadsearch = false
          })
        } else {
          that.showcaledatalist = JSON.parse(JSON.stringify(that.caledatalist))
        }
      },
      transposebooking(timeslot,tableitem){
        var that = this
        that.showPop = false
        timeslot.showpopover = false
        that.transposeid = timeslot.bookingdetail.id
        that.transposetimeslot = timeslot.subtimeslot
        if(tableitem){
          that.transposetableid = tableitem.Id
        }else{
          that.transposetableid = null
        }
      },
      editbooking(item) {
        var that = this
        that.transposeid = null
        that.showcalendar = false

        that.scrolltop = document.getElementById("timelistscroll").scrollTop
        that.scrollleft = document.getElementById("timelistscroll").scrollLeft
        that.$router.push('/booking/edit/' + item.id)
      },
      deletebooking(id) {
        var that = this
        that.$confirm('Are you sure to cancel?')
          .then(_ => {
            var data = {
              bookingid: id
            }
            that.startLoading()
            request.requestPost('/RestaurantBooking/CancelBooking', data, function(res) {
              that.endLoading()
              if (res.statuscode == 200) {
                that.$message({
                  message: 'Successfully',
                  type: 'success'
                })
                that.getbookinglistbycalendar(false)
              }
            })
          })
          .catch(_ => {});
        return
      },
      seatbooking(bookingdetail) {
        var that = this
        that.selectbookinginfo = bookingdetail
        that.showbookinginfovisible = true
        that.$forceUpdate()
        // // that.$confirm('Are you sure you want to change it?')
        // //   .then(_ => {
        //     var data = {
        //       bookingid: id
        //     }
        //     that.startLoading()
        //     request.requestPost('/RestaurantBooking/SeatBooking', data, function(res) {
        //       that.endLoading()
        //       if (res.statuscode == 200) {
        //         that.$message({
        //           message: 'Successfully',
        //           type: 'success'
        //         })

        //         if (!that.keyword || that.keyword.trim() == '') {
        //           that.startLoading()
        //           that.getbookinglistbycalendar(true)
        //         } else {
        //           that.getbookinglistbycalendar(true, true)
        //           that.searchbooking()
        //         }
        //       }
        //     })
        //   })
        //   .catch(_ => {});
        // return
      },
      seatconfirmbooking(id){
        var that = this
        var data = {
              bookingid: id
            }
            that.startLoading()
            request.requestPost('/RestaurantBooking/SeatBooking', data, function(res) {
              that.endLoading()

              that.showbookinginfovisible = false

              if (res.statuscode == 200) {
                that.$message({
                  message: 'Successfully',
                  type: 'success'
                })

                if (!that.keyword || that.keyword.trim() == '') {
                  that.startLoading()
                  that.getbookinglistbycalendar(true)
                } else {
                  that.getbookinglistbycalendar(true, true)
                  that.searchbooking()
                }
              }
            })
      },
      confirmbooking(id) {
        var that = this
        that.$confirm('Do you want to mark this booking as read?')
          .then(_ => {
            var data = {
              bookingid: id
            }
            that.startLoading()
            request.requestPost('/RestaurantBooking/ConfirmBooking', data, function(res) {
              that.endLoading()
              if (res.statuscode == 200) {
                that.$message({
                  message: 'Successfully',
                  type: 'success'
                })
                if (!that.keyword || that.keyword.trim() == '') {
                  that.getbookinglistbycalendar(false)
                } else {
                  that.getbookinglistbycalendar(true, true)
                  that.searchbooking()
                }
              }
            })
          })
          .catch(_ => {});
        return
      },
    }
  }
</script>
<style>

  /**
   * 解决el-input设置类型为number时，去掉输入框后面上下箭头
   **/
  #timetable input[type=number]::-webkit-inner-spin-button, #timetable input[type=number]::-webkit-outer-spin-button {
  	 -webkit-appearance: none;
  	 margin: 0;
  }

  #timetable th {
    position: -webkit-sticky;
    position: sticky;
    top: 0;
    background-color: #FFFFFF;
    width: 104px;
  }

  #timetable .el-popover{
    height: 100px !important;
    box-sizing: border-box;padding: 10px;
    z-index: 9999;
  }

  #timetable th div,
  #timetable tr div {
    width: 100%;
    height: 40px;
    font-weight: initial;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  #timetable th,
  #timetable tr {
    border-right: 0.5px solid #dcdcdc;
  }

  #timetable th:nth-last-of-type,
  #timetable tr:nth-last-of-type {
    border-right: 0px
  }

  #timetable tr td {
    text-align: center;
    padding: 0px;
    margin: 0px;
  }


  #timetable tr .timeslotitem:nth-last-of-type {
    border-right: 0px !important;
  }

  @-webkit-keyframes zy{
  	10% {
  	  transform: rotate(40deg);
  	}
  	20% {
  	  transform: rotate(-40deg);
  	}
  	30% {
  	  transform: rotate(25deg);
  	}
  	40% {
  	  transform: rotate(-25deg);
  	}
  	50%,100% {
  	  transform: rotate(0deg);
  	}
  }

  /* 左右晃动动画使用 */
  .contentanimation {
  	animation: zy 1.5s .15s linear infinite;
    -moz-animation: zy 1.5s .15s linear infinite; /* Firefox */
    -webkit-animation: zy 1.5s .15s linear infinite; /* Safari and Chrome */
    -o-animation: zy 1.5s .15s linear infinite; /* Opera */
  }




  /* 闪动动画使用 */
  .fadeanimation {
  	animation: rotatefade 1.5s .15s linear infinite;
    -moz-animation: rotatefade 1.5s .15s linear infinite; /* Firefox */
    -webkit-animation: rotatefade 1.5s .15s linear infinite; /* Safari and Chrome */
    -o-animation: rotatefade 1.5s .15s linear infinite; /* Opera */
  }

  @keyframes rotatefade {
      10% {
        transform: rotate(15deg);
      }
      20% {
        transform: rotate(-15deg);
      }
      30% {
        transform: rotate(15deg);
      }
      40% {
        transform: rotate(-15deg);
      }
      50%,100% {
        transform: rotate(0deg);
      }
  }

  .detailrow {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    padding-top: 10px;
    padding-bottom: 10px;
    align-items: center;
    border-width: 0px 0px 1px 0px;
    border-style: dashed;
    border-color: #CDCDCD;
  }
</style>
<style lang="scss" scoped>
  /deep/ .el-calendar-table {
    width: 100%;
    height: 100%;

    &:not(.is-range) {

      //使不是本月的日期不可点击，不会跳转到其他月份
      td.next {
        pointer-events: none;
      }

      td.prev {
        pointer-events: none;
      }

      //td{
      //    pointer-events: none;
      //}
    }
  }

  /deep/ .el-calendar__body {
    padding: 5px 20px 12px;
  }

  /deep/ .el-calendar .el-calendar__title {
    font-size: 13px;
    text-align: center;
    margin-right: 10px;
  }

  /deep/ .el-calendar .el-button-group .el-button:nth-of-type(2) {
    display: none !important;
  }

  /deep/ .el-calendar .el-calendar__button-group .el-button-group {
    display: flex;
    flex-direction: row;
  }

  /deep/ .el-calendar-table .current {
    text-align: center;
  }

  /deep/ .el-calendar-table .next,
  /deep/ .el-calendar-table .prev {
    text-align: center;
  }

  /deep/ .el-calendar-table thead th {
    text-align: center;
    font-size: 13px;
    padding: 7px 0px !important
  }

  /deep/ .el-calendar-table .el-calendar-day {
    height: 28px;
    font-size: 13px;
    padding: 0px !important;
  }

  /deep/ .el-calendar-table .el-calendar-day p {
    width: 100%;
    height: 100%;
  }

  .directive {
    padding: 20px;


    &-dialog {
      margin: 10px 0;
      display: flex;
      align-items: center;

      .el-input,
      .el-select {
        width: 70%;
        margin-left: 10px;
      }
    }

    &-title {
      width: 100%;
      padding: 12px 20px;
      border-bottom: 1px solid #eef1f5;
      margin-top: 10px;
      margin-bottom: 20px;

      div {
        font-size: 15px;
        line-height: 26px;
        position: relative;

        &::before {
          content: '';
          position: absolute;
          width: 4px;
          height: 14px;
          background: #00aeff;
          top: 6px;
          left: -12px;
        }
      }
    }

    &-add {
      margin: 20px 0;
    }

    .filtrate {
      padding-bottom: 20px;
      display: flex;
      justify-content: space-between;

      &-seek {
        display: flex;

        &>div {
          margin-left: 15px;
        }

        &-input {
          max-width: 200px;
        }
      }
    }
  }
</style>
