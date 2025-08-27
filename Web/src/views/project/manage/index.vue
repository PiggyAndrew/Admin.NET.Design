<template>
	<div class="file-manager-container">
		<splitpanes class="default-theme">
			<pane size="20">
				<CategoryTree ref="categoryTreeRef" @node-click="nodeClick" />
			</pane>
			<pane size="80" style="overflow: auto">
				<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
					<el-form :model="state.queryParams" ref="queryForm" :inline="true">
						<el-form-item label="文件名称">
							<el-input v-model="state.queryParams.fileName" placeholder="文件名称" clearable />
						</el-form-item>
						<el-form-item label="文件类型">
							<el-input v-model="state.queryParams.suffix" placeholder="文件类型" clearable />
						</el-form-item>
						<el-form-item label="开始时间">
							<el-date-picker v-model="state.queryParams.startTime" type="datetime" placeholder="开始时间" value-format="YYYY-MM-DD HH:mm:ss" />
						</el-form-item>
						<el-form-item label="结束时间">
							<el-date-picker v-model="state.queryParams.endTime" type="datetime" placeholder="结束时间" value-format="YYYY-MM-DD HH:mm:ss" />
						</el-form-item>
						<el-form-item>
							<el-button-group>
								<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysFile:page'"> 查询 </el-button>
								<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
							</el-button-group>
						</el-form-item>
						<el-form-item>
							<el-button type="primary" icon="ele-Plus" @click="openUploadDialog" v-auth="'sysFile:uploadFile'"> 上传文件 </el-button>
						</el-form-item>
					</el-form>
				</el-card>

				<el-card class="full-table" shadow="hover" style="margin-top: 5px">
					<el-table :data="state.fileData" style="width: 100%" v-loading="state.loading" border>
						<el-table-column type="index" label="序号" width="55" align="center" />
						<el-table-column prop="fileName" label="文件名称" min-width="200" header-align="center" show-overflow-tooltip />
						<el-table-column prop="suffix" label="文件类型" width="100" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-tag round>{{ scope.row.suffix }}</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="sizeKb" label="文件大小" width="120" align="center" show-overflow-tooltip>
							<template #default="scope">
								{{ formatFileSize(scope.row.sizeKb) }}
							</template>
						</el-table-column>
						<el-table-column prop="url" label="预览" width="80" align="center">
							<template #default="scope">
								<el-image
									style="width: 60px; height: 60px"
									:src="getFileUrl(scope.row)"
									alt="无法预览"
									:lazy="true"
									:hide-on-click-modal="true"
									:preview-src-list="[getFileUrl(scope.row)]"
									:initial-index="0"
									fit="scale-down"
									preview-teleported
								>
									<template #error> </template>
								</el-image>
							</template>
						</el-table-column>
						<el-table-column prop="bucketName" label="存储位置" width="120" align="center" show-overflow-tooltip />
						<el-table-column prop="fileType" label="文件类型" width="100" align="center" show-overflow-tooltip />
						<el-table-column prop="isPublic" label="是否公开" width="100" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-tag v-if="scope.row.isPublic === true" type="success">是</el-tag>
								<el-tag v-else type="danger">否</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="relationName" label="关联分类" width="120" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-tag type="primary">{{ getCategoryName(scope.row.categoryId) }}</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="createTime" label="上传时间" width="160" align="center" show-overflow-tooltip />
						<el-table-column label="操作" width="320" fixed="right" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-button-group>

									<el-button icon="ele-Edit" size="small" type="primary" @click="openEditDialog(scope.row)" v-auth="'sysFile:update'">编辑</el-button>
									<el-button icon="ele-View" size="small" type="primary" @click="openFilePreviewDialog(scope.row)" v-auth="'sysFile:detail'">预览</el-button>
									<el-button icon="ele-Download" size="small" type="primary" @click="downloadFile(scope.row)" v-auth="'sysFile:downloadFile'">下载</el-button>
									<el-button icon="ele-Delete" size="small" type="danger" @click="delFile(scope.row)" v-auth="'sysFile:delete'">删除</el-button>
								</el-button-group>
							</template>
						</el-table-column>
					</el-table>

					<!-- 分页 -->
					<el-pagination
						v-model:current-page="state.tableParams.page"
						v-model:page-size="state.tableParams.pageSize"
						:page-sizes="[10, 20, 50, 100]"
						:total="state.tableParams.total"
						size="small"
						background
						layout="total, sizes, prev, pager, next, jumper"
						@size-change="handleSizeChange"
						@current-change="handleCurrentChange"
						style="margin-top: 15px"
					/>
				</el-card>
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
							<el-option label="相关文件" value="相关文件" />
							<el-option label="归档文件" value="归档文件" />
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
					accept=".jpg,.png,.bmp,.gif,.txt,.xml,.pdf,.xlsx,.docx,.dwg,.zip,.rar"
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

<script lang="ts" setup name="fileManager">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage, UploadInstance } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import VueOfficeDocx from '@vue-office/docx';
import VueOfficeExcel from '@vue-office/excel';
import VueOfficePdf from '@vue-office/pdf';
import '@vue-office/docx/lib/index.css';
import '@vue-office/excel/lib/index.css';

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
	queryParams: {
		fileName: undefined,
		suffix: undefined,
		startTime: undefined,
		endTime: undefined,
		categoryId: undefined, // 选中的分类ID
	},
	tableParams: {
		page: 1,
		pageSize: 20,
		total: 0,
	},
	// 上传相关
	dialogUploadVisible: false,
	fileList: [] as any,
	uploadForm: {
		categoryId: undefined,
		fileType: '相关文件',
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

// 打开编辑对话框
const openEditDialog = (file: SysFile) => {
	state.editFileForm.id = file.id || 0;
	state.editFileForm.fileName = `${file.fileName}${file.suffix}`;
	state.editFileForm.categoryId = file.categoryId; // 假设relationId存储分类ID
	state.editCategoryDialogVisible = true;
};

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
		console.log(params)
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
	state.tableParams.page = 1;
	handleQuery();
};

// 树组件点击
const nodeClick = async (node: any) => {
	state.queryParams.categoryId = node.id;
	console.log(node.id)
	state.uploadForm.categoryId = node.id; // 设置上传时的分类ID
	state.tableParams.page = 1;
	handleQuery();
};

// 打开上传对话框
const openUploadDialog = () => {
	state.fileList = [];
	state.uploadForm.fileType = '相关文件';
	state.uploadForm.isPublic = false;
	// 如果已选中分类，自动设置为上传分类
	if (state.queryParams.categoryId) {
		state.uploadForm.categoryId = state.queryParams.categoryId;
	}
	state.dialogUploadVisible = true;
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
			
			// 上传成功后，更新文件的关联信息
			// 注意：这里可能需要根据实际API调整，确保文件与分类正确关联
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
	console.log(state.categoryTreeData,categoryId)
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
.file-manager-container {
	height: calc(100vh - 84px);
	.full-table {
		height: calc(100vh - 180px);
	}
}
</style>