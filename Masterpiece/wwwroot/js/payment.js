function nextTab(tabIndex) {
    var myTab = new bootstrap.Tab(document.querySelector(`#tab${tabIndex + 1}-tab`));
    myTab.show();
}