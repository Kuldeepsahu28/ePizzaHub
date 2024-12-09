function AddToCart(btn, ItemId, Name, UnitPrice, Quantity) {
    debugger
    $(".loader").css("visibility", "visible");
    let btnId = $(btn).attr("id");
    console.log(btnId);
    $.ajax({
        type: "GET",
        url: "/Cart/AddToCart/" + ItemId + "/" + UnitPrice + "/" + Quantity,
        success: function (res) {
            if (res.status == "success") {


                $("#cartCounter").text(res.count);


                ToastSuccess('Item Added to Cart Successfully');
                $(btn).replaceWith(`
                                              <div Id="replace-${ItemId}" class="def-number-input number-input safari_only mb-0 w-100;">
                                             <div class="input-group mb-3" style="width: 120px;margin: auto;">
                                                 <div class="input-group-prepend">
                                                  
                                                     <button onclick="updateQuantity(${ItemId},1,-1)" class="btn btn-outline-secondary" type="button" style="padding:6px 10px;"><i class="bi bi-dash-lg"></i></button>
                                                 </div>
                                                 <input Id="qtyInpt-${ItemId}" class="form-control sm-control text-center" size="2" min="0" name="quantity" value="1" readonly />
                                                 <div class="input-group-prepend">
                                                     
                                                     <button onclick="updateQuantity(${ItemId},1,1)" class="btn btn-outline-secondary" type="button" style="padding:6px 10px;"><i class="bi bi-plus-lg"></i></button>
                                                 </div>
                                             </div>
                                         </div>
                                      `);
                $("#cartCounter2").text(res.count);
                $(".loader").css("visibility", "hidden");
                $("#view-cart").css("display", "block");
            }
        }
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
    

    $(".loader").css("visibility", "visible");
    let qntInptId = "#qtyInpt-" + ItemId;
    let tQuantity = currentQuantity + quantity;
    let replaceId = "#replace-" + ItemId;

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
                        //window.location.reload();
                        console.log(qntInptId);
                        $(qntInptId).val(tQuantity);


                        $(replaceId).replaceWith(`
      <div Id="replace-${ItemId}" class="def-number-input number-input safari_only mb-0 w-100;">
     <div class="input-group mb-3" style="width: 120px;margin: auto;">
         <div class="input-group-prepend">

             <button onclick="updateQuantity(${ItemId},${tQuantity},-1)" class="btn btn-outline-secondary" type="button" style="padding:6px 10px;"><i class="bi bi-dash-lg"></i></button>
         </div>
         <input Id="qtyInpt-${ItemId}" class="form-control sm-control text-center" size="2" min="0" name="quantity" value="${tQuantity}" readonly />
         <div class="input-group-prepend">
             
             <button onclick="updateQuantity(${ItemId},${tQuantity},1)" class="btn btn-outline-secondary" type="button" style="padding:6px 10px;"><i class="bi bi-plus-lg"></i></button>
         </div>
     </div>
 </div>
    `);

                    }

                  
                }

                $(".loader").css("visibility", "hidden");
            }
        });
    }
    else {
        $(".loader").css("visibility", "hidden");
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
    
    $(".loader").css("visibility", "visible");
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
                                 <img class="cartItemImg" src="${item.imageUrl}" />
                                  <div>${item.name}</div>
                       </td>
                       <td>${item.unitPrice}</td>
                       <td>
                           <div class="def-number-input number-input safari_only mb-0 w-100 ">
                               <div class="input-group mb-3 cartItemQty" style="margin: auto;">
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
                           <a href="javascript:void(0);" data-id="${item.itemId}" class="btn-delete">
                               <i class="bi bi-trash text-danger"></i>
                           </a>
                       </td>
                 </tr>
                        `;
                tableBody.append(row);
            });

            $("#loader").css("visibility", "hidden");
        },
        error: function(error){
            $("#loader").css("visibility", "hidden");
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

