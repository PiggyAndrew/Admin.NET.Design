<template>
    <div class="project-manage-container">
        <el-header class="toolbar"><!-- 顶部工具栏 -->
            <div class="toolbar">
                <div class="software-tabs">
                    <el-radio-group v-model="currentSoftware" size="large">
                        <el-radio-button label="CAD">
                            <el-icon>
                                <Document />
                            </el-icon>
                            CAD
                        </el-radio-button>
                        <el-radio-button label="Revit">
                            <el-icon>
                                <Files />
                            </el-icon>
                            Revit
                        </el-radio-button>
                    </el-radio-group>
                </div>


            </div>
        </el-header>
        <el-container>
            <el-aside width="280px"><!-- 左侧分类树 -->
                <div class="sidebar">
                    <div class="sidebar-header">
                        <h3>资源分类</h3>
                        <el-button @click="showAddCategoryDialog = true" type="primary" size="small" :icon="Plus"
                            circle />
                    </div>

                    <div class="category-tree">
                        <el-tree ref="categoryTreeRef" :data="categoryTree" :props="treeProps" node-key="id"
                            :default-expand-all="true" :highlight-current="true" @node-click="handleCategoryClick"
                            class="custom-tree">
                            <template #default="{ node, data }">
                                <div class="tree-node">
                                    <el-icon class="node-icon">
                                        <component :is="data.icon || 'Folder'" />
                                    </el-icon>
                                    <span class="node-label">{{ node.label }}</span>
                                    <span class="node-count">({{ data.count || 0 }})</span>
                                </div>
                            </template>
                        </el-tree>
                    </div>
                </div>
            </el-aside>
            <el-main><!-- 主内容区域 -->
                <div class="main-content">



                    <!-- 文件上传 -->
                    <div class="upload-section">
                        <!-- 上传区域头部 -->
                        <div class="upload-header">
                            <span class="section-title">文件上传</span>
                            <el-tag :type="currentSoftware === 'CAD' ? 'warning' : 'primary'">
                            </el-tag>
                        </div>

                        <el-upload ref="uploadRef" class="upload-button" :action="uploadUrl" :headers="uploadHeaders"
                            :data="uploadData" :before-upload="beforeUpload" :on-success="handleUploadSuccess"
                            :on-error="handleUploadError" :on-progress="handleUploadProgress" :file-list="fileList"
                            multiple :accept="getAcceptTypes()" :show-file-list="false">
                            <el-button type="primary" :icon="UploadFilled">
                                将文件拖到此处，或点击上传
                            </el-button>
                            <template #tip>
                                <div class="upload-tip">
                                    支持 {{ getAcceptTypes() }} 格式文件，单个文件不超过 100MB
                                </div>
                            </template>
                        </el-upload>

                        
                    <div class="toolbar">
                        <el-input v-model="searchKeyword" placeholder="搜索资源..." :prefix-icon="Search"
                            style="width: 300px; margin-right: 16px;" clearable />
                        <el-button type="primary" :icon="Refresh" @click="refreshData">刷新</el-button>
                    </div>
                    </div>

                    <!-- 文件列表 -->
                    <div class="file-list-section">
                        <div class="list-header">
                            <span class="section-title">文件列表</span>
                            <div class="list-actions">
                                <el-select v-model="sortBy" placeholder="排序" size="small" style="width: 120px">
                                    <el-option label="名称" value="name" />
                                    <el-option label="大小" value="size" />
                                    <el-option label="时间" value="time" />
                                </el-select>
                                <el-button-group class="view-mode">
                                    <el-button :type="viewMode === 'table' ? 'primary' : ''" @click="viewMode = 'table'"
                                        :icon="List" size="small" />
                                    <el-button :type="viewMode === 'grid' ? 'primary' : ''" @click="viewMode = 'grid'"
                                        :icon="Grid" size="small" />
                                </el-button-group>
                            </div>
                        </div>

                        <!-- 表格视图 -->
                        <div v-if="viewMode === 'table'" class="table-view">
                            <el-table :data="filteredFileList" v-loading="tableLoading" empty-text="暂无内容"
                                @selection-change="handleSelectionChange">
                                <el-table-column type="selection" width="55" />
                                <el-table-column label="缩略图" width="80">
                                    <template #default="{ row }">
                                        <el-image :src="row.thumbnail" class="file-thumbnail" fit="cover">
                                            <template #error>
                                                <div class="image-slot">
                                                    <el-icon>
                                                        <Picture />
                                                    </el-icon>
                                                </div>
                                            </template>
                                        </el-image>
                                    </template>
                                </el-table-column>
                                <el-table-column prop="name" label="名称" min-width="200" show-overflow-tooltip />
                                <el-table-column prop="size" label="所属目录" width="150" />
                                <el-table-column prop="version" label="试用版本" width="120" />
                                <el-table-column prop="createTime" label="创建时间" width="180" />
                                <el-table-column prop="updateTime" label="更新时间" width="180" />
                                <el-table-column prop="status" label="状态" width="100">
                                    <template #default="{ row }">
                                        <el-tag :type="getStatusType(row.status)" size="small">
                                            {{ getStatusText(row.status) }}
                                        </el-tag>
                                    </template>
                                </el-table-column>
                                <el-table-column label="操作" width="200" fixed="right">
                                    <template #default="{ row }">
                                        <el-button size="small" @click="previewFile(row)" :icon="View">预览</el-button>
                                        <el-button size="small" @click="downloadFile(row)"
                                            :icon="Download">下载</el-button>
                                        <el-dropdown @command="handleCommand">
                                            <el-button size="small" :icon="MoreFilled" />
                                            <template #dropdown>
                                                <el-dropdown-menu>
                                                    <el-dropdown-item
                                                        :command="{ action: 'edit', row }">编辑</el-dropdown-item>
                                                    <el-dropdown-item
                                                        :command="{ action: 'move', row }">移动</el-dropdown-item>
                                                    <el-dropdown-item :command="{ action: 'delete', row }"
                                                        divided>删除</el-dropdown-item>
                                                </el-dropdown-menu>
                                            </template>
                                        </el-dropdown>
                                    </template>
                                </el-table-column>
                            </el-table>
                        </div>

                        <!-- 网格视图 -->
                        <div v-else class="grid-view">
                            <div class="file-grid">
                                <div class="file-card" @click="selectFile(file)">
                                    <div class="file-preview">
                                        <el-image :src="file.thumbnail" class="file-image" fit="cover">
                                            <template #error>
                                                <div class="image-slot">
                                                    <el-icon>
                                                        <Picture />
                                                    </el-icon>
                                                </div>
                                            </template>
                                        </el-image>
                                        <div class="file-overlay">
                                            <el-button :icon="View" circle @click.stop="previewFile(file)" />
                                            <el-button :icon="Download" circle @click.stop="downloadFile(file)" />
                                        </div>
                                    </div>
                                    <div class="file-info">
                                        <div class="file-name" :title="file.name">{{ file.name }}</div>
                                        <div class="file-meta">
                                            <span class="file-size">{{ file.size }}</span>
                                            <el-tag :type="getStatusType(file.status)" size="small">
                                                {{ getStatusText(file.status) }}
                                            </el-tag>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- 分页 -->
                        <div class="pagination-section">
                            <el-pagination v-model:current-page="currentPage" v-model:page-size="pageSize"
                                :page-sizes="[10, 20, 50, 100]" :total="totalFiles"
                                layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange"
                                @current-change="handleCurrentChange" background />
                        </div>
                    </div>
                </div>
            </el-main>
        </el-container>


        <!-- 添加分类对话框 -->
        <el-dialog v-model="showAddCategoryDialog" title="添加分类" width="500px">
            <el-form :model="categoryForm" :rules="categoryRules" ref="categoryFormRef" label-width="80px">
                <el-form-item label="分类名称" prop="name">
                    <el-input v-model="categoryForm.name" placeholder="请输入分类名称" />
                </el-form-item>
                <el-form-item label="父级分类" prop="parentId">
                    <el-tree-select v-model="categoryForm.parentId" :data="categoryTree" :props="treeProps"
                        placeholder="请选择父级分类" check-strictly clearable />
                </el-form-item>
                <el-form-item label="图标" prop="icon">
                    <el-select v-model="categoryForm.icon" placeholder="请选择图标">
                        <el-option label="文件夹" value="Folder" />
                        <el-option label="文档" value="Document" />
                        <el-option label="图片" value="Picture" />
                        <el-option label="设置" value="Setting" />
                    </el-select>
                </el-form-item>
            </el-form>
            <template #footer>
                <el-button @click="showAddCategoryDialog = false">取消</el-button>
                <el-button type="primary" @click="handleAddCategory">确定</el-button>
            </template>
        </el-dialog>

        <!-- 文件预览对话框 -->
        <el-dialog v-model="showPreviewDialog" title="文件预览" width="80%" center>
            <div class="preview-content">
                <el-image v-if="previewFileData" :src="previewFileData.thumbnail" class="preview-image" fit="contain" />
            </div>
        </el-dialog>
    </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import {
    Plus,
    Search,
    Refresh,
    Document,
    Files,
    UploadFilled,
    View,
    Download,
    MoreFilled,
    List,
    Grid,
    Picture,
    Folder,
    Setting
} from '@element-plus/icons-vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { UploadProps, UploadUserFile } from 'element-plus'

// 响应式数据
const currentSoftware = ref('CAD')
const searchKeyword = ref('')
const selectedCategory = ref('all')
const viewMode = ref('table')
const sortBy = ref('name')
const currentPage = ref(1)
const pageSize = ref(20)
const totalFiles = ref(0)
const tableLoading = ref(false)
const fileList = ref<UploadUserFile[]>([])
const selectedFiles = ref([])

// 对话框状态
const showAddCategoryDialog = ref(false)
const showPreviewDialog = ref(false)
const previewFileData = ref(null)

// 表单数据
const categoryForm = reactive({
    name: '',
    parentId: null,
    icon: 'Folder'
})

const categoryRules = {
    name: [{ required: true, message: '请输入分类名称', trigger: 'blur' }]
}

// 树形数据
const categoryTree = ref([
    {
        id: 'all',
        label: '全部资源',
        icon: 'Folder',
        count: 0,
        children: [
            {
                id: 'cad',
                label: 'CAD资源',
                icon: 'Document',
                count: 0,
                children: [
                    { id: 'cad-blocks', label: '图块库', icon: 'Picture', count: 0 },
                    { id: 'cad-templates', label: '模板文件', icon: 'Files', count: 0 },
                    { id: 'cad-standards', label: '标准图库', icon: 'Setting', count: 0 }
                ]
            },
            {
                id: 'revit',
                label: 'Revit资源',
                icon: 'Files',
                count: 0,
                children: [
                    { id: 'revit-families', label: '族文件', icon: 'Document', count: 0 },
                    { id: 'revit-templates', label: '项目模板', icon: 'Files', count: 0 },
                    { id: 'revit-materials', label: '材质库', icon: 'Picture', count: 0 }
                ]
            }
        ]
    }
])

const treeProps = {
    children: 'children',
    label: 'label'
}

// 文件列表数据
const mockFileList = ref([
    {
        id: 1,
        name: '标准图块_电气符号.dwg',
        size: '2.5MB',
        version: 'AutoCAD 2020',
        createTime: '2024-01-15 10:30:00',
        updateTime: '2024-01-15 10:30:00',
        status: 'active',
        thumbnail: '/api/placeholder/100/80',
        category: 'cad-blocks'
    },
    {
        id: 2,
        name: '建筑模板_住宅.dwt',
        size: '1.8MB',
        version: 'AutoCAD 2021',
        createTime: '2024-01-14 15:20:00',
        updateTime: '2024-01-14 15:20:00',
        status: 'active',
        thumbnail: '/api/placeholder/100/80',
        category: 'cad-templates'
    },
    {
        id: 3,
        name: '门窗族文件.rfa',
        size: '3.2MB',
        version: 'Revit 2023',
        createTime: '2024-01-13 09:15:00',
        updateTime: '2024-01-13 09:15:00',
        status: 'draft',
        thumbnail: '/api/placeholder/100/80',
        category: 'revit-families'
    }
])

// 计算属性
const filteredFileList = computed(() => {
    let filtered = mockFileList.value

    // 根据软件类型筛选
    if (currentSoftware.value === 'CAD') {
        filtered = filtered.filter(file => file.category.startsWith('cad'))
    } else if (currentSoftware.value === 'Revit') {
        filtered = filtered.filter(file => file.category.startsWith('revit'))
    }

    // 根据分类筛选
    if (selectedCategory.value !== 'all') {
        filtered = filtered.filter(file => file.category === selectedCategory.value)
    }

    // 根据搜索关键词筛选
    if (searchKeyword.value) {
        filtered = filtered.filter(file =>
            file.name.toLowerCase().includes(searchKeyword.value.toLowerCase())
        )
    }

    totalFiles.value = filtered.length
    return filtered
})

// 上传配置
const uploadUrl = ref('/api/upload')
const uploadHeaders = ref({
    'Authorization': 'Bearer token'
})
const uploadData = computed(() => ({
    software: currentSoftware.value,
    category: selectedCategory.value
}))

// 方法
const getCurrentCategoryName = () => {
    const findCategory = (nodes: any[], id: string): any => {
        for (const node of nodes) {
            if (node.id === id) return node
            if (node.children) {
                const found = findCategory(node.children, id)
                if (found) return found
            }
        }
        return null
    }

    const category = findCategory(categoryTree.value, selectedCategory.value)
    return category ? category.label : '全部资源'
}

const getAcceptTypes = () => {
    if (currentSoftware.value === 'CAD') {
        return '.dwg,.dxf,.dwt'
    } else if (currentSoftware.value === 'Revit') {
        return '.rfa,.rvt,.rte'
    }
    return '*'
}

const getStatusType = (status: string) => {
    const statusMap: Record<string, string> = {
        active: 'success',
        draft: 'warning',
        archived: 'info'
    }
    return statusMap[status] || 'info'
}

const getStatusText = (status: string) => {
    const statusMap: Record<string, string> = {
        active: '正常',
        draft: '草稿',
        archived: '已归档'
    }
    return statusMap[status] || '未知'
}

const handleCategoryClick = (data: any) => {
    selectedCategory.value = data.id
}

const refreshData = () => {
    tableLoading.value = true
    setTimeout(() => {
        tableLoading.value = false
        ElMessage.success('数据刷新成功')
    }, 1000)
}

const beforeUpload: UploadProps['beforeUpload'] = (file) => {
    const acceptTypes = getAcceptTypes().split(',')
    const fileExt = '.' + file.name.split('.').pop()?.toLowerCase()

    if (!acceptTypes.includes(fileExt)) {
        ElMessage.error(`只支持 ${getAcceptTypes()} 格式的文件`)
        return false
    }

    if (file.size > 100 * 1024 * 1024) {
        ElMessage.error('文件大小不能超过 100MB')
        return false
    }

    return true
}

const handleUploadSuccess = (response: any, file: any) => {
    ElMessage.success('文件上传成功')
    refreshData()
}

const handleUploadError = (error: any) => {
    ElMessage.error('文件上传失败')
}

const handleUploadProgress = (event: any, file: any) => {
    // 处理上传进度
}

const handleSelectionChange = (selection: any[]) => {
    selectedFiles.value = selection
}

const previewFile = (file: any) => {
    previewFileData.value = file
    showPreviewDialog.value = true
}

const downloadFile = (file: any) => {
    ElMessage.success(`开始下载 ${file.name}`)
}

const selectFile = (file: any) => {
    console.log('选择文件:', file)
}

const handleCommand = (command: any) => {
    const { action, row } = command

    switch (action) {
        case 'edit':
            ElMessage.info('编辑功能开发中')
            break
        case 'move':
            ElMessage.info('移动功能开发中')
            break
        case 'delete':
            ElMessageBox.confirm('确定要删除这个文件吗？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                ElMessage.success('删除成功')
            })
            break
    }
}

const handleAddCategory = () => {
    // 添加分类逻辑
    ElMessage.success('分类添加成功')
    showAddCategoryDialog.value = false
}

const handleSizeChange = (size: number) => {
    pageSize.value = size
}

const handleCurrentChange = (page: number) => {
    currentPage.value = page
}

onMounted(() => {
    refreshData()
})
</script>

<style scoped lang="scss">
.project-manage-container {
    display: flex;
    flex-direction: column;
    height: 100vh;
    background-color: #f5f7fa;
}

// 顶部工具栏
.header-toolbar {
    background: white;
    border-bottom: 1px solid #e4e7ed;
    padding: 12px 24px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    min-height: 60px;

    .header-left {
        .software-tabs {
            :deep(.el-radio-button__inner) {
                padding: 12px 24px;
                font-weight: 500;
                display: flex;
                align-items: center;
                gap: 8px;
            }
        }
    }

    .header-right {
        .breadcrumb {
            :deep(.el-breadcrumb__item) {
                font-size: 14px;
            }
        }
    }
}

// 主容器
.el-container {
    flex: 1;
    overflow: hidden;
}

// 左侧边栏
.sidebar {
    height: 100%;
    background: white;
    border-right: 1px solid #e4e7ed;
    display: flex;
    flex-direction: column;

    .sidebar-header {
        padding: 20px;
        border-bottom: 1px solid #f0f2f5;
        display: flex;
        justify-content: space-between;
        align-items: center;
        background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);

        h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 600;
            color: #2c3e50;
        }
    }

    .category-tree {
        flex: 1;
        padding: 10px 0;
        overflow-y: auto;

        .custom-tree {
            :deep(.el-tree-node__content) {
                padding: 8px 20px;
                height: auto;

                &:hover {
                    background-color: #f5f7fa;
                }
            }

            :deep(.el-tree-node.is-current > .el-tree-node__content) {
                background-color: #e6f0ff;
                color: #409eff;
            }

            .tree-node {
                display: flex;
                align-items: center;
                width: 100%;

                .node-icon {
                    margin-right: 8px;
                    font-size: 16px;
                }

                .node-label {
                    flex: 1;
                    font-size: 14px;
                }

                .node-count {
                    font-size: 12px;
                    color: #909399;
                }
            }
        }
    }
}

// 主内容区域
.main-content {
    height: 100%;
    display: flex;
    flex-direction: column;
    overflow: hidden;

    // 操作工具栏
    .action-toolbar {
        background: white;
        padding: 16px 24px;
        border-bottom: 1px solid #e4e7ed;
        display: flex;
        justify-content: space-between;
        align-items: center;
        flex-wrap: wrap;
        gap: 16px;

        .toolbar-left {
            display: flex;
            align-items: center;
            gap: 16px;

            .upload-section {
                display: flex;
                align-items: center;
                gap: 12px;

                .upload-button {
                    display: flex;
                    align-items: center;

                    .upload-tip {
                        font-size: 12px;
                        color: #909399;
                        margin-left: 12px;
                        white-space: nowrap;
                    }
                }

                .software-tag {
                    margin-left: 8px;
                }
            }
        }

        .toolbar-right {
            display: flex;
            align-items: center;
            gap: 12px;
        }
    }

    .file-list-section {
        flex: 1;
        padding: 24px;
        background: white;
        overflow: hidden;
        display: flex;
        flex-direction: column;

        .list-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 16px;

            .section-title {
                font-weight: 600;
                font-size: 16px;
                color: #2c3e50;
            }

            .list-actions {
                display: flex;
                align-items: center;
                gap: 12px;
            }
        }

        .table-view {
            flex: 1;
            overflow: hidden;

            .file-thumbnail {
                width: 60px;
                height: 45px;
                border-radius: 6px;

                .image-slot {
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    width: 100%;
                    height: 100%;
                    background: #f5f7fa;
                    color: #909399;
                }
            }
        }

        .grid-view {
            flex: 1;
            overflow-y: auto;

            .file-grid {
                display: grid;
                grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
                gap: 16px;
                padding: 16px 0;
            }

            .file-card {
                border: 1px solid #e4e7ed;
                border-radius: 8px;
                overflow: hidden;
                cursor: pointer;
                transition: all 0.3s;
                background: white;

                &:hover {
                    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
                    transform: translateY(-2px);

                    .file-overlay {
                        opacity: 1;
                    }
                }

                .file-preview {
                    position: relative;
                    height: 120px;

                    .file-image {
                        width: 100%;
                        height: 100%;

                        .image-slot {
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            width: 100%;
                            height: 100%;
                            background: #f5f7fa;
                            color: #909399;
                            font-size: 24px;
                        }
                    }

                    .file-overlay {
                        position: absolute;
                        top: 0;
                        left: 0;
                        right: 0;
                        bottom: 0;
                        background: rgba(0, 0, 0, 0.6);
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        gap: 12px;
                        opacity: 0;
                        transition: opacity 0.3s;

                        .el-button {
                            background: rgba(255, 255, 255, 0.9);
                            border: none;
                        }
                    }
                }

                .file-info {
                    padding: 12px;

                    .file-name {
                        font-size: 14px;
                        font-weight: 500;
                        color: #303133;
                        margin-bottom: 8px;
                        overflow: hidden;
                        text-overflow: ellipsis;
                        white-space: nowrap;
                    }

                    .file-meta {
                        display: flex;
                        justify-content: space-between;
                        align-items: center;

                        .file-size {
                            font-size: 12px;
                            color: #909399;
                        }
                    }
                }
            }
        }

        .pagination-section {
            padding: 16px 0;
            border-top: 1px solid #f0f2f5;
            display: flex;
            justify-content: center;
            margin-top: auto;
        }
    }
}

// 预览对话框
.preview-content {
    text-align: center;

    .preview-image {
        max-width: 100%;
        max-height: 60vh;
        border-radius: 8px;
    }
}

// 响应式设计
@media (max-width: 1200px) {
    .action-toolbar {

        .toolbar-left,
        .toolbar-right {
            flex: 1;
            justify-content: space-between;
        }
    }
}

@media (max-width: 768px) {
    .project-manage-container {
        .header-toolbar {
            flex-direction: column;
            gap: 12px;
            align-items: stretch;
            min-height: auto;
            padding: 16px;

            .header-left,
            .header-right {
                justify-content: center;
            }
        }

        .el-container {
            flex-direction: column;
        }

        .sidebar {
            width: 100%;
            height: auto;
            max-height: 200px;

            .category-tree {
                max-height: 150px;
            }
        }

        .action-toolbar {
            flex-direction: column;
            align-items: stretch;
            gap: 12px;

            .toolbar-left,
            .toolbar-right {
                justify-content: center;
            }
        }
    }
}
</style>