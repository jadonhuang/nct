<template>

    <el-container>
        <el-aside width="250px">
            <el-input v-model="q" clearable placeholder="输入关键字搜索地址" prefix-icon="el-icon-search">
               
            </el-input>

            <div class="result">
                <div v-for="(addr,index) in addresses" :key="index" class="address-item" @dblclick="$closeDialog(addr)" @click="moveToAddress(addr)">
                    <div class="title">{{addr.title}}</div>
                    <div class="address">{{addr.address}}</div>
                </div>

            </div>
        </el-aside>
        <el-main style="position:relative">
            <div id="map">

            </div>
            <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABMAAAAfCAYAAAAFkva3AAADOklEQVR42qWUWU8TcRTFxxiNj8YXEx+BUmjZKS1Ll2k70720oH4TeXWJn8Dod/BRozHGCARNlMTEQIzGBAFjFChrgcJAKeV4z2CakLK08vDLzb33nPOfJTMKgDJy/ckbwpAwJswJ+X/1HefcH+c70mwkE5eEh4Kxm0yjkLyJYvIWDhK3Wc2ec+6po/7YsGwiflUYMRIpFOMSEJeA6CCKoZSQZGXPubmnTvSjwrUjYWvx6EVh2IgfBu1HB7AfjKGghlHw6SXYc849ddSL76NwuRS2Eovez8X6sR+TW5Er2BPTnlfHnkcrh3Meoh/qt8RHvxm2FIlcF4w9Oa0QTiPvCyHvDp4FdXJwGvTRzxwlEw7f2YgkUIgMyolR5Hs1IVgBGvWmj37JGVLmQ9rotp5CPpjGTm8IO93BiqGePvqZo/zRtMUdLY1dNQFDBIYrUDHU00e/5GSUX0F/cTc4gB1PHNvOQNXQRz9zlJ9+NWcEUjA8MWx1BQR/FQRMH/2Ss6lM+7wTG74kjL4Eco4gcp3+iqGePvolZ1KZ8noeLXojMLwp5Lo0bHaoFUM9fcvil5zHynd3X/eMW8WWO4VNVxTrHX6st6tnQh319NHPHPML+NrbMzbXqyPX04/1Tg3ZFlXwnYJKnamnj/7S5/Slp9sq7Ga75f6dMWTb/Fhr9p0I99RRTx/9pTAy4XI+mHXKs3AmkG3Vsdrkw6rdWwbn3FNHPX1lv6DPXY4rwuyCQ8e6I4aVJhXLNk8ZnHNPHfX0lYWRT47O9LfOPmTb41ht1rHU6MVSg6cEe865p476E/+0ZLyjfXhWXvtaW0yuRMWi1V2CPefci26E+lPDPrS1tgjFtVbeqi4hHmQsblaz55x76s4MI+9bW55ONXvFHEXGqmK+to/V7DnnnrqKwsaam2zCwbI9KlekSZjHrOw5577iMDJitz//YfNhyRbGgsVvVvacc19V2FtbozZua5e3GEGmXjcre86rDnvT2HBBmPlt0bBoiYCVPedVh5HXVuvdCYsLmboQWKW/x/l/hb2qr28YsdixUBsCK/v/DiMvLXXTszV+sLI/V9iLutonkzUusJ477FltjSZsCPpZ2r+QQ8neDXOh7wAAAABJRU5ErkJggg==" class="markMapImg">
        </el-main>
    </el-container>

</template>
<script>

    var map;

    import emitter from '@/mixins/emitter';
    export default {
        mixins: [emitter],
        data() {
            return {
                point: null,
                q: '',
                addresses: [],

            }
        },
        watch: {
            q(val) {
                this.search(val);
            }
         
        },

        methods: {

            initMap: function () {

                 map = new BMapGL.Map("map");
                
                var loc = this.point;
                if (loc && loc.lat) {

                    let point = new BMapGL.Point(loc.lng, loc.lat);
                    map.centerAndZoom(point, 12);
                 

                } else {
                    var localcity = new BMapGL.LocalCity();
                    localcity.get(e => {
                        let name = e.name || '北京';
                        map.centerAndZoom(name, 12);

                    })



                }
                map.enableScrollWheelZoom(true);
                var zoomCtrl = new BMapGL.ZoomControl();  // 添加缩放控件
                map.addControl(zoomCtrl);


             
                var self = this;
                map.addEventListener("dragend", function (pos) {
                    console.log('post', pos.latlng);
                    var point = map.getCenter();
                    self.searchNearby(point);
                });


            },
            moveToAddress(address) {
                let point = new BMapGL.Point(address.lng, address.lat);
                map.centerAndZoom(point, 12);
                var marker = new BMapGL.Marker(point);
                map.addOverlay(marker);

            },
            searchNearby(point) {

                var self = this;
                self.addresses = [];   

                var options = {
                    onSearchComplete: function (results) {
                        console.log('results', results);
                        if (local.getStatus() == BMAP_STATUS_SUCCESS) {
                 
                            var list = [];
                            for (var result of results) {
                                for (var i = 0; i < result.getCurrentNumPois(); i++) {
                                    let pos = result.getPoi(i);
                                    list.push({ title: pos.title, address: pos.address, lng: pos.point.lng, lat: pos.point.lat });
                                    if (i > 3) break;
                                }
                            }
                            self.addresses = list;

                        }
                    }
                };
                var local = new BMapGL.LocalSearch(map, options);
                local.searchNearby('', point,1000);
             
            },

            search(key) {
                var self = this;
                self.addresses = [];

                var options = {
                    onSearchComplete: function (results) {
                        console.log('results', results);
                        if (local.getStatus() == BMAP_STATUS_SUCCESS) {

                            var list = [];
                         
                            for (var i = 0; i < results.getCurrentNumPois(); i++) {
                                let pos = results.getPoi(i);
                                    list.push({ title: pos.title, address: pos.address, lng: pos.point.lng, lat: pos.point.lat });
                                   
                                }
                            
                            self.addresses = list;

                        }
                    }
                };
                var local = new BMapGL.LocalSearch(map, options);
                local.search(key);

            }
        },

        async mounted() {

            this.point = JSON.parse(this.$route.query.point || '{}');
            this.initMap();



        },
        computed: {


        },


    }


</script>

<style scoped>
    #map {
        height: 100%;
        width: 100%;
        position:relative;
    }
    .markMapImg {
        left: 50%;
        position: absolute;
        top: 50%;
       z-index:10000;
    }

    .el-main {
        padding: 0px;
    }

    .el-aside {
        display: flex;
        flex-direction: column;
        height: 500px;
        border: 1px solid #EBEEF5;
        padding: 5px;
    }

    .result {
        flex: 1;
        overflow-y: auto;
    }

    .address-item {
        padding: 10px;
        cursor: pointer;
        border-bottom:solid 1px #EBEEF5;
    }

        .address-item:hover {
            background-color: #EBEEF5;
        }

        .title {
            font-weight: bold;
        }

    /deep/ .el-dialog__body {
        padding: 10px;
    }
</style>
