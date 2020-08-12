const groupEl = document.getElementById('group');
var stateLabel = document.getElementById("stateLabel");
var messagesLog = document.getElementById("messageLog");
var socket;

var scheme = document.location.protocol === "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";

var connectionUrl = scheme + "://" + document.location.hostname + port + "/2ndclient" ;

function updateState() {
    
    switch (socket.readyState) {
        case WebSocket.CLOSED:
            stateLabel.innerHTML = "Соединение закрыто";
            Status("alert-danger");
            break;
        case WebSocket.CLOSING:
            stateLabel.innerHTML = "Закрываем соединение...";
            Status("alert-secondary");
            disable();
            break;
        case WebSocket.CONNECTING:
            stateLabel.innerHTML = "Подключаемся...";
            Status("alert-primary");
            disable();
            break;
        case WebSocket.OPEN:
            stateLabel.innerHTML = "Подключено";
            Status("alert-success");
            enable();
            break;
        default:
            stateLabel.innerHTML = "Неизвестно что происходит: " + htmlEscape(socket.readyState);
            Status("alert-warning");
            disable();
            break;
    }
    
}

function Status(val){
    stateLabel.classList.forEach(item => {
        if (item !== "alert") {
            stateLabel.classList.remove(item);
        }
    });
    stateLabel.classList.add(val);
}

document.addEventListener("DOMContentLoaded", () => {
    stateLabel.innerHTML = "Подключаемся...";
    Status("alert-primary");
    socket = new WebSocket(connectionUrl);
    socket.onopen = function (event) {
        updateState();
    };
    socket.onclose = function (event) {
        updateState();
    };
    socket.onerror = updateState;
    socket.onmessage = function (event) {
        var res =JSON.parse(event.data);
        const cardEl = document.createElement('div');
        cardEl.classList.add('card');
        cardEl.classList.add('my-1');
        cardEl.innerHTML = `
        <div class="card-header">
            <h6>ID${res.Id}</h6>
            <h5>${new Date(res.SendTime).toLocaleString()}</h5>
            </div>
            <div class="card-body row mx-1">
                ${res.Text}
        </div>`;
        groupEl.append(cardEl);
    };
});

window.onunload = function(){
    socket.close(1000, "Client disconected");
}

function htmlEscape(str) {
    return str.toString()
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}