// ============================================
// ROLES INDEX - JavaScript
// ============================================

$(document).ready(function() {
    
    // ============================================
    // FILTROS DE BUSQUEDA
    // ============================================
    
    function filterTable() {
        const nameFilter = $('#filterName').val().toLowerCase();
        const statusFilter = $('#filterStatus').val();
        
        $('#rolesTable tbody tr').each(function() {
            const row = $(this);
            const nombre = row.attr('data-nombre') || '';
            const activo = row.attr('data-activo') || '';
            
            const matchName = nombre.includes(nameFilter);
            const matchStatus = statusFilter === '' || activo === statusFilter;
            
            if (matchName && matchStatus) {
                row.show();
            } else {
                row.hide();
            }
        });
    }
    
    $('#filterName, #filterStatus').on('keyup change', filterTable);
    
    // ============================================
    // EDITAR ROL
    // ============================================
    
    $('.btn-edit-rol').on('click', function() {
        const rolId = $(this).data('rol-id');
        
        $('#editRolContent').html('<div class="text-center p-5"><div class="spinner-border text-primary" role="status"></div></div>');
        $('#editRolModal').modal('show');
        
        $.get('/Roles/Edit/' + rolId, function(data) {
            $('#editRolContent').html(data);
        }).fail(function() {
            $('#editRolContent').html('<div class="alert alert-danger m-3">Error al cargar el rol.</div>');
        });
    });
    
    // Submit del formulario de edición
    $(document).on('submit', '#updateRolForm', function(e) {
        e.preventDefault();
        
        const form = $(this);
        const formData = form.serialize();
        
        $.post('/Roles/Edit', formData, function(response) {
            if (response.success) {
                $('#editRolModal').modal('hide');
                showNotification('success', response.message);
                setTimeout(() => location.reload(), 1500);
            } else {
                showNotification('error', response.message);
            }
        }).fail(function() {
            showNotification('error', 'Error al actualizar el rol.');
        });
    });
    
    // ============================================
    // GESTIONAR PERMISOS
    // ============================================
    
    $('.btn-manage-permisos').on('click', function() {
        const rolId = $(this).data('rol-id');
        
        $('#permisosManageContent').html('<div class="text-center p-5"><div class="spinner-border text-primary" role="status"></div></div>');
        $('#permisosManageModal').modal('show');
        
        $.get('/Roles/GetPermisos/' + rolId, function(data) {
            $('#permisosManageContent').html(data);
        }).fail(function() {
            $('#permisosManageContent').html('<div class="alert alert-danger m-3">Error al cargar los permisos.</div>');
        });
    });
    
    // Submit del formulario de permisos
    $(document).on('submit', '#updatePermisosForm', function(e) {
        e.preventDefault();
        
        const form = $(this);
        const formData = form.serialize();
        
        $.post('/Roles/UpdatePermisos', formData, function(response) {
            if (response.success) {
                $('#permisosManageModal').modal('hide');
                showNotification('success', response.message);
                setTimeout(() => location.reload(), 1500);
            } else {
                showNotification('error', response.message);
            }
        }).fail(function() {
            showNotification('error', 'Error al actualizar los permisos.');
        });
    });
    
    // ============================================
    // SELECCIONAR TODOS LOS PERMISOS POR CATEGORIA
    // ============================================
    
    // Manejar cambio en el checkbox de categoría
    $(document).on('change', '.categoria-checkbox', function() {
        const categoria = $(this).closest('.categoria-header').data('categoria');
        const isChecked = $(this).is(':checked');
        
        // Marcar/desmarcar todos los permisos de esta categoría
        $(`.permiso-checkbox[data-categoria="${categoria}"]`).prop('checked', isChecked);
        
        // Feedback visual
        const header = $(this).closest('.categoria-header');
        header.css('opacity', '0.8');
        setTimeout(() => {
            header.css('opacity', '1');
        }, 200);
    });
    
    // Actualizar checkbox de categoría cuando cambian los permisos individuales
    $(document).on('change', '.permiso-checkbox', function() {
        const categoria = $(this).data('categoria');
        const categoriaGroup = $(`.categoria-header[data-categoria="${categoria}"]`);
        const checkboxes = $(`.permiso-checkbox[data-categoria="${categoria}"]`);
        const checkedCount = checkboxes.filter(':checked').length;
        const totalCount = checkboxes.length;
        
        const categoriaCheckbox = categoriaGroup.find('.categoria-checkbox');
        
        if (checkedCount === 0) {
            // Ninguno marcado
            categoriaCheckbox.prop('checked', false);
            categoriaCheckbox.prop('indeterminate', false);
        } else if (checkedCount === totalCount) {
            // Todos marcados
            categoriaCheckbox.prop('checked', true);
            categoriaCheckbox.prop('indeterminate', false);
        } else {
            // Algunos marcados (estado indeterminado)
            categoriaCheckbox.prop('checked', false);
            categoriaCheckbox.prop('indeterminate', true);
        }
    });
    
    // Inicializar estado de checkboxes de categoría al cargar el modal
    $(document).on('shown.bs.modal', '#permisosManageModal', function() {
        $('.categoria-group').each(function() {
            const categoriaGroup = $(this);
            const categoria = categoriaGroup.find('.categoria-header').data('categoria');
            const checkboxes = categoriaGroup.find('.permiso-checkbox');
            const checkedCount = checkboxes.filter(':checked').length;
            const totalCount = checkboxes.length;
            
            const categoriaCheckbox = categoriaGroup.find('.categoria-checkbox');
            
            if (checkedCount === 0) {
                categoriaCheckbox.prop('checked', false);
                categoriaCheckbox.prop('indeterminate', false);
            } else if (checkedCount === totalCount) {
                categoriaCheckbox.prop('checked', true);
                categoriaCheckbox.prop('indeterminate', false);
            } else {
                categoriaCheckbox.prop('checked', false);
                categoriaCheckbox.prop('indeterminate', true);
            }
        });
    });
    
    // ============================================
    // ELIMINAR ROL
    // ============================================
    
    $('.btn-delete-rol').on('click', function() {
        const rolId = $(this).data('rol-id');
        const rolNombre = $(this).data('rol-nombre');
        const cantUsuarios = parseInt($(this).data('cant-usuarios'));
        
        $('#deleteRolId').val(rolId);
        $('#deleteRolNombre').text(rolNombre);
        hideModalAlert('deleteRolAlert');
        
        if (cantUsuarios > 0) {
            $('#deleteWarning').show();
            $('#confirmDeleteBtn').prop('disabled', true);
        } else {
            $('#deleteWarning').hide();
            $('#confirmDeleteBtn').prop('disabled', false);
        }
        
        $('#deleteConfirmModal').modal('show');
    });
    
    $('#confirmDeleteBtn').on('click', function() {
        const rolId = $('#deleteRolId').val();
        const token = $('input[name="__RequestVerificationToken"]').val();
        
        $(this).prop('disabled', true);
        
        $.post('/Roles/Delete', {
            id: rolId,
            __RequestVerificationToken: token
        }, function(response) {
            if (response.success) {
                $('#deleteConfirmModal').modal('hide');
                showNotification('success', response.message);
                setTimeout(() => location.reload(), 1500);
            } else {
                showModalAlert('deleteRolAlert', response.message);
                $('#confirmDeleteBtn').prop('disabled', false);
            }
        }).fail(function() {
            showModalAlert('deleteRolAlert', 'Error al eliminar el rol.');
            $('#confirmDeleteBtn').prop('disabled', false);
        });
    });
    
});