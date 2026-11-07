<template>

    <el-container>
        <el-aside width="300px">
            <div  class="search-container">
                <el-input v-model="q" clearable placeholder="输入关键字搜索地址" prefix-icon="el-icon-search"/>

            </div>
            <div class="result">
                <div v-for="(addr,index) in addresses" :key="index" class="address-item" @click="callback(addr)">
                    <div class="title"><template v-if="addr.isCurrent">(当前位置)</template>{{addr.title}}</div>
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
    import AMapLoader from "@amap/amap-jsapi-loader";
    import emitter from '@/mixins/emitter';
    export default {
        mixins: [emitter],
        data() {
            return {
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

            callback(addr) {
                delete addr.isCurrent;
                this.$closeDialog(addr)
            },
            initMap(point) {


                window._AMapSecurityConfig = {
                    securityJsCode: "1d5654635e1743bcf26d883433c1927c",
                };
                var self = this;
                AMapLoader.load({
                    key: "17ecc291f8f2ae3be003507d8f524542",
                    version: "2.0",
                    plugins: ['AMap.Scale', 'AMap.PlaceSearch', 'AMap.ToolBar', 'AMap.Geocoder'],
                })
                    .then((AMap) => {
                        map = new AMap.Map("map", {

                            zoom: 15,
                            center: point,
                        });

                        AMap.plugin([
                            'AMap.ToolBar',
                        ], function () {
                            map.addControl(new AMap.ToolBar());
                        });

                        map.on('dragend', function () {
                            self.addresses = [];
                            var point = map.getCenter();
                            self.getCurrentAddress(point);
                            self.searchNearby(point);

                        });

                    })
                    .catch((e) => {
                        console.log(e);
                    });
            },

            getCurrentAddress(point) {

                var self = this;
                AMap.plugin("AMap.Geocoder", function () {
                    var geocoder = new AMap.Geocoder({

                    });

                    var lnglat = [point.lng, point.lat];
                    geocoder.getAddress(lnglat, function (status, result) {
                        if (status === "complete" && result.info === "OK") {
                            var regeocode = result.regeocode;
                            var addCmpt = regeocode.addressComponent;
                            var title = addCmpt.street;
                            var address = { isCurrent: true, title: title, address: regeocode.formattedAddress, lng: lnglat[0], lat: lnglat[1] }
                            self.addresses.splice(0, 0, address)

                        }
                    });
                });

            },
            searchNearby(point) {

                var self = this;
                AMap.plugin(["AMap.PlaceSearch"], function () {

                    var placeSearch = new AMap.PlaceSearch({
                        pageSize: 10,
                        pageIndex: 1,

                    });
                    placeSearch.searchNearBy('', point, 200, function (status, result) {
                        if (status == 'complete' && result.poiList) {
                            for (var poi of result.poiList.pois) {
                                var address = { lng: poi.location.lng, lat: poi.location.lat, title: poi.name, address: poi.address };
                                self.addresses.push(address)
                            }

                        }
                    });
                });

            },

            search(keywords) {

                this.addresses = [];
                var self = this;
                if (!keywords) return;

                AMap.plugin('AMap.PlaceSearch', function () {
                    var autoOptions = {
                        pageSize: 10,
                        pageIndex: 1,
                    }
                    var placeSearch = new AMap.PlaceSearch(autoOptions);
                    placeSearch.search(keywords, function (status, result) {
                        if (status == 'complete' && result.poiList) {
                            var list = [];
                            for (var poi of result.poiList.pois) {
                                var address = { lng: poi.location.lng, lat: poi.location.lat, title: poi.name, address: poi.address };
                                list.push(address)
                            }
                            self.addresses = list;
                        }
                    })
                })

            }
        },

        async mounted() {

            var location = '';
            var point = this.$route.query.point;
            if (point) {
                point = JSON.parse(point);
                if (point.lng)
                    location =[point.lng, point.lat];
            }
            this.initMap(location);

        },
        computed: {


        },


    }


</script>

<style scoped>
    #map {
        height: 100%;
        width: 100%;
        position: relative;
    }

    .search-container {
        padding: 10px;
    }
    .markMapImg {
        left: 50%;
        position: absolute;
        top: 50%;
        transform: translateX(-50%) translateY(-100%);
        z-index: 10000;
    }

    .el-main {
        padding: 0px;
    }

    .el-aside {
        display: flex;
        flex-direction: column;
        height: 600px;
        border: 1px solid #EBEEF5;
    }

    .result {
        flex: 1;
        overflow-y: auto;
    }

    .address-item {
        font-size:13px;
        padding: 10px;
        cursor: pointer;
        border-bottom: solid 1px #EBEEF5;
    }

        .address-item:hover {
            background-color: #EBEEF5;
        }
    .address {
        color: #9e9e9e;
    }

    .title {
        font-weight: bold;
    }

    /deep/ .el-dialog__body {
        padding: 10px;
    }

    /deep/ .el-input__inner {

        border-radius:20px;
    }


</style>
