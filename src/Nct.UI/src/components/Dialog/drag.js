export default {
    bind(el, binding, vnode, oldVnode) {


        let resizeEvent = new CustomEvent('resize', {
            detail: '尺寸变化',
            bubbles: true
        });

        el.style.overflow = 'initial'

        let isFullScreen = false;

        el.className += ' nct-dialog'

    

        let elDialog = el.querySelector('.el-dialog');
        let elHeader = el.querySelector('.el-dialog__header');

        let headerVisibleAttr = el.getAttribute("headerVisible") || 'Y';
        let headerVisible = headerVisibleAttr === 'Y';

        let resizeAttr = el.getAttribute("resizeable") || 'Y';
        let resizeable = resizeAttr === 'Y' && headerVisible;

        let height = el.getAttribute("height");
        elDialog.style.height = height;

    


        if (!headerVisible) {

            elHeader.style.display = 'none';
            let elClose = document.createElement("div");

            let iconHtml = `<i class="el-icon-error" style="font-size:20px;color:#999" onMouseOver="this.style.color='#409EFF'" onMouseOut="this.style.color='#999'"></i>`
            elClose.innerHTML = iconHtml;
            elClose.style.cursor = 'pointer';
            elClose.style.position = 'absolute';
            elClose.style.right = '10px';
            elClose.style.top = '10px';
            elClose.style.zIndex = 100;

            elClose.onclick = function (e) {
                elHeader.childNodes[1].click(e);

            };

            elDialog.appendChild(elClose);

        }


        let bakProps = {};
     
        if (resizeable) {
            let maxBtn = document.createElement('button');
            maxBtn.className = 'el-dialog__headerbtn';
            maxBtn.style.right = '45px';
            maxBtn.style.color = '#909399';

            let iconHtml = `<i class="el-icon-full-screen" onMouseOver="this.style.color='#409EFF'" onMouseOut="this.style.color='inherit'"></i>`
            maxBtn.innerHTML = iconHtml;

            elHeader.insertBefore(maxBtn, elHeader.childNodes[1]);

            let elResize = document.createElement("div");
            //elResize.innerHTML = `<i class="el-icon-full-screen"></i>`
            elResize.style.cursor = 'se-resize';
            elResize.style.position = 'absolute';
            elResize.style.height = '10px';
            elResize.style.width = '10px';
            elResize.style.right = 0;
            elResize.style.bottom = 0;
            elResize.style.zIndex = 100;
            elDialog.appendChild(elResize);
            maxBtn.onclick = function () {

                if (isFullScreen) {

                    elDialog.style.top = bakProps.top;
                    elDialog.style.left = bakProps.left;
                    elDialog.style.width = bakProps.width;
                    elDialog.style.height = bakProps.height;
                    elDialog.style.marginTop = bakProps.marginTop;

                } else {

                    bakProps.width = elDialog.style.width;
                    bakProps.height = elDialog.style.height;
                    bakProps.left = elDialog.style.left;
                    bakProps.top = elDialog.style.top;
                    bakProps.marginTop = elDialog.style.marginTop;

                    elDialog.style.top = 0;
                    elDialog.style.left = 0;
                    elDialog.style.marginTop = 0;
                    elDialog.style.width = '100VW';
                    elDialog.style.height = '100vh';


                }
                isFullScreen = !isFullScreen;
                setMaxBtnTitleAndIcon();
                el.dispatchEvent(resizeEvent);

            }

            elResize.onmousedown = function (e) {

                const x = e.clientX;
                const y = e.clientY;
                const width = elDialog.offsetWidth
                const height = elDialog.offsetHeight;
                const winHeigth = window.innerHeight;
                const winWidth = window.innerWidth;

                const body = document.querySelector("body");
                body.style.cursor = 'se-resize';
                const moveFun = function (e) {
                    e.preventDefault();
                    let disX = e.clientX - x;
                    let disY = e.clientY - y;

                    let realWidth = width + disX * 2;
                    if (realWidth > winWidth)
                        realWidth = winWidth;

                    let realHeight = height + disY;
                    if (realHeight > winHeigth)
                        realHeight = winHeigth;

                    elDialog.style.height = realHeight + 'px';
                    elDialog.style.width = realWidth + 'px';

                    el.dispatchEvent(resizeEvent);

                }

                const stopFun = function (e) {

                    document.removeEventListener('mousemove', moveFun);
                    document.removeEventListener('mouseup', stopFun);
                    body.style.cursor = 'initial';

                }

                document.addEventListener("mousemove", moveFun);
                document.addEventListener("mouseup", stopFun);

            }

            function setMaxBtnTitleAndIcon() {

                maxBtn.title = isFullScreen ? '还原' : '最大化';
                elHeader.style.cursor = isFullScreen ? 'initial' : 'move';

                let elIcon = maxBtn.querySelector('i');
                elIcon.className = isFullScreen ? 'el-icon-crop' : 'el-icon-full-screen';

            }

            setMaxBtnTitleAndIcon();


        }
       
        elHeader.onmousedown = function (e) {
            e.preventDefault();
            const disX = e.clientX - elHeader.offsetLeft;
            const disY = e.clientY - elHeader.offsetTop;

            let top = elDialog.style.top;
            let left = elDialog.style.left;
            if (top) {

                top = parseInt(top.replace('px', ''));
            } else {
                top = 0;
            }
            if (left) {

                left = parseInt(left.replace('px', ''));
            } else {
                left = 0;
            }

            const winHeigth = window.innerHeight;
            const winWidth = window.innerWidth;
            const width = elDialog.offsetWidth
            const height = elDialog.offsetHeight;
            const offsetLeft = elDialog.offsetLeft;
            const offsetTop = elDialog.offsetTop;

            const moveFun = function (e) {

                let x = e.clientX - disX;
                let y = e.clientY - disY;

                if (x < 0) {

                    let diff = -x - offsetLeft;
                    if (diff > 0)
                        x += diff;

                } else {
                    let diff = x - (winWidth - offsetLeft - width);
                    if (diff > 0)
                        x -= diff;
                }

                if (y < 0) {

                    let diff = -y - offsetTop;
                    if (diff > 0)
                        y += diff;

                } else {

                    let diff = y - (winHeigth - offsetTop - height);
                    if (diff > 0)
                        y -= diff;

                }

                x += left;
                y += top;


                elDialog.style.top = y + 'px';
                elDialog.style.left = x + 'px';


            }

            const stopFun = function (e) {

                document.removeEventListener('mousemove', moveFun);
                document.removeEventListener('mouseup', stopFun);

            }

            document.addEventListener("mousemove", moveFun);
            document.addEventListener("mouseup", stopFun);



        }






    }
}