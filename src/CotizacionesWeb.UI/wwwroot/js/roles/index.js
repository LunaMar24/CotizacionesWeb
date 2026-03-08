// ============================================
// ROLES INDEX - JavaScript
// ============================================

$(document).ready(function() {
    
    // ============================================
    // FILTROS DE BÚSQUEDA
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
    // SELECCIONAR TODOS LOS PERMISOS POR CATEGORÍA
    // ============================================
    
    $(document).on('click', '.categoria-header', function(e) {
        // Solo si no se hizo click en el badge
        if (!$(e.target).hasClass('badge')) {
            const categoriaGroup = $(this).closest('.categoria-group');
            const checkboxes = categoriaGroup.find('.permiso-checkbox');
            const allChecked = checkboxes.filter(':checked').length === checkboxes.length;
            
            // Toggle: si todos están marcados, desmarcar todos; sino, marcar todos
            checkboxes.prop('checked', !allChecked);
            
            // Feedback visual
            $(this).css('opacity', '0.8');
            setTimeout(() => {
                $(this).css('opacity', '1');
            }, 200);
        }
    });
    
    // Agregar cursor pointer al header
    $(document).on('mouseenter', '.categoria-header', function() {
        $(this).css('cursor', 'pointer');
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
        
        $.post('/Roles/Delete', {
            id: rolId,
            __RequestVerificationToken: token
        }, function(response) {
            if (response.success) {
                $('#deleteConfirmModal').modal('hide');
                showNotification('success', response.message);
                setTimeout(() => location.reload(), 1500);
            } else {
                showNotification('error', response.message);
            }
        }).fail(function() {
            showNotification('error', 'Error al eliminar el rol.');
        });
    });
    
    // ============================================
    // NOTIFICACIONES
    // ============================================
    
    // Usar la función global de notificaciones definida en site.js
});

