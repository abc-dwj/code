import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

/* Layout */
import Layout from '@/layout'

import { get } from 'sortablejs'
import language from '../utils/language.js'

import store from '../store'

import apirequest from '../api/request.js'

import Cookies from 'js-cookie'

/**
 * Note: sub-menu only appear when route children.length >= 1
 * Detail see: https://panjiachen.github.io/vue-element-admin-site/guide/essentials/router-and-nav.html
 *
 * hidden: true                   if set true, item will not show in the sidebar(default is false)
 * alwaysShow: true               if set true, will always show the root menu
 *                                if not set alwaysShow, when item has more than one children route,
 *                                it will becomes nested mode, otherwise not show the root menu
 * redirect: noRedirect           if set noRedirect will no redirect in the breadcrumb
 * name:'router-name'             the name is used by <keep-alive> (must set!!!)
 * meta : {
    roles: ['admin','editor']    control the page roles (you can set multiple roles)
    title: 'title'               the name show in sidebar and breadcrumb (recommend set)
    icon: 'svg-name'             the icon show in the sidebar
    noCache: true                if set true, the page will no be cached(default is false)
    affix: true                  if set true, the tag will affix in the tags-view
    breadcrumb: false            if set false, the item will hidden in breadcrumb(default is true)
    activeMenu: '/example/list'  if set path, the sidebar will highlight the path you set
  }
 */

function getroute() {
    var that = this

    return new Promise((resolve, reject) => {
        var data = { }
        apirequest.requestPost('Hotel/GetRouterList', data, function(res) {
            //这里更新一下可以查看某报表的权限
            try{
              store.commit('updateAuthcode', JSON.parse(res.msg));
            }catch(ex){

            }

            resolve(res.data)
        })
    })
}

/**
 * constantRoutes
 * a base page that does not have permission requirements
 * all roles can be accessed
 */
export let constantRoutes = [{
  path: '/redirect',
  component: Layout,
  hidden: true,
  children: [{
    path: '/redirect/:path(.*)',
    component: () => import('@/views/redirect/index')
  }]
},
{
  path: '/login',
  component: () => import('@/views/login/index'),
  hidden: true
},
{
  path: '/auth-redirect',
  component: () => import('@/views/login/auth-redirect'),
  hidden: true
},
{
  path: '/404',
  component: () => import('@/views/error-page/404'),
  hidden: true
},
{
  path: '/401',
  component: () => import('@/views/error-page/401'),
  hidden: true
},
{
  path: '/',
  component: () => import('@/views/login/index'),
  hidden: true
},
// {
//   path: '/list',
//   component: () => import('@/views/additional/list'),
//   hidden: true
// },
{
    path: '/setting/Admin/editpwd',
    component: () => import('@/views/setting/Admin/editpwd'),
    name:'Change Password',
    hidden:true,
    meta: {
      title: 'Change Password',
      roles: ['admin'] // or you can only set roles in sub nav
    }
},
{
  path: '/message',
  component: Layout,
  hidden: true,
  redirect: '/message/index',
  children: [{
    path: 'index',
    component: () => import('@/views/message/index'),
    name: '站内消息',
    meta: {
      title: '站内消息'
    }
  },
  {
    path: 'mdetail/:id',
    component: () => import('@/views/message/msg-detail'),
    name: '站内消息详情',
    meta: {
      title: '站内消息详情'
    }
  }
  ]
}
]

/**
 * asyncRoutes
 * the routes that need to be dynamically loaded based on user roles
 */
export let asyncRoutes = [
  {
      path: '*',
      redirect: '/404',
      hidden: true
    }
  // {
  //   path: '/dashboard',
  //   component: Layout,
  //   redirect: '/dashboard/index',
  //   children: [{
  //     path: 'index',
  //     component: () => import('@/views/dashboard/index'),
  //     name: language.GetTextByLanguage('仪表盘'),
  //     meta: {
  //       title: language.GetTextByLanguage('仪表盘'),
  //       icon: 'dashboard',
  //       affix: true
  //     }
  //   }]
  // },
  // {
  //    path: '/restaurant',
  //    component: Layout,
  //    redirect: '/restaurant/index',
  //    alwaysShow: true, // will always show the root menu
  //    name: language.GetTextByLanguage('餐厅管理'),
  //    hidden: false,
  //    meta: {
  //      title: language.GetTextByLanguage('餐厅管理'),
  //      roles: ['admin', 'editor'], // you can set roles in root nav
  //      icon: 'example'
  //    },
  //    children: [{
  //      path: 'index',
  //      component: () => import('@/views/restaurant/index'),
  //      name: language.GetTextByLanguage('餐厅列表'),
  //      meta: {
  //        title: language.GetTextByLanguage('餐厅列表'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },
  //    {
  //      path: 'orderlist',
  //      component: () => import('@/views/restaurant/orderlist'),
  //      name: language.GetTextByLanguage('订单列表'),
  //      meta: {
  //        title: language.GetTextByLanguage('订单列表'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },
  //    {
  //      path: 'transactionlist',
  //      component: () => import('@/views/restaurant/transactionlist'),
  //      name: "eMoney/Points Record",
  //      meta: {
  //        title: "eMoney/Points Record",
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },
  //    {
  //      path: 'billorderlist',
  //      component: () => import('@/views/restaurant/billorderlist'),
  //      name: 'ScanPay Record',
  //      meta: {
  //        title: 'ScanPay Record',
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },
  //    {
  //      path: 'menu',
  //      component: () => import('@/views/restaurant/menu'),
  //      name: language.GetTextByLanguage('菜单列表'),
  //      meta: {
  //        title: language.GetTextByLanguage('菜单列表'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      },
  //    },
  //    {
  //      path: 'menu/:data',
  //      component: () => import('@/views/restaurant/menu'),
  //      name: language.GetTextByLanguage('菜单列表'),
  //      meta: {
  //        title: language.GetTextByLanguage('菜单列表'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      },
  //      hidden:true
  //    },{
  //      path: 'table/:data',
  //      component: () => import('@/views/restaurant/table'),
  //      name: language.GetTextByLanguage('座位列表'),
  //      meta: {
  //        title: language.GetTextByLanguage('座位列表'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      },
  //      hidden:true
  //    },
  //    {
  //      path: 'settings',
  //      component: () => import('@/views/restaurant/setting'),
  //      name: language.GetTextByLanguage('设置'),
  //      meta: {
  //        title: language.GetTextByLanguage('设置'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      },
  //    },{
  //      path: 'setting/:data',
  //      component: () => import('@/views/restaurant/setting'),
  //      name: language.GetTextByLanguage('设置'),
  //      meta: {
  //        title: language.GetTextByLanguage('设置'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      },
  //      hidden:true
  //    },{
  //      path: 'salepromotion/:data',
  //      component: () => import('@/views/restaurant/salepromotion/index'),
  //      name: language.GetTextByLanguage('促销推广'),
  //      meta: {
  //        title: language.GetTextByLanguage('促销推广'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      },
  //      hidden:true
  //    },{
  //      path: 'salepromotion/edit/:data',
  //      component: () => import('@/views/restaurant/salepromotion/edit'),
  //      name: language.GetTextByLanguage('编辑促销推广'),
  //      meta: {
  //        title: language.GetTextByLanguage('编辑促销推广'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      },
  //      hidden:true
  //    },{
  //      path: 'salepromotion/selecttype/:data',
  //      component: () => import('@/views/restaurant/salepromotion/selecttype'),
  //      name: language.GetTextByLanguage('选择促销推广类型'),
  //      meta: {
  //        title: language.GetTextByLanguage('选择促销推广类型'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      },
  //      hidden:true
  //    }]
  //  },

  //  {
  //    path: '/user',
  //    component: Layout,
  //    redirect: '/user/index',
  //    alwaysShow: true, // will always show the root menu
  //    name: language.GetTextByLanguage('用户管理'),
  //    hidden: false,
  //    meta: {
  //      title: language.GetTextByLanguage('用户管理'),
  //      icon: 'lock',
  //      roles: ['admin', 'editor'], // you can set roles in root nav
  //      icon: 'member'
  //    },
  //    children: [{
  //      path: 'index',
  //      component: () => import('@/views/user/index'),
  //      name: language.GetTextByLanguage('用户列表'),
  //      meta: {
  //        title: language.GetTextByLanguage('用户列表'),
  //        roles: ['admin'], // or you can only set roles in sub nav
  //        keepAlive:true
  //      },
  //    }, {
  //      path: 'index/:data',
  //      component: () => import('@/views/user/index'),
  //      name: language.GetTextByLanguage('用户列表'),
  //      hidden: true,
  //      meta: {
  //        title: language.GetTextByLanguage('用户列表'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },{
  //      path: 'detail/:data',
  //      component: () => import('@/views/user/detail'),
  //      name: language.GetTextByLanguage('用户详情'),
  //      hidden: true,
  //      meta: {
  //        title: language.GetTextByLanguage('用户详情'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },
  //    {
  //      path: 'carddetail/:data',
  //      component: () => import('@/views/user/carddetail'),
  //      name: language.GetTextByLanguage('卡片详情'),
  //      hidden: true,
  //      meta: {
  //        title: language.GetTextByLanguage('卡片详情'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },
  //    {
  //      path: 'edit/:data',
  //      component: () => import('@/views/user/edit'),
  //      name: language.GetTextByLanguage('编辑用户'),
  //      hidden: true,
  //      meta: {
  //        title: language.GetTextByLanguage('编辑用户'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },
  //    {
  //      path: 'editgiftcard/:data',
  //      component: () => import('@/views/user/editgiftcard'),
  //      name: language.GetTextByLanguage('编辑用户'),
  //      hidden: true,
  //      meta: {
  //        title: language.GetTextByLanguage('编辑用户'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },{
  //      path: 'group',
  //      component: () => import('@/views/user/group'),
  //      name: language.GetTextByLanguage('用户组'),
  //      meta: {
  //        title: language.GetTextByLanguage('用户组'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    },
  //    {
  //      path: 'groupedit/:data',
  //      hidden: true,
  //      component: () => import('@/views/user/groupedit'),
  //      name: language.GetTextByLanguage('用户组'),
  //      meta: {
  //        title: language.GetTextByLanguage('用户组'),
  //        roles: ['admin'] // or you can only set roles in sub nav
  //      }
  //    }] ,
  //  },
  // {
  //   path: '/coupons',
  //   component: Layout,
  //   redirect: '/coupons/index',
  //   alwaysShow: true, // will always show the root menu
  //   name: 'Coupons',
  //   meta: {
  //     title: 'Coupons',
  //     icon: 'lock',
  //     roles: ['admin', 'editor'], // you can set roles in root nav
  //     icon: 'couponnew'
  //   },
  //   children: [{
  //     path: 'index',
  //     component: () => import('@/views/Coupons/index'),
  //     name: 'Coupon List',
  //     meta: {
  //       title: 'Coupon List',
  //       affix: true
  //     },
  //   },{
  //     path: 'userecord',
  //     component: () => import('@/views/Coupons/userecord'),
  //     name: 'Use Record',
  //     meta: {
  //       title: 'Use Record',
  //       affix: true
  //     },
  //   },
  //   {
  //     alwaysShow: true,
  //     path: 'edit/:data',
  //     component: () => import('@/views/Coupons/edit'),
  //     hidden: true,
  //     name: language.GetTextByLanguage('编辑优惠券'),
  //     meta: {
  //       title: language.GetTextByLanguage('编辑优惠券'),
  //       activeMenu: '/marketing/coupons' // or you can only set roles in sub nav，
  //     }
  //   },
  //   {
  //     alwaysShow: true,
  //     path: 'record/:data',
  //     component: () => import('@/views/Coupons/record'),
  //     hidden: true,
  //     name: language.GetTextByLanguage('优惠券发放记录'),
  //     meta: {
  //       title: language.GetTextByLanguage('优惠券发放记录'),
  //       activeMenu: '/marketing/coupons' // or you can only set roles in sub nav，
  //     }
  //   }
  //   ]
  // },
  // {
  //   path: '/reward',
  //   component: Layout,
  //   redirect: '/reward/index',
  //   children: [{
  //     path: 'index',
  //     component: () => import('@/views/reward/index'),
  //     name: language.GetTextByLanguage('奖励'),
  //     meta: {
  //       title: language.GetTextByLanguage('奖励'),
  //       icon: 'star',
  //       affix: true
  //     }
  //   },{
  //     alwaysShow: true,
  //     path: 'edit/:data',
  //     component: () => import('@/views/reward/edit'),
  //     hidden: true,
  //     name: language.GetTextByLanguage('编辑奖励'),
  //     meta: {
  //       title: language.GetTextByLanguage('编辑奖励'),
  //       activeMenu: '/marketing/coupons' // or you can only set roles in sub nav，
  //     }
  //   }]
  // },
  // {
  //   path: '/Campaigns',
  //   component: Layout,
  //   redirect: '/Campaigns/index',
  //   alwaysShow: true, // will always show the root menu
  //   name: language.GetTextByLanguage('战役'),
  //   meta: {
  //     title: language.GetTextByLanguage('战役'),
  //     icon: 'lock',
  //     roles: ['admin', 'editor'], // you can set roles in root nav
  //     icon: 'activity'
  //   },
  //   children: [{
  //     path: 'quick',
  //     component: () => import('@/views/Campaigns/quick'),
  //     name: language.GetTextByLanguage('快速战役'),
  //     hidden: false,
  //     meta: {
  //       title: language.GetTextByLanguage('快速战役'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }, {
  //     path: 'index',
  //     component: () => import('@/views/Campaigns/index'),
  //     name: language.GetTextByLanguage('战役管理'),
  //     meta: {
  //       title: language.GetTextByLanguage('战役管理'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }, {
  //     path: 'edit/:data',
  //     component: () => import('@/views/Campaigns/edit'),
  //     name: language.GetTextByLanguage('编辑战役'),
  //     hidden: true,
  //     meta: {
  //       title: language.GetTextByLanguage('编辑战役'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }, {
  //     path: 'detail/:data',
  //     component: () => import('@/views/Campaigns/detail'),
  //     name: language.GetTextByLanguage('触发器列表'),
  //     hidden: true,
  //     meta: {
  //       title: language.GetTextByLanguage('触发器列表'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }, {
  //     path: 'triggertest/:data',
  //     component: () => import('@/views/Campaigns/triggertest'),
  //     name: language.GetTextByLanguage('触发用户列表'),
  //     hidden: true,
  //     meta: {
  //       title: language.GetTextByLanguage('触发用户列表'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }, {
  //     path: 'trigger/edit/:data',
  //     component: () => import('@/views/Campaigns/trigger/edit'),
  //     name: language.GetTextByLanguage('修改触发器'),
  //     hidden: true,
  //     meta: {
  //       title: language.GetTextByLanguage('修改触发器'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }, {
  //     path: 'quickrecord/:data',
  //     component: () => import('@/views/Campaigns/quickrecord'),
  //     name: 'QuickCampaign Record',
  //     hidden: true,
  //     meta: {
  //       title: 'QuickCampaign Record',
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }]
  // },
  // {
  //   path: '/referralfriends',
  //   component: Layout,
  //   redirect: '/referralfriends/index',
  //   meta: {
  //     title: 'Referral friends',
  //     roles: ['admin', 'editor'], // you can set roles in root nav
  //     icon: 'tree'
  //   },
  //   children: [{
  //     path: 'index',
  //     component: () => import('@/views/referralfriends/index'),
  //     name: language.GetTextByLanguage('推荐记录'),
  //     meta: {
  //       title: language.GetTextByLanguage('推荐记录'),
  //       affix: true
  //     }
  //   },{
  //     path: 'setting',
  //     component: () => import('@/views/referralfriends/setting'),
  //     name: language.GetTextByLanguage('设置'),
  //     meta: {
  //       title: language.GetTextByLanguage('设置'),
  //       affix: true
  //     }
  //   }]
  // },
  // {
  //   path: '/Egiftcard',
  //   component: Layout,
  //   redirect: '/Egiftcard/setting',
  //   alwaysShow: true, // will always show the root menu
  //   name: 'E-GiftCard',
  //   meta: {
  //     title: 'E-GiftCard',
  //     roles: ['admin', 'editor'], // you can set roles in root nav
  //     icon: 'people'
  //   },
  //   children: [{
  //     path: 'transactionlist',
  //     component: () => import('@/views/egiftcard/transactionlist'),
  //     name: 'Transaction Record',
  //     hidden: false,
  //     meta: {
  //       title: 'Transaction Record',
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },{
  //     path: 'setting',
  //     component: () => import('@/views/egiftcard/setting'),
  //     name: 'Setting',
  //     hidden: false,
  //     meta: {
  //       title: 'Setting',
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }]
  // },
  // {
  //   path: '/Luckydraw',
  //   component: Layout,
  //   redirect: '/Luckydraw/setting',
  //   alwaysShow: true, // will always show the root menu
  //   name: 'Luckydraw',
  //   meta: {
  //     title: 'Luckydraw',
  //     roles: ['admin', 'editor'], // you can set roles in root nav
  //     icon: 'luckydraw'
  //   },
  //   children: [{
  //     path: 'record',
  //     component: () => import('@/views/luckydraw/record'),
  //     name: 'Luckydraw Record',
  //     hidden: false,
  //     meta: {
  //       title: 'Luckydraw Record',
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },{
  //     path: 'setting',
  //     component: () => import('@/views/luckydraw/setting'),
  //     name: 'Setting',
  //     hidden: false,
  //     meta: {
  //       title: 'Setting',
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }]
  // },
  // {
  //   path: '/sms',
  //   component: Layout,
  //   redirect: '/sms/index',
  //   alwaysShow: true, // will always show the root menu
  //   name: language.GetTextByLanguage('短信管理'),
  //   hidden: false,
  //   meta: {
  //     title: language.GetTextByLanguage('短信管理'),
  //     roles: ['admin', 'editor'], // you can set roles in root nav
  //     icon: 'guide'
  //   },
  //   children: [{
  //     path: 'index',
  //     component: () => import('@/views/sms/index'),
  //     name: language.GetTextByLanguage('短信发送记录'),
  //     meta: {
  //       title: language.GetTextByLanguage('短信发送记录'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }]
  // },
  // {
  //   path: '/Setting',
  //   component: Layout,
  //   redirect: '/setting/News/index',
  //   alwaysShow: true, // will always show the root menu
  //   name: 'Basic Setting',
  //   hidden: false,
  //   meta: {
  //     title: 'Basic Setting',
  //     icon: 'lock',
  //     roles: ['admin', 'editor'], // you can set roles in root nav
  //   },
  //   children: [
  //     {
  //       path: 'News/index',
  //       component: () => import('@/views/setting/News/index'),
  //       name: 'News and Offers',
  //       meta: {
  //         title: 'News and Offers',
  //         roles: ['admin'] // or you can only set roles in sub nav
  //       }
  //     },
  //     {
  //       path: 'News/editgroup/:data',
  //       component: () => import('@/views/setting/News/editgroup'),
  //       name: 'Edit Group',
  //       hidden:true,
  //       meta: {
  //         title: 'News List',
  //         roles: ['admin'] // or you can only set roles in sub nav
  //       }
  //     },
  //     {
  //       path: 'News/list/:data',
  //       component: () => import('@/views/setting/News/list'),
  //       name: 'News List',
  //       hidden:true,
  //       meta: {
  //         title: 'News List',
  //         roles: ['admin'] // or you can only set roles in sub nav
  //       }
  //     },
  //     {
  //       path: 'News/detail/:data',
  //       component: () => import('@/views/setting/News/detail'),
  //       name: 'Edit Detail',
  //       hidden:true,
  //       meta: {
  //         title: 'Edit Detail',
  //         roles: ['admin'] // or you can only set roles in sub nav
  //       }
  //     },
  //     {
  //       path: 'User/Cardtypeset',
  //       component: () => import('@/views/setting/User/cardtypeset'),
  //       name: language.GetTextByLanguage('会员卡类型'),
  //       meta: {
  //         title: language.GetTextByLanguage('会员卡类型'),
  //         roles: ['admin'] // or you can only set roles in sub nav
  //       }
  //     },
  //     {
  //       path: 'User/Cardtypesetedit/:data',
  //       component: () => import('@/views/setting/User/cardtypesetedit'),
  //       name: language.GetTextByLanguage('编辑会员卡类型'),
  //       hidden : true,
  //       meta: {
  //         title: language.GetTextByLanguage('编辑会员卡类型'),
  //         roles: ['admin'] // or you can only set roles in sub nav
  //       }
  //     },
  //   {
  //     path: 'Reload',
  //     component: () => import('@/views/setting/Reload/index'),
  //     name: language.GetTextByLanguage('充值赠送政策'),
  //     meta: {
  //       title: language.GetTextByLanguage('充值赠送政策'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },
  //   {
  //     path: 'Point',
  //     component: () => import('@/views/setting/Point/index'),
  //     name: language.GetTextByLanguage('积分政策'),
  //     meta: {
  //       title: language.GetTextByLanguage('积分政策'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },{
  //     path: 'SpecialTime',
  //     component: () => import('@/views/setting/SpecialTime/index'),
  //     name: language.GetTextByLanguage('特殊时间段设置'),
  //     meta: {
  //       title: language.GetTextByLanguage('特殊时间段设置'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },
  //   {
  //     path: 'SpecialTime/edit/:data',
  //     component: () => import('@/views/setting/SpecialTime/edit'),
  //     name: language.GetTextByLanguage('特殊时间段编辑'),
  //       hidden:true,
  //     meta: {
  //       title: language.GetTextByLanguage('特殊时间段编辑'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },
  //   {
  //     path: 'Holiday',
  //     component: () => import('@/views/setting/Holiday/index'),
  //     name: language.GetTextByLanguage('节假日设置'),
  //     meta: {
  //       title: language.GetTextByLanguage('节假日设置'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },
  //   {
  //     path: 'Holiday/edit/:data',
  //     component: () => import('@/views/setting/Holiday/edit'),
  //     name: language.GetTextByLanguage('节假日编辑'),
  //     hidden:true,
  //     meta: {
  //       title: language.GetTextByLanguage('节假日编辑'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },
  //   {
  //     path: 'Reload/edit/:data',
  //     component: () => import('@/views/setting/Reload/edit'),
  //     name: language.GetTextByLanguage('编辑充值赠送政策'),
  //     hidden:true,
  //     meta: {
  //       title: language.GetTextByLanguage('编辑充值赠送政策'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },
  //   {
  //     path: 'Point/edit/:data',
  //     component: () => import('@/views/setting/Point/edit'),
  //     name: language.GetTextByLanguage('编辑积分政策'),
  //     hidden:true,
  //     meta: {
  //       title: language.GetTextByLanguage('编辑积分政策'),
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }]
  // },
  // {
  //   path: '/admin',
  //   component: Layout,
  //   redirect: '/admin/index',
  //   alwaysShow: true, // will always show the root menu
  //   name: 'Admin',
  //   meta: {
  //     title: 'Admin Setting',
  //     icon: 'lock',
  //     roles: ['admin', 'editor'], // you can set roles in root nav
  //   },
  //   children: [{
  //     path: '/setting/Admin/index',
  //     component: () => import('@/views/setting/Admin/index'),
  //     name: 'Admin List',
  //     meta: {
  //       title: 'Admin List',
  //       affix: true
  //     },
  //   },
  //   {
  //     path: '/setting/Admin/edit/:data',
  //     component: () => import('@/views/setting/Admin/edit'),
  //     name:'Edit Admin',
  //     hidden:true,
  //     meta: {
  //       title: 'Edit Admin',
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },
  //   {
  //     path: '/setting/Admin/role/index',
  //     component: () => import('@/views/setting/Admin/role/index'),
  //     name:'Role List',
  //     meta: {
  //       title: 'Role List',
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   },
  //   {
  //     path: '/setting/Admin/role/edit/:data',
  //     component: () => import('@/views/setting/Admin/role/edit'),
  //     name:'Edit Role',
  //     hidden:true,
  //     meta: {
  //       title: 'Edit Role',
  //       roles: ['admin'] // or you can only set roles in sub nav
  //     }
  //   }]
  // },
  // {
  //   path: 'couponAnalyze',
  //   component: () => import('@/views/statistical/couponAnalyze'),
  //   meta: {
  //     title: '优惠券分析'
  //   }
  // },
  // {
  //   path: 'groupAnalyze',
  //   component: () => import('@/views/statistical/groupAnalyze'),
  //   meta: {
  //     title: '拼团分析'
  //   }
  // },
  // {
  //   path: 'seckillAnalyze',
  //   component: () => import('@/views/statistical/seckillAnalyze'),
  //   meta: {
  //     title: '秒杀分析'
  //   }
  // },
  // {
  //   path: 'orderStatistics',
  //   component: () => import('@/views/statistical/order-statistics'),
  //   meta: {
  //     title: '订单统计'
  //   }
  // },
  // {
  //   path: 'raisedAnalysis',
  //   component: () => import('@/views/statistical/raised-analysis'),
  //   meta: {
  //     title: '众筹分析'
  //   }
  // }
  // {
  //   path: '/distribution',
  //   component: Layout,
  //   children: [
  //     {
  //       path: 'index',
  //       component: () => import('@/views/distribution/index'),
  //       name: '入驻申请',
  //       meta: { title: '入驻申请', icon: 'dashboard', noCache: true }
  //     }
  //   ]
  // },
  // {
  //   path: '/withdrawal',
  //   component: Layout,
  //   children: [
  //     {
  //       path: 'withdrawal',
  //       component: () => import('@/views/distribution/withdrawal'),
  //       name: '提现申请',
  //       meta: { title: '提现申请', icon: 'dashboard', noCache: true }
  //     }
  //   ]
  // },
  // {
  //   path: '/generalize',
  //   component: Layout,
  //   redirect: '/generalize/generalizeMember',
  //   alwaysShow: true, // will always show the root menu
  //   name: '推广联盟',
  //   meta: {
  //     title: '推广联盟',
  //     icon: 'lock',
  //     roles: ['admin', 'editor'] // you can set roles in root nav
  //   },
  //   children: [{
  //       path: 'posters',
  //       component: () => import('@/views/distribution/posters'),
  //       name: '生成海报',
  //       meta: {
  //         title: '生成海报',
  //         roles: ['admin']
  //       }
  //     },
  //     {
  //       path: 'deposit',
  //       component: () => import('@/views/generalize/deposit'),
  //       name: '联盟会员提现',
  //       meta: {
  //         title: '联盟会员提现',
  //         roles: ['admin']
  //       }
  //     },
  //     {
  //       path: 'Member',
  //       component: () => import('@/views/generalize/generalizeMember'),
  //       name: '会员组',
  //       meta: {
  //         title: '会员组',
  //         roles: ['admin']
  //       }
  //     },
  //     {
  //       path: 'memberEdit/:id',
  //       hidden: true,
  //       component: () => import('@/views/generalize/memberEdit'),
  //       name: '添加会员组',
  //       meta: {
  //         title: '添加会员组',
  //         roles: ['admin']
  //       }
  //     },
  //     {
  //       path: 'generalizeStatistics',
  //       component: () => import('@/views/generalize/generalizeStatistics'),
  //       name: '推广联盟统计',
  //       meta: {
  //         title: '推广联盟统计',
  //         roles: ['admin']
  //       }
  //     },
  //     {
  //       path: 'tgMemberList',
  //       component: () => import('@/views/generalize/tgMemberList'),
  //       name: '联盟会员管理',
  //       meta: {
  //         title: '联盟会员管理',
  //         roles: ['admin']
  //       }
  //     },
  //     {
  //       path: 'memberF',
  //       hidden: true,
  //       component: () => import('@/views/generalize/member'),
  //       name: '联盟会员',
  //       meta: {
  //         title: '联盟会员',
  //         activeMenu: '/generalize/tgMemberList'
  //       }
  //     },
  //     {
  //       path: 'orderManagement',
  //       component: () => import('@/views/generalize/orderManagement'),
  //       name: '联盟订单管理',
  //       meta: {
  //         title: '联盟订单管理',
  //         roles: ['admin']
  //       }
  //     },
  //     {
  //       path: 'orderDetails',
  //       component: () => import('@/views/generalize/orderDetails'),
  //       name: '订单详情',
  //     }
  //   ]
  // },

  // {
  //   path: 'external-link',
  //   component: Layout,
  //   children: [
  //     {
  //       path: 'https://github.com/PanJiaChen/vue-element-admin',
  //       meta: { title: 'External Link', icon: 'link' }
  //     }
  //   ]
  // },
]

// if(addRouter && addRouter.length > 0){
//    asyncRoutes.push({
//        path: '/reports',
//        component: Layout,
//        redirect: '/reports/index',
//        children: [{
//          path: 'index',
//          component: () => import('@/views/Reports/index'),
//          name: language.GetTextByLanguage('报表'),
//          meta: {
//            title: language.GetTextByLanguage('报表'),
//            icon: 'report',
//            affix: true
//          }
//        },
//        {
//          alwaysShow: true,
//          path: 'detail/:data',
//          component: () => import('@/views/Reports/detail'),
//          hidden: true,
//          name: language.GetTextByLanguage('预览报表'),
//          meta: {
//            title: language.GetTextByLanguage('预览报表'),
//            activeMenu: '/marketing/coupons' // or you can only set roles in sub nav，
//          }
//        }
//        ]
//      })
//    })
// }



const createRouter = () => new Router({
  // mode: 'history', // require service support
  scrollBehavior: () => ({
    y: 0
  }),
  routes: constantRoutes
})

const router = createRouter()
function routerCom(path) { //对路由的component解析
	return (resolve) => require([`@/views/${path}`], resolve);
}

function routerChildren(children) { //对子路由的component解析
	children.forEach(v => {
		v.component = routerCom(v.component);
		if (v.children != undefined) {
			v.children = routerChildren(v.children)
		}
	})
	return children
}

router.beforeEach((to, from, next) => {
  let token = Cookies.get('loginkey')
  if (!token || to.path === '/login') {
    console.log('router：login')
    next();
  } else {
    getroute().then((res) =>{
      if(!res){
        next();
        return
      }
      clearRouter()
      pushRouter(res)
      //下面这些是固定的router数据，后面添加页面可以对照调试
      // asyncRoutes.push({
      //     path: '/reports',
      //     component: Layout,
      //     redirect: '/reports/index',
      //     children: [{
      //       path: 'index',
      //       component: () => import('@/views/Reports/index'),
      //       name: language.GetTextByLanguage('报表'),
      //       meta: {
      //         title: language.GetTextByLanguage('报表'),
      //         icon: 'report',
      //         affix: true
      //       }
      //     },
      //     {
      //       alwaysShow: true,
      //       path: 'detail/:data',
      //       component: () => import('@/views/Reports/detail'),
      //       hidden: true,
      //       name: language.GetTextByLanguage('预览报表'),
      //       meta: {
      //         title: language.GetTextByLanguage('预览报表'),
      //         activeMenu: '/marketing/coupons' // or you can only set roles in sub nav，
      //       }
      //     }
      //     ]
      //   })

      next();
    })
  }
});

export function pushRouter(res){
  var addRoute = []
    res.forEach((item,index)=>{
      var routeitem = {
        // alwaysShow:item.alwaysShow,
        path:item.path,
        component:Layout,
        redirect:item.redirect
      }
      if(item.affix != null){
        routeitem.affix = item.affix
      }
      if(item.alwaysShow != null){
        routeitem.alwaysShow = item.alwaysShow
      }
      if(item.meta){
        routeitem.meta = {
           title:item.meta.title,
           icon:item.meta.icon,
        }
        if(item.meta.affix != null){
          routeitem.meta.affix = item.meta.affix
        }
      }

      var children = []
      item.children.forEach((sonitem,sonindex)=>{
         var childrenobj = {
           path:sonitem.path,
           component: (resolve) => require([`@/views/${sonitem.component}`], resolve),
           name:sonitem.name,
           meta:{
             title:sonitem.meta.title,
             icon:sonitem.meta.icon,
           }
         }

         if(sonitem.hidden){
           childrenobj.hidden = true
         }
         if(sonitem.alwaysShow != null){
           childrenobj.alwaysShow = sonitem.alwaysShow
         }

         if(sonitem.meta.affix != null){
           childrenobj.meta.affix = sonitem.meta.affix
         }

         children.push(childrenobj)
      })
      routeitem.children = children
      asyncRoutes.push(routeitem)
    })
  constantRoutes = constantRoutes.concat(addRoute)
  createRouter()
}

export function clearRouter(){
  if(asyncRoutes){
    asyncRoutes.forEach((asyncitem,asyncindex)=>{
      let aaindex = router.options.routes.findIndex((r) => r === asyncitem)
      if (aaindex !== -1) {
        router.options.routes.splice(aaindex, 1)
      }
    })
    asyncRoutes = [{
      path: '*',
      redirect: '/404',
      hidden: true
    }]
  }
}


// Detail see: https://github.com/vuejs/vue-router/issues/1234#issuecomment-357941465
export function resetRouter() {
  const newRouter = createRouter()
  router.matcher = newRouter.matcher // reset router
}

router.pushRouter = function(param){
  clearRouter()
  pushRouter(param)
}

export default router
