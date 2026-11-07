
<template>
  <div id="app">
    <router-view></router-view>
  </div>
</template>

<script>

  export default {
    name: 'app'
  }

</script>

<style>
  body {
    padding: 0;
    margin: 0;
    font-size: 12px;
  }

  a {
    text-decoration: none;
  }

  #app {
    font-family: 'Microsoft YaHei', 'Avenir', Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    color: #2c3e50;
  }

  body, #app {
    height: 100%;
  }
</style>
