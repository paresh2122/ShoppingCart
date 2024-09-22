var dataTable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns":
            [
                { data: 'name', "width": "10%" },
                { data: 'email', "width": "10%" },
                { data: 'phoneNumber', "width": "10%" },
                { data: 'company.name', "width": "15%" },
                { data: 'role', "width": "10%" },
                {
                    data: { id: "id", lockoutEnd: "lockoutEnd" },
                    "render": function (data) {
                        var today = new Date().getTime();
                        var lockout = new Date(data.lockoutEnd).getTime();
                        var EmailConfirm=data.emailConfirmed;
                        if (lockout > today) {
                            return `<div class="text-center">
                    
                    <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white "style="cursor:pointer;width:100px;">
                        <i class="bi bi-unlock-fill"></i> Lock
                    </a>
                     <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white "style="cursor:pointer;width:1500px;">
                        <i class="bi bi-unlock-fill"></i> Permission
                    </a>
                    </div>`
                        }
                        else 
                        {
                            return  `<div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white "style="cursor:pointer;width:100px;">
                                    <i class="bi bi-unlock-fill"></i> Unlock
                                </a>
                                <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white "style="cursor:pointer;width:150px;">
                                    <i class="bi bi-unlock-fill"></i> Permission
                                </a>
                            </div>`
                        }

                    },
                    "width": "40%"
                },
                {
                    data:{id:"id",emailConfirmed:"emailConfirmed"},
                "render":function(data){
                    var EmailConfirmed=data.emailConfirmed;
                    if(EmailConfirmed!=true){
                        return `<a onclick=EmailConfirm('${data.id}') class="btn btn-danger text-white "style="cursor:pointer;width:auto;">
                        <i class="bi bi-unlock-fill"></i> NotConfirm
                    </a>`
                    }
                    else{
                        return `<a onclick=EmailConfirm('${data.id}') class="btn btn-success text-white "style="cursor:pointer;width:auto;">
                        <i class="bi bi-unlock-fill"></i> Confirm
                    </a>`

                    }

                },
                "width": "10%"
                

                }
            ]
    });
}
function LockUnlock(id){
    $.ajax({
        type:"POST",
        url:'/Admin/User/LockUnlock',
        data:JSON.stringify(id),
        contentType:"application/json",
        success:function(data){
            if(data.success){
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}
function EmailConfirm(id){
    $.ajax({
        type:"POST",
        url:'/Admin/User/EmailConfirm',
        data:JSON.stringify(id),
        contentType:"application/json",
        success:function(data){
            if(data.success){
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}