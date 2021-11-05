function ConfirmOrder() {
    document.location = 'Cart/OrderConfirmation/';
}

function loadEventInfo(id) {
    $.get("/Cart/LoadEventInfo/"+ id,
        function (data) {
            $('#EventSectionContainer').html(data);
        });
}

function selectedEvent(event) {
    var id = event.value;
    $.get("/Cart/LoadEventInfo/" + id,
        function (data) {
            $('#EventSectionContainer').html(data);
        });
}

function OrderConfirmation() {
    var id = document.getElementById("listbox").value;
    //$.post("/Cart/OrderConfirmation/" + id,
    //    function (data) {
    //    });
    document.location = 'Cart/OrderConfirmation/' + id;
    $("#ConfirmOrderModal").modal("hide");
}
//$('#ConfirmOrderModal').on('load', function () {
//    var e = document.getElementById("listbox");
//    var result = e.options[e.selectedIndex].value;
//    selectedEvent(result);
//});
