function displayError(xhr) {
    var msg = JSON.parse(xhr.responseText);
    alert(msg.message);
}