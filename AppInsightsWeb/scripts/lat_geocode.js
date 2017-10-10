function Geocode() {

    var address = {};
    address["address1"] = Xrm.Page.getAttribute("lat_address1").getValue();
    address["city"] = Xrm.Page.getAttribute("lat_city").getValue();
    address["state"] = Xrm.Page.getAttribute("lat_state").getValue();
    address["zip"] = Xrm.Page.getAttribute("lat_zip").getValue();


    var req = new XMLHttpRequest();
    req.open("POST", "https://{Your Value}.azurewebsites.net/api/Geocode?code={Your Value}", true);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json;charset=utf-8");
    req.onreadystatechange = function () {
        if (req.readyState === 4) {
            if (req.status === 200) {
                var retrieved = JSON.parse(req.responseText);
                Xrm.Page.getAttribute("lat_latitude").setValue(retrieved.Latitude);
                Xrm.Page.getAttribute("lat_longitude").setValue(retrieved.Longitude);

                Xrm.Page.getAttribute("lat_geocode").setValue(false);

            }
        }
    };
    req.send(JSON.stringify(address));

}