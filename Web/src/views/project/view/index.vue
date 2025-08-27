<template>
	<div class="file-view-container">
		<splitpanes class="default-theme">
			<pane size="20">
				<CategoryTree ref="categoryTreeRef" @node-click="nodeClick" />
			</pane>
			<pane size="80" >
					<div class="content-area">
						<!-- 筛选区域 -->
						<div class="filter-section">
							<div class="filter-tabs">
								<span class="filter-label">适用：</span>
								<el-radio-group v-model="state.selectedSoftware" size="small" @change="handleQuery">
									<el-radio-button label="全部软件">全部软件</el-radio-button>
									<el-radio-button label="Revit">Revit</el-radio-button>
								</el-radio-group>
							</div>
							
						
							<div class="action-buttons">
								<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
							</div>
						
						</div>

						<!-- 搜索结果信息 -->
						<div class="search-info">
							<div class="search-tags">
								<el-tag v-if="state.queryParams.fileName" closable @close="clearSearch">搜索: {{ state.queryParams.fileName }}</el-tag>
								<el-tag v-if="state.selectedCategory" type="primary">分类: {{ getCategoryName(state.selectedCategory) }}</el-tag>
							</div>
						</div>
						<!-- 文件网格 -->
						<div class="files-grid"  v-loading="state.loading">
							<el-card 
								v-for="file in state.fileData" 
								:key="file.id"
								class="file-card"
								@click="selectFile(file)"
							>
								<div class="file-preview">
									<el-image
										:src="getFileUrl(file)"
										:alt="file.fileName"
										class="file-thumbnail"
										fit="cover"
										:lazy="true"
									>
										<template #error>
											<div class="file-icon">
												<el-icon size="40"><Document /></el-icon>
												<span class="file-type">{{ file.suffix }}</span>
											</div>
										</template>
									</el-image>
									<div class="file-actions">
										<el-button :icon="Download" size="small" circle @click.stop="downloadFile(file)" title="下载"></el-button>
										<el-button :icon="View" size="small" circle @click.stop="openFilePreviewDialog(file)" title="预览"></el-button>
										<el-button :icon="Delete" size="small" circle type="danger" @click.stop="delFile(file)" title="删除"></el-button>
									</div>
								</div>
								<div class="file-info">
									<div class="file-title" :title="file.fileName">{{ file.fileName }}</div>
									<div class="file-meta">
										<span class="file-size">{{ formatFileSize(file.sizeKb) }}</span>
										<span class="file-type-tag">{{ file.suffix }}</span>
									</div>
									<div class="file-category">
										<el-tag v-if="file.relationId" type="primary" size="small">{{ getCategoryName(file.relationId) }}</el-tag>
										<span v-else class="no-category">未分类</span>
									</div>
								</div>
							</el-card >
						</div>

						<!-- 分页 -->
						<div class="pagination-section">
							<el-pagination
								v-model:current-page="state.tableParams.page"
								v-model:page-size="state.tableParams.pageSize"
								:page-sizes="[12, 24, 48, 96]"
								:total="state.tableParams.total"
								layout="total, sizes, prev, pager, next, jumper"
								@size-change="handleSizeChange"
								@current-change="handleCurrentChange"
							/>
						</div>
					</div>
			</pane>
		</splitpanes>

		<!-- 上传文件对话框 -->
		<el-dialog v-model="state.dialogUploadVisible" :lock-scroll="false" draggable width="500px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-UploadFilled /> </el-icon>
					<span> 上传文件 </span>
				</div>
			</template>
			<div>
				<el-form :model="state.uploadForm" label-width="100px">
					<el-form-item label="选择分类">
						<el-tree-select
							v-model="state.uploadForm.categoryId"
							:data="state.categoryTreeData"
							:props="{ value: 'id', label: 'name', children: 'children' }"
							placeholder="请选择分类"
							check-strictly
							clearable
						/>
					</el-form-item>
					<el-form-item label="文件类型">
						<el-select v-model="state.uploadForm.fileType" placeholder="请选择文件类型" style="width: 100%">
							<el-option label="族文件" value="族文件" />
							<el-option label="样板文件" value="样板文件" />
							<el-option label="项目文档" value="项目文档" />
							<el-option label="设计图纸" value="设计图纸" />
						</el-select>
					</el-form-item>
					<el-form-item label="是否公开">
						<el-radio-group v-model="state.uploadForm.isPublic">
							<el-radio :value="false">否</el-radio>
							<el-radio :value="true">是</el-radio>
						</el-radio-group>
					</el-form-item>
				</el-form>
				<el-upload 
					ref="uploadRef" 
					drag 
					:auto-upload="false" 
					:limit="5" 
					:file-list="state.fileList" 
					action 
					:on-change="handleChange" 
					multiple
					accept=".jpg,.png,.bmp,.gif,.txt,.xml,.pdf,.xlsx,.docx,.dwg,.zip,.rar,.rvt,.rfa"
				>
					<el-icon class="el-icon--upload">
						<ele-UploadFilled />
					</el-icon>
					<div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
					<template #tip>
						<div class="el-upload__tip">请上传大小不超过 50MB 的文件，支持多文件上传</div>
					</template>
				</el-upload>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="state.dialogUploadVisible = false">取消</el-button>
					<el-button type="primary" @click="uploadFile" :loading="state.uploading">确定上传</el-button>
				</span>
			</template>
		</el-dialog>

		<!-- 文件分类编辑对话框 -->
		<el-dialog v-model="state.editCategoryDialogVisible" title="设置文件分类" width="500px">
			<el-form :model="state.editFileForm" label-width="100px">
				<el-form-item label="文件名称">
					<el-input v-model="state.editFileForm.fileName" disabled />
				</el-form-item>
				<el-form-item label="选择分类">
					<el-tree-select
						v-model="state.editFileForm.categoryId"
						:data="state.categoryTreeData"
						:props="{ value: 'id', label: 'name', children: 'children' }"
						placeholder="请选择分类"
						check-strictly
						clearable
					/>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.editCategoryDialogVisible = false">取消</el-button>
				<el-button type="primary" @click="saveFileCategory">确定</el-button>
			</template>
		</el-dialog>

		<!-- 文件预览对话框 -->
		<el-drawer :title="state.fileName" v-model="state.dialogDocxVisible" size="50%" destroy-on-close>
			<vue-office-docx :src="state.docxUrl" style="height: 100vh" @rendered="renderedHandler" @error="errorHandler" />
		</el-drawer>
		<el-drawer :title="state.fileName" v-model="state.dialogXlsxVisible" size="50%" destroy-on-close>
			<vue-office-excel :src="state.excelUrl" style="height: 100vh" @rendered="renderedHandler" @error="errorHandler" />
		</el-drawer>
		<el-drawer :title="state.fileName" v-model="state.dialogPdfVisible" size="50%" destroy-on-close>
			<vue-office-pdf :src="state.pdfUrl" style="height: 100vh" @rendered="renderedHandler" @error="errorHandler" />
		</el-drawer>
		<el-image-viewer v-if="state.showViewer" :url-list="state.previewList" :hideOnClickModal="true" @close="state.showViewer = false"></el-image-viewer>
	</div>
</template>

<script lang="ts" setup name="fileView">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage, UploadInstance } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import VueOfficeDocx from '@vue-office/docx';
import VueOfficeExcel from '@vue-office/excel';
import VueOfficePdf from '@vue-office/pdf';
import '@vue-office/docx/lib/index.css';
import '@vue-office/excel/lib/index.css';
import {
	Search,
	Download,
	View,
	Edit,
	Delete,
	Document
} from '@element-plus/icons-vue';

import { getAPI } from '/@/utils/axios-utils';
import { downloadByUrl } from '/@/utils/download';
import { SysCategoryApi, SysFileApi } from '/@/api-services/api';
import { SysCategory, SysFile, PageFileInput, DeleteFileInput } from '/@/api-services/models';
import CategoryTree from '/@/views/category/component/categoryTree.vue';

const categoryTreeRef = ref<InstanceType<typeof CategoryTree>>();
const uploadRef = ref<UploadInstance>();
const state = reactive({
	loading: false,
	uploading: false,
	fileData: [] as Array<SysFile>, // 文件列表数据
	categoryTreeData: [] as Array<SysCategory>, // 分类树所有数据
	selectedCategory: undefined as number | undefined, // 选中的分类ID
	selectedSoftware: '全部软件',
	sortOrder: 'default',
	queryParams: {
		fileName: undefined,
		suffix: undefined,
		startTime: undefined,
		endTime: undefined,
		categoryId: undefined, // 选中的分类ID
	},
	tableParams: {
		page: 1,
		pageSize: 24,
		total: 0,
	},
	// 上传相关
	dialogUploadVisible: false,
	fileList: [] as any,
	uploadForm: {
		categoryId: undefined,
		fileType: '族文件',
		isPublic: false,
	},
	// 文件分类编辑
	editCategoryDialogVisible: false,
	editFileForm: {
		id: 0,
		fileName: '',
		categoryId: undefined,
	},
	// 文件预览
	dialogDocxVisible: false,
	dialogXlsxVisible: false,
	dialogPdfVisible: false,
	showViewer: false,
	docxUrl: '',
	excelUrl: '',
	pdfUrl: '',
	fileName: '',
	previewList: [] as string[],
});

onMounted(async () => {
	await loadCategoryTree();
	handleQuery();
});

// 加载分类树数据
const loadCategoryTree = async () => {
	const res = await getAPI(SysCategoryApi).apiSysCategoryListGet(0);
	state.categoryTreeData = res.data.result ?? [];
};

// 查询操作
const handleQuery = async () => {
	if (state.queryParams.startTime == null) state.queryParams.startTime = undefined;
	if (state.queryParams.endTime == null) state.queryParams.endTime = undefined;

	state.loading = true;
	try {
		const params: PageFileInput = {
			page: state.tableParams.page,
			pageSize: state.tableParams.pageSize,
			fileName: state.queryParams.fileName,
			suffix: state.queryParams.suffix,
			startTime: state.queryParams.startTime,
			endTime: state.queryParams.endTime,
			categoryId: state.queryParams.categoryId,
		};
		const res = await getAPI(SysFileApi).apiSysFilePagePost(params);
		state.fileData = res.data.result?.items ?? [];
		state.tableParams.total = res.data.result?.total ?? 0;
	} catch (error) {
		console.error('查询文件列表失败:', error);
		ElMessage.error('查询文件列表失败');
	} finally {
		state.loading = false;
	}
};

// 重置操作
const resetQuery = () => {
	state.queryParams.fileName = undefined;
	state.queryParams.suffix = undefined;
	state.queryParams.startTime = undefined;
	state.queryParams.endTime = undefined;
	state.queryParams.categoryId = undefined;
	state.selectedCategory = undefined;
	state.selectedSoftware = '全部软件';
	state.sortOrder = 'default';
	state.tableParams.page = 1;
	handleQuery();
};

// 清除搜索
const clearSearch = () => {
	state.queryParams.fileName = undefined;
	handleQuery();
};

// 树组件点击
const nodeClick = async (node: any) => {
	state.queryParams.categoryId = node.id;
	state.selectedCategory = node.id;
	state.uploadForm.categoryId = node.id; // 设置上传时的分类ID
	state.tableParams.page = 1;
	handleQuery();
};



// 通过onChange方法获得文件列表
const handleChange = (file: any, fileList: []) => {
	state.fileList = fileList;
};

// 上传文件
const uploadFile = async () => {
	if (state.fileList.length < 1) {
		ElMessage.warning('请选择要上传的文件');
		return;
	}
	
	if (!state.uploadForm.categoryId) {
		ElMessage.warning('请选择文件分类');
		return;
	}

	state.uploading = true;
	try {
		// 批量上传文件
		for (const fileItem of state.fileList) {
			await getAPI(SysFileApi).apiSysFileUploadFilePostForm(
				fileItem.raw, 
				state.uploadForm.fileType, 
				state.uploadForm.isPublic,
				undefined,
				undefined,
				undefined,
				state.uploadForm.categoryId // 传递分类ID
			);
		}
		
		ElMessage.success(`成功上传 ${state.fileList.length} 个文件`);
		state.dialogUploadVisible = false;
		handleQuery(); // 刷新文件列表
	} catch (error) {
		console.error('上传文件失败:', error);
		ElMessage.error('上传文件失败');
	} finally {
		state.uploading = false;
	}
};

// 文件大小格式化
const formatFileSize = (sizeKb: number | undefined): string => {
	if (!sizeKb) return '0 KB';
	if (sizeKb < 1024) return `${sizeKb} KB`;
	if (sizeKb < 1024 * 1024) return `${(sizeKb / 1024).toFixed(2)} MB`;
	return `${(sizeKb / (1024 * 1024)).toFixed(2)} GB`;
};

// 获取分类名称
const getCategoryName = (categoryId: number | undefined): string => {
	if (!categoryId) return '-';
	const findCategory = (categories: SysCategory[], id: number): string => {
		for (const category of categories) {
			if (category.id === id) return category.name || '-';
			if (category.children && category.children.length > 0) {
				const found = findCategory(category.children, id);
				if (found !== '-') return found;
			}
		}
		return '-';
	};
	return findCategory(state.categoryTreeData, categoryId);
};

// 获取文件URL
const getFileUrl = (file: SysFile): string => {
	if (file.bucketName === 'Local') {
		return `/${file.filePath}/${file.id}${file.suffix}`;
	} else {
		return file.url || '';
	}
};

// 选择文件
const selectFile = (file: SysFile) => {
	console.log('选择文件:', file);
	// 可以在这里添加文件选择的逻辑
};

// 打开文件预览
const openFilePreviewDialog = async (row: SysFile) => {
	if (row.suffix == '.pdf') {
		state.fileName = `【${row.fileName}${row.suffix}】`;
		state.pdfUrl = getFileUrl(row);
		state.dialogPdfVisible = true;
	} else if (row.suffix == '.docx') {
		state.fileName = `【${row.fileName}${row.suffix}】`;
		state.docxUrl = getFileUrl(row);
		state.dialogDocxVisible = true;
	} else if (row.suffix == '.xlsx') {
		state.fileName = `【${row.fileName}${row.suffix}】`;
		state.excelUrl = getFileUrl(row);
		state.dialogXlsxVisible = true;
	} else if (['.jpg', '.png', '.jpeg', '.bmp'].findIndex((e) => e == row.suffix) > -1) {
		state.previewList = [getFileUrl(row)];
		state.showViewer = true;
	} else {
		ElMessage.error('此文件格式不支持预览');
	}
};

// 下载文件
const downloadFile = async (file: SysFile) => {
	try {
		const fileUrl = getFileUrl(file);
		downloadByUrl({ url: fileUrl });
		ElMessage.success('下载开始');
	} catch (error) {
		console.error('下载文件失败:', error);
		ElMessage.error('下载文件失败');
	}
};

// 保存文件分类
const saveFileCategory = async () => {
	try {
		// 更新文件的分类关联
		const updateData: SysFile = {
			id: state.editFileForm.id,
			relationId: state.editFileForm.categoryId,
			relationName: 'category', // 关联对象名称
		};
		
		await getAPI(SysFileApi).apiSysFileUpdatePost(updateData);
		ElMessage.success('分类设置成功');
		state.editCategoryDialogVisible = false;
		handleQuery();
	} catch (error) {
		console.error('设置文件分类失败:', error);
		ElMessage.error('设置文件分类失败');
	}
};

// 删除文件
const delFile = (file: SysFile) => {
	ElMessageBox.confirm(`确定删除文件：【${file.fileName}${file.suffix}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			const deleteData: DeleteFileInput = { id: file.id || 0 };
			await getAPI(SysFileApi).apiSysFileDeletePost(deleteData);
			ElMessage.success('删除成功');
			handleQuery();
		})
		.catch(() => {});
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	state.tableParams.page = 1;
	handleQuery();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
	state.tableParams.page = val;
	handleQuery();
};

// 文件渲染完成
const renderedHandler = () => {};
// 文件渲染失败
const errorHandler = () => {};
</script>

<style scoped lang="scss">
.file-view-container {
	height: calc(100vh - 84px);
}

.files-container {
	min-height: 100%;
	background-color: #f5f7fa;
}

// 顶部导航
.top-nav {
	background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
	padding: 0 20px;
	display: flex;
	justify-content: space-between;
	align-items: center;
	height: 60px;
	color: white;
	
	.nav-tabs {
		display: flex;
		gap: 30px;
		
		.nav-item {
			padding: 8px 16px;
			cursor: pointer;
			border-radius: 4px;
			transition: background-color 0.3s;
			
			&.active {
				background-color: rgba(255, 255, 255, 0.2);
				font-weight: 600;
			}
			
			&:hover {
				background-color: rgba(255, 255, 255, 0.1);
			}
		}
	}
	
	.nav-right {
		.nav-link {
			cursor: pointer;
			&:hover {
				text-decoration: underline;
			}
		}
	}
}

// 搜索区域
.search-section {
	background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
	padding: 20px;
	
	.search-container {
		max-width: 600px;
		margin: 0 auto;
		
		.search-input {
			:deep(.el-input__wrapper) {
				border-radius: 25px;
				box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
			}
			
			:deep(.el-input-group__append) {
				border-radius: 0 25px 25px 0;
				background: #409eff;
				border-color: #409eff;
				
				.el-button {
					background: transparent;
					border: none;
					color: white;
				}
			}
		}
	}
}

// 内容区域
.content-area {
	padding: 20px;
	background: white;
	height: 100%;
	overflow: auto;
}

// 筛选区域
.filter-section {
	margin-bottom: 20px;
	display: flex;
	justify-content: space-between;
	align-items: center;
	flex-wrap: wrap;
	gap: 15px;
	
	.filter-tabs {
		display: flex;
		align-items: center;
		
		.filter-label {
			margin-right: 10px;
			font-weight: 500;
		}
	}
	
	.filter-options {
		:deep(.el-checkbox) {
			margin-right: 20px;
		}
	}
	
	.action-buttons {
		display: flex;
		gap: 10px;
	}
}

// 搜索信息
.search-info {
	display: flex;
	justify-content: space-between;
	align-items: center;
	margin-bottom: 20px;
	padding-bottom: 15px;
	border-bottom: 1px solid #e4e7ed;
	
	.search-tags {
		display: flex;
		gap: 8px;
	}
	
	.result-info {
		display: flex;
		align-items: center;
		gap: 20px;
		
		.result-text {
			font-size: 14px;
			color: #606266;
		}
		
		.view-options {
			display: flex;
			align-items: center;
			gap: 8px;
			font-size: 14px;
			color: #909399;
		}
	}
}

// 文件网格
.files-grid {
	display: grid;
  height: 100%;
	grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
	gap: 20px;
	margin-bottom: 30px;
}

.file-card {
	border: 1px solid #e4e7ed;
  height: 300px;
	border-radius: 8px;
	overflow: hidden;
	cursor: pointer;
	transition: all 0.3s;
	background: white;
	
	&:hover {
		box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
		transform: translateY(-2px);
		
		.file-actions {
			opacity: 1;
		}
	}
	
	.file-preview {
		position: relative;
		height: 150px;
		overflow: hidden;
		
		.file-thumbnail {
			width: 100%;
			height: 100%;
			object-fit: cover;
			background: #f5f7fa;
		}
		
		.file-icon {
			display: flex;
			flex-direction: column;
			align-items: center;
			justify-content: center;
			height: 100%;
			background: #f5f7fa;
			color: #909399;
			
			.file-type {
				margin-top: 8px;
				font-size: 12px;
				font-weight: 500;
			}
		}
		
		.file-actions {
			position: absolute;
			top: 50%;
			left: 50%;
			transform: translate(-50%, -50%);
			display: flex;
			gap: 8px;
			opacity: 0;
			transition: opacity 0.3s;
			
			.el-button {
				background: rgba(255, 255, 255, 0.9);
				border: none;
				
				&:hover {
					background: white;
				}
				
				&.el-button--danger {
					background: rgba(245, 108, 108, 0.9);
					color: white;
					
					&:hover {
						background: #f56c6c;
					}
				}
			}
		}
	}
	
	.file-info {
		padding: 12px;
		
		.file-title {
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
			font-size: 12px;
			color: #909399;
			margin-bottom: 8px;
			
			.file-size {
				color: #606266;
			}
			
			.file-type-tag {
				background: #409eff;
				color: white;
				padding: 2px 6px;
				border-radius: 4px;
				font-size: 10px;
			}
		}
		
		.file-category {
			font-size: 12px;
			
			.no-category {
				color: #c0c4cc;
				font-style: italic;
			}
		}
	}
}

// 分页
.pagination-section {
	display: flex;
	justify-content: center;
	padding: 20px 0;
}

// 响应式设计
@media (max-width: 768px) {
	.files-grid {
		grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
		gap: 15px;
	}
	
	.filter-section {
		flex-direction: column;
		align-items: flex-start;
	}
}
</style>