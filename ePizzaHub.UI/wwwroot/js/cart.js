function AddToCart(ItemId, Name, UnitPrice, Quantity) {
    
    $.ajax({
        type: "GET",
        url: "/Cart/AddToCart/" + ItemId + "/" + UnitPrice + "/" + Quantity,
        success: function (res) {
            if (res.status == "success") {
                
                $("#cartCounter").text(res.count);
             
                ToastSuccess('Item Added to Cart Successfully');
                location.reload();
            }
        }
    });
}


$(document).ready(function () {
    
    GetCartData();
    RefereshCartQuantity();
    SwalFire();
});

function RefereshCartQuantity() {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: '/Cart/GetCartCount',
        success: function (data) {
            $("#cartCounter").text(data);
        },
        error: function (result) {
        },
    });
}

function SwalFire() {
    $(document).on('click', '.btn-delete', function (event) {
        event.preventDefault();
        const itemId = $(this).data('id'); // Use jQuery's .data() method
        
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                deleteItem(itemId); // Your delete function
            }
        });
    });
}

function updateQuantity(ItemId, currentQuantity, quantity) {
    
    if ((currentQuantity >= 1 && quantity == 1) || (currentQuantity > 1 && quantity == -1)) {
        $.ajax({
            url: 'Cart/UpdateQuantity/' + ItemId + "/" + quantity,
            type: "GET",
            success: function (response) {
                if (response > 0) {
                    let pathname = window.location.pathname;
                    if (pathname == '/Cart') {
                        GetCartData();
                    }
                    else {
                        window.location.reload();
                    }

                   
                }
            }
        });
    }
}



function deleteItem(ItemId) {

    if (ItemId > 0) {
     /*   if (confirm("Are you sure! You want to delete this item? ")) {*/
            $.ajax({
                type: "GET",
                url: '/Cart/DeleteItem/' + ItemId,
                success: function (response) {
                    if (response > 0) {
                        GetCartData();
                        RefereshCartQuantity();
                        ToastSuccess('Item removed from cart successfully.')
                    }
                }
            });
        /*}*/
    }
}


function GetCartData() {

    $.ajax({
        url: '/Cart/GetCartDetails',
        type: 'get',
        success: function (response) {
            
            let id = response.id;
            let userId = response.userId;
            let total = response.total;
            let tax = response.tax;
            let grandTotal = response.grandTotal;
            let createdDate = new Date(response.createdDate);
            let items = response.items;

            $('#total').text(total);
            $('#tax').text(tax);
            $('#grandTotal').text(grandTotal);
            $('#cartItems').text(items.length);

            // $('#cart-table').html('');

            let tableBody = $('#cart-table tbody');
            tableBody.empty();
            tableBody.html('');

            $.each(items, function (i, item) {
                let row = `
                                         <tr>
                                            <td>
                                                <img src="${item.imageUrl}" style="height:70px;" width="180" />
                                                <div>${item.name}</div>
                                            </td>
                                            <td>${item.unitPrice}</td>
                                            <td>
                                                <div class="def-number-input number-input safari_only mb-0 w-100">
                                                    <div class="input-group mb-3" style="width: 120px;margin: auto;">
                                                        <div class="input-group-prepend">
                                                            <button onclick="updateQuantity('${item.itemId}','${item.quantity}',-1)" class="btn btn-outline-secondary" type="button">-</button>
                                                        </div>
                                                        <input class="form-control sm-control text-center" size="2" min="0" name="quantity" value="${item.quantity}" readonly />
                                                        <div class="input-group-prepend">
                                                            <button onclick="updateQuantity('${item.itemId}','${item.quantity}',1)" class="btn btn-outline-secondary" type="button">+</button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                            <td>${item.total}</td>
                                            <td>

                                                <!--Remove Button-->
                                                <a href="javascript:void(0);" data-id="${item.itemId}" class="btn-delete"><i class="bi bi-trash text-danger"></i></a>
                                                <!--     <button onclick="deleteItem('${item.itemId}')" class=""><i class="bi bi-trash text-danger"></i></button> -->
                                            </td>
                                        </tr>
                            `;




                tableBody.append(row);
            });


        }
    })
}

function ToastSuccess(msg) {
    
    toastr.success(msg, 'Success', {
        ProgressEvent: true,
        positionClass: 'toast-top-right',
        timeOut: 3000
    });
}

