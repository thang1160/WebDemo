self.addEventListener('push', function (e) {
    //document.getElementById("lead").innerHTML = e;
    var message = e.data.json()
    //document.getElementById("lead").innerHTML = JSON.stringify(message);
    //var option = {
    //    body: message["body"],
    //    vibrate: [100, 50, 100],
    //    data: {
    //        dateOfArrival: Date.now(),
    //        primaryKey: '2'
    //    },
    //    icon: "/image/abc.jpg",
    //    actions: [
    //        {
    //            action: 'explore', title: 'Xem ngay'
    //        },
    //        {
    //            action: 'close', title: 'Đóng'
    //        }
    //    ]
    //}

    var option = {
        body: message["body"],
        icon: "/image/abc.jpg",
        vibrate: [200, 100, 200, 100, 200, 100, 400],
        tag: "request",
        actions: [
            { "action": "yes", "title": "Yes" },
            { "action": "no", "title": "No" }
        ]
    }

    e.waitUntil(
        self.registration.showNotification(message["title"], option)
    )
});
